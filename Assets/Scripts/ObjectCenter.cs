using UnityEngine;

public class ObjectCenter : MonoBehaviour
{
    public Transform center;
    public Vector2 centerPosition;

    private void Update()
    {
        centerPosition = center.position;
    }
}
