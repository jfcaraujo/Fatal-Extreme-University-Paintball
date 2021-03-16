using UnityEngine;

public class LoseEnemy : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<Enemy_Controller>().inputBlocked = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var parent = animator.transform.parent;
        Destroy(parent.gameObject);
        Instantiate(parent.GetComponent<Enemy_Controller>().ammoDrop, parent.position, Quaternion.identity);
    }
}
