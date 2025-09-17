using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(PlayerAttack))]
public class PlayerAttackFeedback : MonoBehaviour
{

    [Header("VFX")]
    public List<VisualEffect> attackEffects;
    public string vfxHitDirectionParameter = "HitDirection";
    public string playerPositionParameter = "PlayerPosition";
    public ParticleOrder hitExperienceVFX;


    [Header("Animation")]
    public Animator animator;
    public float animationFreezeTime;



    [Header("TimeFreeze")]
    public float timeFreezeDuration;
    public bool timePlaying;



    [Header("CameraShake")]
    public float shakeForce;
    public float shakeIntensity;
    public float shakeDuration;
    public AnimationCurve shakeCurve;



    [Header("References")]
    public PlayerDirection playerFlipper;


    void Start()
    {
        GetComponent<PlayerAttack>().onHitEnemy.AddListener(PlayAttackFeedback);
    }

    void PlayAttackFeedback(Vector2 hitPosition, Health enemy)
    {
        PlayVFX(hitPosition, enemy);
        PlayAnimationStop();
        PlayEnemyAnimationStop(enemy);
        StartCameraShake();
        TimeStop();

    }

    #region Vfx
    void PlayVFX(Vector2 hitPosition, Health enemyHealth)
    {
        Vector3 hitDirection = transform.right * (playerFlipper.isFacingRight ? 1 : -1);
        enemyHealth.TryGetComponent<ObjectCenter>(out ObjectCenter enemyCenter);

        if (enemyCenter != null)
        {
            hitDirection = enemyCenter.centerPosition - hitPosition;
        }

        Vector3 playerPosition = PlayerState.Instance.transform.position;

        //Experience Burst System
        hitExperienceVFX.position = hitPosition;
        hitExperienceVFX.velocity = hitDirection.normalized;
        ExperienceSystem.Instance.SpawnParticles(hitExperienceVFX);


        //VFX
        foreach (var attackEffect in attackEffects)
        {
            if (attackEffect.HasVector2(vfxHitDirectionParameter))//Checks hit direction
            {
                attackEffect.SetVector2(vfxHitDirectionParameter, hitDirection.normalized);
            }
            if (attackEffect.HasVector3(playerPositionParameter))//Checks player position
            {
                attackEffect.SetVector3(playerPositionParameter, playerPosition);
            }


            attackEffect.transform.position = hitPosition;
            attackEffect.Play();
        }
    }
    #endregion

    #region Animation

    void PlayAnimationStop()
    {
        if (animator != null)
        {
            StartCoroutine(AnimationStopCoroutine());
        }
    }

    IEnumerator AnimationStopCoroutine()
    {
        animator.speed = 0;
        yield return new WaitForSeconds(animationFreezeTime);
        animator.speed = 1;

        animator.Update(timeFreezeDuration);

    }

    void PlayEnemyAnimationStop(Health enemyHealth)
    {
        enemyHealth.TryGetComponent<Animator>(out Animator enemyAnimator);
        if ((enemyAnimator != null))
        {
            StartCoroutine(EnemyAnimationStopCoroutine(enemyAnimator));
        }
    }

    IEnumerator EnemyAnimationStopCoroutine(Animator enemyAnimator)
    {
        enemyAnimator.speed = 0;
        yield return new WaitForSeconds(animationFreezeTime);
        enemyAnimator.speed = 1;
    }

    #endregion

    #region Time
    void TimeStop()
    {
        if (timeFreezeDuration > 0)
        {
            StartCoroutine(TimeStopCoroutine());
        }
    }
    IEnumerator TimeStopCoroutine()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(timeFreezeDuration);
        Time.timeScale = 1;
    }
    #endregion

    #region CameraShake

    void StartCameraShake()
    {
        CameraShake.Instance.AddShake(shakeDuration, shakeForce, shakeIntensity, shakeCurve);
    }

    #endregion
}
