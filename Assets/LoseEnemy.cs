using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseEnemy : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<Enemy_Controller>().inputBlocked = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.transform.parent.gameObject);
        Instantiate(animator.transform.parent.GetComponent<Enemy_Controller>().ammoDrop, animator.transform.parent.position, Quaternion.identity);
    }
}
