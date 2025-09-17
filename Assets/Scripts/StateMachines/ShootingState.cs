using UnityEngine;

public class ShootingState : EnemyState
{
    [SerializeField]
    int minShots = 1;
    [SerializeField]
    int maxShots = 3;
    int plannedShotsCount;
    int currentShotCount;

    [SerializeField]
    [Tooltip("Both values should be higher than animation time in order to work properly")]
    Vector2 timeRangeBeetweenShots;
    float currentShootTime;
    float lastShotTime;


    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    Transform projectileSpawnPoint;

    [SerializeField]
    string animatorTriggerName;
    [SerializeField]
    string idleStateName;

    [SerializeField]
    EnemyState shootEndState;


    public override void EnterState()
    {
        base.EnterState();
        plannedShotsCount = Random.Range(minShots, maxShots + 1);
        //Need to start the shooting animation
    }
    public override void UpdateState()
    {
        base.UpdateState();


        float currentAnimationTime = lastShotTime - Time.time;

        if(currentShotCount < plannedShotsCount && currentAnimationTime < currentShootTime)
        {
            currentShotCount++;
            lastShotTime = Time.time;
            currentShootTime = Random.Range(timeRangeBeetweenShots.x, timeRangeBeetweenShots.y);

            stateMachine.animator.SetTrigger(animatorTriggerName);
        }

    }

    public override void ExitState()
    {
        base.ExitState();
        currentShotCount = 0;
        plannedShotsCount = 0;
    }


    //This function should be called from an animation event
    public void Shoot()
    {
        Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
    }
}
