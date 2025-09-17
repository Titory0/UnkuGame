using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyStateMachine))]
public class EnemyState : MonoBehaviour
{
    [Tooltip("Name of the state, used for debugging purposes.")]
    public string stateName = "DefaultState";

    [HideInInspector]
    public float runningTime = 0f;
    [HideInInspector]
    public EnemyStateMachine stateMachine;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Animator animator;

    #region Initialization

    public virtual void Awake()
    {
        stateMachine = GetComponent<EnemyStateMachine>();
        
    }
    private void Start()
    {
        player = PlayerState.Instance.gameObject;
        animator = stateMachine.animator;
    }

    #endregion

    public virtual void EnterState()
    {
    }
    public virtual void UpdateState()
    {
        runningTime += Time.deltaTime;
    }
    public virtual void ExitState()
    {
        runningTime = 0f;
    }

    public virtual void OnHit()
    {
        print("hit");
        stateMachine.animator.SetTrigger(stateMachine.hitTriggerName);
    }
}
