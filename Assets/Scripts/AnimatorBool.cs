using UnityEngine;

public class AnimatorBool : StateMachineBehaviour
{
    [Header("Enter")]
    public bool enterEvent;
    public string stateEnterBoolName;
    public bool stateEnterBoolValue;

    [Header("Exit")]
    public bool exitEvent;
    public string stateExitBoolName;
    public bool stateExitBoolValue;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enterEvent)
        {
            animator.SetBool(stateEnterBoolName, stateEnterBoolValue);
        }
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (exitEvent)
        {
            animator.SetBool(stateExitBoolName, stateExitBoolValue);
        }
    }


}
