using UnityEngine;

public class AttackAnimationBehaviour : StateMachineBehaviour
{
    public int attackIndex;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("attackIndex", attackIndex);
        animator.SetBool("isAttacking", true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAttacking", false);
    }
}
