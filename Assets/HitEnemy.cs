using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEnemy : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<Enemy_Controller>().inputBlocked = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<Enemy_Controller>().inputBlocked = false;
    }
}
