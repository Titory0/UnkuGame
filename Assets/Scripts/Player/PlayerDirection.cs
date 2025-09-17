using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerDirection : MonoBehaviour
{
    PlayerMovement playerMovement;
    public bool isFacingRight = true;
    bool wasFacingRight = true;

    public List<Transform> flippedObjects;
    public List<SpriteRenderer> flippedSprites;
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement.movementInput.x == 0) { return; }

        isFacingRight = playerMovement.movementInput.x > 0;

        if (isFacingRight == wasFacingRight) return;

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

        wasFacingRight = isFacingRight;
    }
}
