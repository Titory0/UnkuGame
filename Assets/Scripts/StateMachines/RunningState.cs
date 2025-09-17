using UnityEngine;

public class RunningState : EnemyState
{
    public EnemyStatePool hittedState;
    public EnemyStatePool playerReachedState;
    public EnemyStatePool playerOutOfRangeState;
    public EnemyStatePool playerBehindState;

    public string runningParameterName;

    public float runningSpeed = 2f;
    public float playerReachedDistance = 0.5f;
    public float playerOutOfRangeDistance = 3f; // Distance at which the player is considered out of range

    float runningDirection; // 1 for right, -1 for left

    public override void EnterState()
    {
        base.EnterState();
        stateMachine.animator.SetBool(runningParameterName, true);
        runningDirection = Mathf.Sign(stateMachine.player.transform.position.x - transform.position.x);
    }
    public override void UpdateState()
    {
        base.UpdateState();
        stateMachine.rb.linearVelocityX = runningSpeed * runningDirection;

        if (IsPlayerReached())
        {
            stateMachine.SwitchState(playerReachedState); return;
        }
        if (IsPlayerOutside())
        {
            stateMachine.SwitchState(playerOutOfRangeState); return;
        }
        if (isPlayerBehind())
        {
            stateMachine.SwitchState(playerBehindState); return;
        }

    }
    public override void ExitState()
    {
        base.ExitState();
        stateMachine.rb.linearVelocityX = 0f;
        stateMachine.animator.SetBool(runningParameterName, false);
    }

    public override void OnHit()
    {
        base.OnHit();
        if (hittedState == null) { return; }
        stateMachine.SwitchState(hittedState);
    }

    public bool IsPlayerReached()
    {
        float playerDistance = (stateMachine.player.transform.position.x - transform.position.x) * runningDirection;
       
        if (playerDistance <= playerReachedDistance && playerDistance >= 0f)
        {
            return true;
        }
        return false;
    }

    public bool IsPlayerOutside()
    {
        Vector2 playerPosition = stateMachine.player.transform.position;

        if (playerPosition.x < stateMachine.limitBox.position.x - (stateMachine.limitBox.localScale.x / 2f) - playerOutOfRangeDistance ||
            playerPosition.x > stateMachine.limitBox.position.x + (stateMachine.limitBox.localScale.x / 2f) + playerOutOfRangeDistance)
        {
            return true;
        }
        return false;
    }

    public bool isPlayerBehind()
    {
        float playerDistance = (stateMachine.player.transform.position.x - transform.position.x);

        if (Mathf.Sign(playerDistance) != runningDirection)
        {
            return true;
        }

        return false;
    }
}
