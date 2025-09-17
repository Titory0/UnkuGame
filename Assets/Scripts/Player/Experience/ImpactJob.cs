using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

//[BurstCompile]
public struct ImpactJob : IJob
{
    [ReadOnly] public NativeArray<float2> inPositions;
    [ReadOnly] public NativeArray<float> inLifetimes;

    [ReadOnly] public float2 impactPoint;
    [ReadOnly] public float impactDistance;
    [ReadOnly] public float activationTime;

    [WriteOnly] public NativeList<int> outIndices;
    [WriteOnly] public NativeList<bool> outAvailability;
    public void Execute()
    {
        for (int i = 0; i < inPositions.Length; i++)
        {
            if(inLifetimes[i] < activationTime)
            {
                continue;
            }
            if (math.distancesq(inPositions[i], impactPoint) < math.square(impactDistance))
            {
                outIndices.Add(i);
                outAvailability[i] = false;
            }
        }
    }
}
