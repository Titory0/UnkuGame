using UnityEngine;

public class HittedState : EnemyState
{
    public EnemyStatePool hitEndState;
    public string requiredAnimationName;

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if(stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName(requiredAnimationName))
        {
            if (hitEndState.entries.Count == 0)return;

            stateMachine.SwitchState(hitEndState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void OnHit()
    {
        base.OnHit();
    }
}
