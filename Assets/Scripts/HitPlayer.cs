using UnityEngine;

/// <summary>
/// Script to handle the hit state of the player. Blocks player movement when it starts. It has to be unblocked later.
/// </summary>
public class HitPlayer : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Player_Controller>().inputBlocked = true;
    }
}
