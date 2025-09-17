using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Infos
{
    public bool goingRight;
    public bool isWaiting;
}

public class WonderingState : EnemyState
{
    public float walkingSpeed;  

    public Vector2 wanderingLimits;
    public float stoppingDistance;

    public float waitTime;
    public float waitTimeRange;

    private bool goingRight;
    private bool isWaiting;
    public Infos infos;

    public EnemyState hittedState;
    public EnemyState playerDetectedState;

    private void Start()
    {
        stateMachine.visionDetector.onPlayerDetected.AddListener(DetectPlayer);
    }


    public override void EnterState()
    {
        base.EnterState();

    }
    public override void UpdateState()
    {
        base.UpdateState();

        if(!isWaiting)
        {
            Move();
        }
    }
    public override void ExitState()
    {
        base.ExitState();
    }

    public override void OnHit()
    {
        base.OnHit();
        stateMachine.SwitchState(hittedState);
    }

    void DetectPlayer()
    {
        stateMachine.SwitchState(playerDetectedState);
    }
    void Move()
    {
        if(isWaiting){return;}
        
        float currentLimit = goingRight? wanderingLimits.y : wanderingLimits.x;
        float remainingDistance = transform.position.x - currentLimit;

        if(Mathf.Abs(remainingDistance) < stoppingDistance)
        {
            print(remainingDistance + " going the other way and waiting");
            goingRight = !goingRight;
            StartCoroutine(Wait(GetStoppingTime()));
            return;
        }

        stateMachine.rb.linearVelocity = new Vector2(walkingSpeed * GetDirection(), stateMachine.rb.linearVelocityY);
    }

    IEnumerator Wait(float waitTime)
    {
        stateMachine.rb.linearVelocity = Vector2.zero;
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
    }




    #region Utils

    int GetDirection()
    {
        return goingRight?1:-1;
    }
    float GetStoppingTime()
    {
        return waitTime + UnityEngine.Random.Range(-waitTimeRange, waitTimeRange);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(wanderingLimits.x, transform.position.y), new Vector2(wanderingLimits.y, transform.position.y));
        Gizmos.DrawWireSphere(new Vector2(wanderingLimits.x, transform.position.y), stoppingDistance);
        Gizmos.DrawWireSphere(new Vector2(wanderingLimits.y, transform.position.y), stoppingDistance);
    }

}
