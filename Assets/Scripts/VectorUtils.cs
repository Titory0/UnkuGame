using UnityEngine;

public class VectorUtils : MonoBehaviour
{
    public static Vector2 GetRandomDirection(Vector2 baseDirection, float spread)
    {

        if (baseDirection.sqrMagnitude < 1e-6f)
            return Random.insideUnitCircle.normalized;

        spread = Mathf.Clamp01(spread);

        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
        float maxDeviation = 180f * spread;
        float randomDeviation = Random.Range(-maxDeviation, maxDeviation);
        float finalAngle = baseAngle + randomDeviation;
        float rad = finalAngle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }
}
