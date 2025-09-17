using UnityEngine;

public class AttackState : EnemyState
{
    public EnemyStatePool hittedState;
    public EnemyStatePool attackFinishedState;

    public string animatorAttackTrigger = "Attack";
    public string animatorStateName = "AttackState";
    bool animationStarted = false;
    public bool hasBeenHit = false;


    public override void EnterState()
    {
        base.EnterState();
        stateMachine.animator.SetTrigger(animatorAttackTrigger);
        animationStarted = false;
        hasBeenHit = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (animationStarted == false && stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStateName))
        {
            animationStarted = true;
        }

        if(animationStarted && !stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStateName))
        {
            if (hasBeenHit) return;

            if (attackFinishedState.entries.Count != 0)
            {
                stateMachine.SwitchState(attackFinishedState);
            }
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        animationStarted = false;
        hasBeenHit = false;
    }

    public override void OnHit()
    {
        base.OnHit();
        hasBeenHit = true;
        if (hittedState.entries.Count == 0) { return; } 
        stateMachine.SwitchState(hittedState);
    }
}
