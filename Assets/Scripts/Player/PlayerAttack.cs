using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerAttack : MonoBehaviour
{

    #region Variables

    [Header("Parameters")]
    public float attackDamage;
    public float downAttackDamage;
    public float downAttackUpForce;

    public float unkuAttackBuffer;
    public float attackComboBuffer;
    float currentComboBuffer;
    float lastUnkuTrigger;

    [Range(0f, 1f)] public float directionInputThreshold = 0.5f;





    [Header("States")]
    public bool unkuModeActive;
    public bool isDownAttacking;
    public bool isUpAttacking;

    Vector2 playerDirection;
    List<Collider2D> downAttackedEnemies = new List<Collider2D>();


    [Header("Animation")]
    public AnimatorAttackTriggers animatorAttackTriggers;



    [Header("References")]
    public LayerMask enemyLayer;
    public Transform attackHitBox;

    public Rigidbody2D rb;
    public Animator animator;
    public Transform punchTransform;
    public Transform downAttackTransform;

    public TriggerEvent downAttackTrigger;
    public TriggerEvent upAttackTrigger;

    public PlayerState playerState;



    [Header("Events")]
    public UnityEvent<Vector2, Health> onHitEnemy;

    #endregion

    #region Main

    void Start()
    {
        PlayerInputManager inputs = GetComponent<PlayerInputManager>();
        if (inputs == null) throw new Exception("No PlayerInputManager found");

        inputs.onAttack.AddListener(AttackInput);

        inputs.onUnkuMode.AddListener(GetUnku);
        inputs.onMove.AddListener(GetMovement);

        downAttackTrigger.triggerEvent.AddListener(HitDownAttack);
        upAttackTrigger.triggerEvent.AddListener(HitUpAttack);
    }

    void Update()
    {
        if (animator.GetBool("isAttacking") == false)
        {
            currentComboBuffer += Time.deltaTime;
        }
        else
        {
            currentComboBuffer = 0f;
        }

        if (currentComboBuffer >= attackComboBuffer)
        {
            animator.SetInteger("attackIndex", 0);
        }

    }


    void AttackInput()
    {
        ResetAnimationTrigger();
        if (CanUnkuAttack())
        {
            UnkuAttack();
        }
        else
        {
            BobAttack();
        }
    }


    void BobAttack()
    {
        //Checks directional attacks
        if (playerDirection.y > directionInputThreshold)
        {
            animator.SetTrigger(animatorAttackTriggers.upAttackTrigger);
            return;
        }
        if (playerDirection.y < -directionInputThreshold && !playerState.isGrounded)
        {
            animator.SetTrigger(animatorAttackTriggers.downAttackTrigger);
            return;
        }

        //Else do normal attack
        animator.SetTrigger(animatorAttackTriggers.bobAttackTrigger);
    }


    bool CanUnkuAttack()
    {
        return Time.time - lastUnkuTrigger < unkuAttackBuffer;
    }

    void UnkuAttack()
    {
        animator.SetTrigger(animatorAttackTriggers.unkuAttackTrigger);
    }

    public void UnkuAttackTrigger()
    {
        lastUnkuTrigger = Time.time;
    }


    public void ResetAnimationTrigger()
    {
        animator.ResetTrigger(animatorAttackTriggers.bobAttackTrigger);
        animator.ResetTrigger(animatorAttackTriggers.unkuAttackTrigger);
        animator.ResetTrigger(animatorAttackTriggers.upAttackTrigger);
        animator.ResetTrigger(animatorAttackTriggers.downAttackTrigger);
    }


    void StartDownAttack()
    {
        downAttackedEnemies.Clear();
    }

    void EndDownAttack()
    {
        animator.SetBool("downAttack", false);
    }

    #endregion


    #region HittingLogic

    void HitDownAttack(Collider2D collider)
    {

        if (!isDownAttacking || downAttackedEnemies.Contains(collider)) return;

        Health enemyHealth = collider.transform.GetComponentInParent<Health>();

        if (enemyHealth == null || enemyHealth.isDead) { return; }

        enemyHealth.Hit(downAttackDamage);
        onHitEnemy?.Invoke(collider.ClosestPoint(downAttackTransform.position), enemyHealth);

        downAttackedEnemies.Add(collider);
        animator.SetBool("downAttack", false);
        rb.linearVelocityY = downAttackUpForce;
    }


    void HitUpAttack(Collider2D collider)
    {
        if (!isUpAttacking) return;

        Health enemyHealth = collider.GetComponent<Health>();

        if (enemyHealth == null || enemyHealth.isDead) { return; }

        enemyHealth.Hit(downAttackDamage);
        onHitEnemy?.Invoke(collider.ClosestPoint(downAttackTransform.position), enemyHealth);
    }


    void HitEnemies()
    {

        Collider2D[] enemiesInRange = Physics2D.OverlapBoxAll(attackHitBox.position, attackHitBox.lossyScale, 0f, enemyLayer);

        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            if (enemiesInRange[i].CompareTag("Hittable") == false) { continue; }

            Health enemyHealth = enemiesInRange[i].GetComponentInParent<Health>();

            if (enemyHealth == null || enemyHealth.isDead) { continue; }

            enemyHealth.Hit(attackDamage);
            onHitEnemy?.Invoke(enemiesInRange[i].ClosestPoint(punchTransform.position), enemyHealth);
        }
    }

    #endregion


    #region Input Listeners

    public void GetMovement(Vector2 direction)
    {
        playerDirection = direction;
    }


    public void GetUnku(bool isActive)
    {
        unkuModeActive = isActive;
    }

    #endregion

}

#region InspectorTweaks

[Serializable]
public struct AnimatorAttackTriggers
{
    public string bobAttackTrigger;
    public string unkuAttackTrigger;
    public string upAttackTrigger;
    public string downAttackTrigger;
}

#endregion
