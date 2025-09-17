using System;
using System.Collections;
using UnityEngine;


public class WaitPlayerState: EnemyState
{
    public EnemyStatePool hittedState;
    public EnemyStatePool playerDetectedState;
    public EnemyStatePool waitTimeEnded;

    [Tooltip("Time to wait before switching to the next state. Set to 0 for infinite")]
    public float waitTime = 0f;
    float currentWaitTime = 0f; 

    private void Start()
    {
        stateMachine.visionDetector.onPlayerDetected.AddListener(DetectPlayer);
    }


    public override void EnterState()
    {
        base.EnterState();
        currentWaitTime = 0f;
    }
    public override void UpdateState()
    {
        base.UpdateState();
        if(waitTime > 0f && currentWaitTime >= waitTime)
        {
            stateMachine.SwitchState(playerDetectedState);
            return;
        }
        currentWaitTime += Time.fixedDeltaTime;
    }
    public override void ExitState()
    {
        base.ExitState();
        currentWaitTime = 0f;
    }

    public override void OnHit()
    {
        base.OnHit();
        if(hittedState.entries.Count == 0) {return;}
        stateMachine.SwitchState(hittedState);
    }

    void DetectPlayer()
    {
        if (playerDetectedState.entries.Count == 0)
        {
            return; }
        stateMachine.SwitchState(playerDetectedState);
    } 



    private void OnDrawGizmosSelected()
    {
    }

}
