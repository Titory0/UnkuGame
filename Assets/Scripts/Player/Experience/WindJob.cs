using Icaria.Engine.Procedural;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;


//[BurstCompile]
public struct WindJob : IJob
{

    [ReadOnly] public NativeArray<float2> inPosition;
    [ReadOnly] public NativeArray<float2> inVelocities;

    [ReadOnly] public float2 noisePosition;
    [ReadOnly] public float2 noiseOffset;
    [ReadOnly] public float2 noiseScale;

    [ReadOnly] public float windIntensity;
    [ReadOnly] public float windDrag;
    [ReadOnly] public float precision;
    [ReadOnly] public float deltaTime;

    [WriteOnly] public NativeList<float2> outVelocities;

    public void Execute()
    {
        for (int i = 0; i < inPosition.Length; i++)
        {
            //Wind generation
            //Get the dervative of each point, and then uses "curl" to get wind like effet

            //Derviative
            float2 pos = inPosition[i] * noiseScale + noisePosition;
            float2 velocity = inVelocities[i];
            float2 normalisedVelocity = math.normalizesafe(velocity);
            float x1 = IcariaNoise.GradientNoise(pos.x + noiseOffset.x + precision, pos.y);
            float x2 = IcariaNoise.GradientNoise(pos.x + noiseOffset.x - precision, pos.y);

            float y1 = IcariaNoise.GradientNoise(pos.x, pos.y + noiseOffset.y + precision);
            float y2 = IcariaNoise.GradientNoise(pos.x, pos.y + noiseOffset.y - precision);

            //Curl
            float dirX = (y1 - y2) / (2 * precision);
            float dirY = (x2 - x1) / (2 * precision);
            float2 windDirection = math.normalizesafe(new float2(dirX, dirY));

            //Drag + Wind
            
            velocity = velocity - (normalisedVelocity * windDrag) * deltaTime;
            velocity = velocity + (windDirection * windIntensity) * deltaTime;

            outVelocities[i] = (velocity);
        }
    }
}
