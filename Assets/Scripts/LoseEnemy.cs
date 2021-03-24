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
        Instantiate(parent.GetComponent<Enemy_Controller>().ammoDrop, parent.position, Quaternion.identity)
            .GetComponent<Rigidbody2D>().AddForce(new Vector2(-1f, 0.5f), ForceMode2D.Impulse);
        float probability = Random.value;
        GameObject item = null;
        if (probability < 0.025)
        {
            item = Instantiate(parent.GetComponent<Enemy_Controller>().Droppables[0], parent.position,
                Quaternion.identity);
        }
        else if (probability < 0.05)
        {
            item = Instantiate(parent.GetComponent<Enemy_Controller>().Droppables[1], parent.position,
                Quaternion.identity);
        }
        else if (probability < 0.075)
        {
            item = Instantiate(parent.GetComponent<Enemy_Controller>().Droppables[2], parent.position,
                Quaternion.identity);
        }
        else if (probability < 0.125)
        {
            item = Instantiate(parent.GetComponent<Enemy_Controller>().Droppables[3], parent.position,
                Quaternion.identity);
        }

        if (item)
            item.GetComponent<Rigidbody2D>().AddForce(new Vector2(1f, 0.5f), ForceMode2D.Impulse);
    }
}