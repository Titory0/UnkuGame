using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

//[BurstCompile]
public struct AccelerationJob : IJob
{
    [ReadOnly] public float2 targetPosition;
    [ReadOnly] public NativeArray<float2> inPositions;
    [ReadOnly] public NativeArray<float2> inVelocities;
    [ReadOnly] public NativeArray<float> inLifeTimes;

    [ReadOnly] public float acceleration;
    [ReadOnly] public float drag;
    [ReadOnly] public float maxSpeed;
    [ReadOnly] public float deltaTime;
    [ReadOnly] public float activationTime;

    [WriteOnly] public NativeList<float2> outVelocities;

    public void Execute()
    {
        for (int i = 0; i < inPositions.Length; i++)
        {
            if (inLifeTimes[i] < activationTime)
            {
                outVelocities[i] = inVelocities[i];
                continue;
            }
            float2 direction = math.normalize(targetPosition - inPositions[i]);
            float2 accelerationVector = direction * acceleration;

            float dot = math.dot(math.normalize(inVelocities[i]), direction);

            float2 draggedVelocity = inVelocities[i] - (math.normalize(inVelocities[i]) * drag * (1- math.abs(dot)) * deltaTime);
            float2 newVelocity = draggedVelocity + accelerationVector * deltaTime;
            newVelocity = math.clamp(newVelocity, -maxSpeed, maxSpeed);

            outVelocities[i] = (newVelocity);
        }
    }
}
