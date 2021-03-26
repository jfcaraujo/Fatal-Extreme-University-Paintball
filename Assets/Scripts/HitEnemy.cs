using UnityEngine;

/// <summary>
/// Script to handle the hit state of the enemy. Blocks enemy movement while it's happening.
/// </summary>
public class HitEnemy : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<Enemy_Controller>().inputBlocked = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<Enemy_Controller>().inputBlocked = false;
    }
}
