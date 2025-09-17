using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyStateMachine : MonoBehaviour
{
    public EnemyState currentState;
    public EnemyState deadState;

    [Header("References")]
    public Health health;
    public ObjectFlipper objectFlipper;
    public PlayerDetector visionDetector;
    public Transform limitBox;
    public string hitTriggerName = "Hit";


    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Animator animator;

    [SerializeField]
    private bool debug;

    #region Initialization

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (health != null)
        {
            health.onHit.AddListener(EnemyHit);
            health.onDeath.AddListener(EnemyDie);
        }
        else
        {
            Debug.LogError("Immortal Enemy : " + gameObject.name);
        }
    }

    private void Start()
    {
        player = PlayerState.Instance.gameObject;

    }

    #endregion

    private void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }

    public void SwitchState(EnemyState newState, bool shouldFlip = true)
    {
        if (objectFlipper != null && shouldFlip)
        {
            objectFlipper.FaceDirection(player.transform.position.x - transform.position.x);
        }

        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public void EnemyHit()
    {
        currentState.OnHit();
    }

    public void EnemyDie()
    {
        if (deadState != null)
        {
            SwitchState(deadState,false);
        }
        else
        {
            Debug.LogWarning("Dead state is not set for " + gameObject.name);
        }
    }
}
