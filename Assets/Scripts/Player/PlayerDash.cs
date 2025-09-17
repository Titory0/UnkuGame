using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Parameters")]
    public AnimationCurve dashSpeedCurve;
    public float dashGlobalSpeed;
    public float dashTime;
    public float dashBufferDuration;

    bool isInputBuffered;
    float lastInputTime;

    Coroutine dashCoroutine;

    [Header("References")]
    Rigidbody2D rb;
    PlayerMovement playerMovement;
    PlayerState playerState;


    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerState = PlayerState.Instance;
        rb = GetComponent<Rigidbody2D>();
        LinkInput();
    }

    void Update()
    {
        if (isInputBuffered && (Time.time - lastInputTime) <= dashBufferDuration)
        {
            Dash();
        }
        else
        {
            isInputBuffered = false;
        }
    }

    public void Dash()
    {
        if (playerState.canDash)
        {
            dashCoroutine = StartCoroutine(DashCoroutine(Mathf.Sign(playerState.lastMovementInputX)));
            isInputBuffered = false;
        }
        else
        {
            if (!isInputBuffered)
            {
                lastInputTime = Time.time;
            }
            isInputBuffered = true;
        }
    }

    public void CancelDash()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
            PlayerState.Instance.StopDash();
        }
    }

    public IEnumerator DashCoroutine(float direction)
    {
        PlayerState.Instance.StartDash();
        float elapsedTime = 0f;

        rb.linearVelocityY = 0f;
        rb.linearVelocityX = direction * dashGlobalSpeed;

        while (elapsedTime < dashTime)
        {
            float speedFactor = dashSpeedCurve.Evaluate(elapsedTime / dashTime);

            rb.linearVelocityY = 0f;
            rb.linearVelocityX = direction * dashGlobalSpeed * speedFactor;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        PlayerState.Instance.StopDash();
    }


    public void LinkInput()
    {
        PlayerInputManager inputManager = GetComponent<PlayerInputManager>();
        if (inputManager != null)
        {
            inputManager.onDash.AddListener(Dash);
        }
        else
        {
            Debug.LogWarning("PlayerInputManager not found");
        }
    }
}
