using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public Vector2 movementInput { get; private set; }
    bool jumpInput; //Act as a trigger
    bool jumpHeld;  //Act a a state


    [Header("Horizontal Movement")]
    public float maxSpeed;
    public float acceleration = 10f;
    public float drag;


    [Header("Jumping")]
    public float jumpHeight;
    public float timeToPeak;
    public float timeToGround;
    public float gravitySwitchTime;
    public float maxFallSpeed;
    [Range(0f, 1f)] public float jumpStopFactor;


    float jumpVelocity;
    float jumpGravity;
    float fallingGravity;
    float currentGravityValue;
    float currentJumpTime;


    [Header("Wall Jumping")]
    public float wallJumpSpeedY;
    public float wallJumpSpeedX;
    public float wallJumpDuration;
    public float wallJumpBufferTime;

    public float wallSlideGravity;
    public float wallSlideMaxSpeed;
    public float wallGrabDuration;

    float lastWallJumpTime;
    float lastWallGrabTime;
    bool justWallJumped;
    bool hasWallGrabed;


    [Header("Visualisation Settings")]

    public bool showJumpSimulation = true;
    public int resolution = 10;
    public float timeScale;


    [Header("References")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    Rigidbody2D rb;
    PlayerState playerState;


    #region UnityBasic

    void Start()
    {
        LinkInputs();
        rb = GetComponent<Rigidbody2D>();
        playerState = PlayerState.Instance;
        rb.gravityScale = fallingGravity;
    }

    void Update()
    {

        SetJumpConstants();
        //SetJumpConstant can be called in start for optimisation but will stay here until tuning is done

        if (PlayerState.Instance.isDashing) { return; }

        Move();
        Drag();


        TryWallGrab();
        TryJump();

        UpdateJump();
        UpdateGravityScale();
        UpdateWallGrab();

        ClampSpeed();
    }

    #endregion

    #region Horizontal Movement

    public void Move()
    {
        if (Time.time - lastWallJumpTime < wallJumpDuration) { return; }

        rb.linearVelocityX += (movementInput.x * acceleration * Time.deltaTime);
        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);
    }

    public void Drag()
    {
        if (movementInput.x != 0) { return; }

        if (Time.time - lastWallJumpTime < wallJumpDuration) { return; }

        float dragForce = drag * Time.deltaTime;

        if (Mathf.Abs(rb.linearVelocityX) < dragForce)
        {
            rb.linearVelocityX = 0f;
            return;
        }

        rb.linearVelocityX = rb.linearVelocityX > 0 ?
                                        rb.linearVelocityX - dragForce :
                                        rb.linearVelocityX + dragForce;
    }

    #endregion

    #region Jumping

    public void TryJump()
    {
        if (jumpInput == false) { return; }

        jumpInput = false;

        if (playerState.isGrounded)
        {
            rb.linearVelocityY = jumpVelocity;
            currentJumpTime = 0f;
            playerState.Jump();
            return;
        }

        bool canWallJump = playerState.isOnWall ||
                            ((Time.time - playerState.lastTimeOnWall < wallJumpBufferTime) && justWallJumped == false);

        if (canWallJump && playerState.isGrounded == false)
        {
            rb.linearVelocityX = -Mathf.Sign(playerState.wallSide) * wallJumpSpeedX;
            rb.linearVelocityY = wallJumpSpeedY;

            lastWallJumpTime = Time.time;
            justWallJumped = true;
            currentJumpTime = 0f;
            return;
        }
    }


    public void UpdateJump()
    {
        if (playerState.isOnWall)
        {
            justWallJumped = false;
        }

        if (!playerState.isGrounded)
        {
            currentJumpTime += Time.deltaTime;
            if (jumpHeld || currentJumpTime >= timeToPeak) { return; }

            rb.linearVelocityY = rb.linearVelocityY * jumpStopFactor;
            currentJumpTime = timeToPeak;
        }
        else
        {
            justWallJumped = false;
        }
    }

    public void UpdateGravityScale()
    {
        currentGravityValue += Mathf.Sign(rb.linearVelocityY) * Time.deltaTime / gravitySwitchTime;
        currentGravityValue = Mathf.Clamp01(currentGravityValue);

        float airGravity = Mathf.Lerp(fallingGravity, jumpGravity, currentGravityValue);
        float wallGravity = playerState.isGrabbingWall ? 0f : wallSlideGravity;
        bool isOnWall = playerState.isOnWall && rb.linearVelocityY <= 0f;
        rb.gravityScale = isOnWall ? wallGravity : airGravity;
    }

    public void SetJumpConstants()
    {
        //Conversion in unity units
        float convertedJumpHeight = jumpHeight * 10f;
        float convertedTimeToPeak = timeToPeak * 10f;
        float convertedTimeToGround = timeToGround * 10f;

        jumpVelocity = (2 * convertedJumpHeight) / convertedTimeToPeak;
        jumpGravity = (2 * convertedJumpHeight) / (convertedTimeToPeak * convertedTimeToPeak);
        fallingGravity = (2 * convertedJumpHeight) / (convertedTimeToGround * convertedTimeToGround);
    }

    #endregion


    public void ClampSpeed()
    {
        float maxDownFallSpeed = playerState.isOnWall ? wallSlideMaxSpeed : maxFallSpeed;
        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxDownFallSpeed, float.MaxValue);
    }

    #region Wall Grab

    public void TryWallGrab()
    {
        if (playerState.isOnWall == false || hasWallGrabed) { return; }

        //Grabs wall when falling on it
        if (rb.linearVelocityY <= 0)
        {
            rb.linearVelocityY = 0;
            rb.gravityScale = 0;

            playerState.isGrabbingWall = true;
            hasWallGrabed = true;
            lastWallGrabTime = Time.time;
        }
    }


    public void UpdateWallGrab()
    {
        playerState.isGrabbingWall = lastWallGrabTime + wallGrabDuration > Time.time;

        if (playerState.isOnWall == false || hasWallGrabed == false)
        {
            playerState.isGrabbingWall = false;
            hasWallGrabed = false;
            return;
        }

        //Reset the wall stop when jumping or moving up
        if (rb.linearVelocityY > 0)
        {
            hasWallGrabed = false;
            return;
        }
    }

    #endregion


    #region Input


    public void LinkInputs()
    {
        PlayerInputManager inputManager = GetComponent<PlayerInputManager>();
        if (inputManager != null)
        {
            inputManager.onMove.AddListener(GetMovementInput);
            inputManager.onJump.AddListener(GetJumpInput);
        }
        else
        {
            Debug.LogWarning("PlayerInputManager not found");
        }
    }


    public void GetMovementInput(Vector2 direction)
    {
        movementInput = direction;
    }
    public void GetJumpInput(bool input)
    {
        jumpInput = input;
        jumpHeld = input;
    }

    #endregion

    private void OnDrawGizmos()
    {

        #region JumpSimulation

        if (!showJumpSimulation) { return; }

        List<Vector2> points = new List<Vector2>();
        List<Vector2> velocities = new List<Vector2>();
        Vector2 baseVelocity = new Vector2(maxSpeed, jumpVelocity);
        Vector2 position = transform.position;

        if (!Application.isPlaying)
        {
            SetJumpConstants();
        }
        Gizmos.color = Color.red;

        for (int i = 0; i < resolution; i++)
        {
            Gizmos.DrawSphere(position, 0.1f);
            position += baseVelocity * timeScale;
            baseVelocity.y -= baseVelocity.y > 0 ? jumpGravity * 10f * timeScale : fallingGravity * 10f * timeScale;
        }

        #endregion

    }
}
