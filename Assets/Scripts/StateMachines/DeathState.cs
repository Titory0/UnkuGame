using UnityEngine;

public class DeathState : EnemyState
{
    public string animatorDeathBool = "isDead";
    public override void EnterState()
    {
        base.EnterState();
        stateMachine.animator.SetBool(animatorDeathBool,true);
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void OnHit()
    {
    }
}
