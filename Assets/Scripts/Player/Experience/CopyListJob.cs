using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
public struct CopyListJob : IJob
{
    public NativeList<float2> list1;
    public NativeList<float2> list2;
    public void Execute()
    {
        for (int i = 0; i < list1.Length; i++)
        {
            list2[i] = list1[i];
        }
    }
}
