using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class ExperienceSystem : MonoBehaviour
{
    public static ExperienceSystem Instance;

    public GameObject experienceOrbPrefab;
    public VisualEffect experienceVFX;
    public int poolCount;
    public float spawnRadius;
    public float spawnVelocity;
    public AnimationCurve ySizeBySpeed;
    [SerializeField] public WindParameters windParameters;
    [SerializeField] public ParticleParameters particleParameters;

    public Transform experienceTarget;
    public float impactDistance = 0.5f;

    public List<Transform> experienceTransformList = new List<Transform>();

    public NativeList<ParticleOrder> particleOrders;
    public NativeList<int> experiencePool;

    public NativeList<float2> positions;
    public NativeList<float2> velocities;
    public NativeList<float> lifetimes;
    public NativeList<int> impactIndices;
    public NativeList<bool> usableParticles;

    private JobHandle lastFrameJobHandle;

    private void Awake()
    {
        if (Instance != null)
        {
            print("More than one ExperienceSystem in the scene. Deleting the new one.");
            Destroy(this);
            return;
        }
        Instance = this;

        particleOrders = new NativeList<ParticleOrder>(Allocator.Persistent);
        experiencePool = new NativeList<int>(Allocator.Persistent);

        positions = new NativeList<float2>(Allocator.Persistent);
        velocities = new NativeList<float2>(Allocator.Persistent);
        lifetimes = new NativeList<float>(Allocator.Persistent);
        impactIndices = new NativeList<int>(Allocator.Persistent);
        usableParticles = new NativeList<bool>(Allocator.Persistent);
    }

    void Start()
    {
        for (int i = 0; i < poolCount; i++)
        {
            experiencePool.Add(i);

            Vector2 position = new float2(0, 0);
            Vector2 velocity = new float2(0, 0);
            positions.Add(position);
            velocities.Add(velocity);
            lifetimes.Add(0);
            usableParticles.Add(false);
            GameObject go = Instantiate(experienceOrbPrefab, position, transform.rotation);
            go.transform.parent = transform;
            experienceTransformList.Add(go.transform);
        }
    }

    void Update()
    {
        lastFrameJobHandle.Complete();

        for (int i = 0; i < experienceTransformList.Count; i++)
        {
            experienceTransformList[i].position = (Vector2)positions[i];
            experienceTransformList[i].rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)velocities[i]);
            experienceTransformList[i].localScale = new Vector2(ySizeBySpeed.Evaluate(math.length(velocities[i])), 1);
            experienceTransformList[i].gameObject.SetActive(usableParticles[i]);
        }

        for (int j = 0; j < impactIndices.Length; j++)
        {
            experiencePool.Add(impactIndices[j]);
            experienceVFX.transform.position = (Vector2)positions[impactIndices[j]];
            experienceVFX.Play();
        }
        impactIndices.Clear();

        for (int k = 0; k < particleOrders.Length; k++)
        {
            UnPoolParticles(particleOrders[k]);
        }
        particleOrders.Clear();

        SchedulesJobs();
    }

    public void SpawnParticles(ParticleOrder requiredParticles)
    {
        particleOrders.Add(requiredParticles);
    }

    public void SchedulesJobs()
    {
        float dt = Time.deltaTime;
        int count = experienceTransformList.Count;

        NativeList<float2> tempPosition = new NativeList<float2>(count, Allocator.TempJob);
        NativeList<float2> tempWindVelocities = new NativeList<float2>(count, Allocator.TempJob);
        NativeList<float2> tempAccelerationVelocities = new NativeList<float2>(count, Allocator.TempJob);

        tempPosition.ResizeUninitialized(count);
        tempWindVelocities.ResizeUninitialized(count);
        tempAccelerationVelocities.ResizeUninitialized(count);


        JobHandle lifetimeJobHandle = new LifetimeJob
        {
            inLifeTime = lifetimes.AsArray(),
            deltaTime = dt,
        }.Schedule();

        JobHandle windJobHandle = new WindJob
        {
            inPosition = positions.AsArray(),
            inVelocities = velocities.AsArray(),
            outVelocities = tempWindVelocities,

            noisePosition = windParameters.windNoiseDirection * Time.time,
            noiseOffset = windParameters.windNoiseOffset,
            noiseScale = windParameters.windNoiseScale,

            windIntensity = windParameters.windIntensity,
            windDrag = windParameters.windDrag,
            precision = windParameters.precision,
            deltaTime = dt,

        }.Schedule();

        windJobHandle = JobHandle.CombineDependencies(windJobHandle, lifetimeJobHandle);

        JobHandle accelerationJobHandle = new AccelerationJob
        {
            inPositions = positions.AsArray(),
            inVelocities = tempWindVelocities.AsDeferredJobArray(),
            inLifeTimes = lifetimes.AsArray(),

            acceleration = particleParameters.acceleration,
            drag = particleParameters.drag,
            maxSpeed = particleParameters.maxSpeed,
            activationTime = particleParameters.backTime,
            targetPosition = (float2)(Vector2)transform.position,

            outVelocities = tempAccelerationVelocities,
            deltaTime = dt
        }.Schedule(windJobHandle);


        JobHandle movementJobHandle = new MovementJob
        {
            positions = positions.AsArray(),
            velocities = tempAccelerationVelocities.AsDeferredJobArray(),
            outVelocity = velocities,

            deltaTime = dt,

        }.Schedule(accelerationJobHandle);

        JobHandle impactJobHandle = new ImpactJob
        {
            inPositions = positions.AsArray(),
            inLifetimes = lifetimes.AsArray(),

            impactPoint = (float2)(Vector2)experienceTarget.position,
            impactDistance = impactDistance,
            activationTime = particleParameters.backTime,

            outIndices = impactIndices,
            outAvailability = usableParticles,

        }.Schedule(movementJobHandle);

        tempAccelerationVelocities.Dispose(impactJobHandle);
        tempWindVelocities.Dispose(impactJobHandle);
        tempPosition.Dispose(impactJobHandle);

        lastFrameJobHandle = impactJobHandle;
    }

    void UnPoolParticles(ParticleOrder requiredParticles)
    {
        for (int i = 0; i < requiredParticles.count; i++)
        {
            if (experiencePool.Length == 0) { return; }

            int index = experiencePool[experiencePool.Length - 1];
            experiencePool.RemoveAt(experiencePool.Length - 1);

            Vector2 velocity = VectorUtils.GetRandomDirection(requiredParticles.velocity, requiredParticles.spread);
            float speedRange = requiredParticles.speedRange;

            positions[index] = requiredParticles.position;
            velocities[index] = velocity * (requiredParticles.speed + Random.Range(-speedRange, speedRange));
            lifetimes[index] = 0;
            usableParticles[index] = true;
        }
    }

    void OnDestroy()
    {
        lastFrameJobHandle.Complete();

        experiencePool.Dispose();
        particleOrders.Dispose();

        positions.Dispose();
        velocities.Dispose();
        lifetimes.Dispose();
        impactIndices.Dispose();
        usableParticles.Dispose();
    }
}



[Serializable]
public struct ParticleOrder
{
    public int count;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public Vector2 velocity;
    public float speed;
    public float speedRange;
    public float spread;
}

[Serializable]
public struct WindParameters
{
    public float2 windNoiseDirection;
    public float2 windNoiseOffset;
    public float2 windNoiseScale;
    public float windIntensity;
    public float windDrag;
    public float precision;
}
[Serializable]
public struct ParticleParameters
{
    public float acceleration;
    public float drag;
    public float maxSpeed;
    public float backTime;
}
