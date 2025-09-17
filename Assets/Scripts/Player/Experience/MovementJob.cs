using Icaria.Engine.Procedural;
using System.Numerics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

//[BurstCompile]
public struct MovementJob : IJob
{
    public NativeArray<float2> positions;
    public NativeArray<float2> velocities;
    public NativeList<float2> outVelocity;

    [ReadOnly] public float deltaTime;

    public void Execute()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = (positions[i] + velocities[i] * deltaTime);
            outVelocity[i] = velocities[i];
        }
    }
}
