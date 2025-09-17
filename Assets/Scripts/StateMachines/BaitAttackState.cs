using UnityEngine;

public class BaitAttackState : EnemyState
{
    public EnemyStatePool hittedState;
    public EnemyStatePool baitedState;
    public EnemyStatePool waitTimeEnded;

    [Tooltip("Max time baiting, 0 for infinite")]
    public float baitTime = 1f; 
    float currentBaitTime = 0f;

    public PlayerDetector baitZone;

    private void Start()
    {
        baitZone.onPlayerDetected.AddListener(OnPlayerBaited);
    }

    public override void EnterState()
    {
        base.EnterState();
        currentBaitTime = 0f;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        currentBaitTime += Time.fixedDeltaTime;
        
        if(baitTime > 0f && currentBaitTime >= baitTime)
        {
            if (waitTimeEnded != null){stateMachine.SwitchState(waitTimeEnded);}return;
        }


    }

    public override void ExitState()
    {
        base.ExitState();
        currentBaitTime = 0f;
    }

    public override void OnHit()
    {
        base.OnHit();
        if (hittedState == null) { return; }
        stateMachine.SwitchState(hittedState);
    }

    public void OnPlayerBaited()
    {
        if (baitedState == null) { return; }
        stateMachine.SwitchState(baitedState);
    }
}
