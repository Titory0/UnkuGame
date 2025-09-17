using UnityEngine;

public class MindlessWaitState : EnemyState
{
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    float chosenWaitTime;   
    float currentWaitTime = 0f;

    public EnemyStatePool nextState;
    public override void EnterState()
    {
        base.EnterState(); 
        chosenWaitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        currentWaitTime += Time.fixedDeltaTime;
        if(currentWaitTime >= chosenWaitTime)
        {
            if(nextState.entries.Count == 0) { return; }
            stateMachine.SwitchState(nextState);
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

