using System.Collections.Generic;
using UnityEngine;
public class ObjectFlipper : MonoBehaviour
{
    public List<Transform> flippedObjects;
    public List<SpriteRenderer> flippedSprites;

    public bool isFacingRight;

    public void FaceDirection(float direction)
    {
        if (Mathf.Sign(direction) == (isFacingRight? 1f : -1f)) { return; }

        isFacingRight = !isFacingRight;

        for (int i = 0; i < flippedObjects.Count; i++)
        {
            flippedObjects[i].localPosition = new Vector3(
                                                flippedObjects[i].localPosition.x * -1,
                                                flippedObjects[i].localPosition.y,
                                                flippedObjects[i].localPosition.z
                                                );
        }
        for (int i = 0; i < flippedSprites.Count; i++)
        {
            flippedSprites[i].flipX = !flippedSprites[i].flipX;
        }
    }
}
