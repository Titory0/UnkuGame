using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

//[BurstCompile]
public struct LifetimeJob : IJob
{
    public NativeArray<float> inLifeTime;
    [ReadOnly] public float deltaTime;
    public void Execute()
    {
        for (int i = 0; i < inLifeTime.Length; i++)
        {
            inLifeTime[i] = inLifeTime[i] + deltaTime;
        }
    }
}
