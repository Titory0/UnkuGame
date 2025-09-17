using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
public class PlayerState : MonoBehaviour
{

    #region Singleton
    public static PlayerState Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of PlayerState detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion

    [Header("Basics")]
    public bool isGrounded;

    public bool isDashing;
    public bool canDash;

    public int lastMovementInputX;

    bool wasGrounded;

    [Header("Wall States")]
    public bool isOnWall;
    public int wallSide; //1 means right wall, -1 means left wall, 0 means no wall
    public float lastTimeOnWall;
    public bool isGrabbingWall;




    [Header("References")]
    public PlayerMovement playerMovement;
    public Rigidbody2D rb;

    public Transform rightWallCheck;
    public Transform leftWallCheck;
    public LayerMask wallLayer;

    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Events")]
    public UnityEvent hitGroundEvent;

    private void Update()
    {
        //Set variables
        wasGrounded = isGrounded;

        int xInput = Mathf.RoundToInt(playerMovement.movementInput.x);
        if (xInput != 0)
        {
            lastMovementInputX = xInput;
        }

        if (canDash == false && (isGrounded || isOnWall))
        {
            canDash = true;
        }

        //Check collisions
        CheckGround();
        CheckWalls();

        //Handle events
        if (wasGrounded == false && isGrounded == true)
        {
            hitGroundEvent?.Invoke();
        }
    }


    public void CheckGround()
    {
        Vector2 position = groundCheck.position;
        Vector2 size = groundCheck.lossyScale;
        float angle = 0f;

        Collider2D[] collisions = Physics2D.OverlapBoxAll(position, size, angle, groundLayer);

        for (int i = 0; i < collisions.Length; i++)
        {
            if (collisions[i].tag == "Player" || collisions[i].isTrigger) { continue; }

            isGrounded = true;
            return;
        }
        isGrounded = false;
    }


    public void CheckWalls()
    {
        Vector2 leftPosition = leftWallCheck.position;
        Vector2 leftSize = leftWallCheck.lossyScale;
        Vector2 rightPosition = rightWallCheck.position;
        Vector2 rightSize = rightWallCheck.lossyScale;
        float angle = 0f;

        Collider2D[] leftCollisions = Physics2D.OverlapBoxAll(leftPosition, leftSize, angle, wallLayer);
        Collider2D[] rightCollisions = Physics2D.OverlapBoxAll(rightPosition, rightSize, angle, wallLayer);

        bool isWallLeft = CheckCollision(leftPosition, leftSize, wallLayer);
        bool isWallRight = CheckCollision(rightPosition, rightSize, wallLayer);

        //Checks left wall
        if ((playerMovement.movementInput.x == -1 || lastMovementInputX == -1) && isWallLeft) //-1 means holding left
        {
            isOnWall = true;
            lastTimeOnWall = Time.time;
            wallSide = -1;
            return;
        }
        //Checks right wall
        if ((playerMovement.movementInput.x == 1 || lastMovementInputX == 1) && isWallRight) //-1 means holding left
        {
            isOnWall = true;
            lastTimeOnWall = Time.time;
            wallSide = 1;
            return;
        }

        //If no wall
        isOnWall = false;
    }


    public bool CheckCollision(Vector2 position, Vector2 size, LayerMask layer)
    {
        float angle = 0f;
        Collider2D[] collisions = Physics2D.OverlapBoxAll(position, size, angle, layer);
        for (int i = 0; i < collisions.Length; i++)
        {
            if (collisions[i].tag == "Player" || collisions[i].isTrigger) { continue; }
            return true;
        }
        return false;
    }


    public void Jump()
    {
        isGrounded = false;
    }


    public void StartDash()
    {
        isDashing = true;
        canDash = false;
    }


    public void StopDash()
    {
        isDashing = false;
    }
}
