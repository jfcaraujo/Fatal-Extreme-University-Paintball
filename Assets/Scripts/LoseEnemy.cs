using UnityEngine;

/// <summary>
/// Script to handle the lose state of the enemy.
/// </summary>
public class LoseEnemy : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<Enemy_Controller>().inputBlocked = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Animator is in child object, the object that has to be destroyed is parent
        var parent = animator.transform.parent;

        Destroy(parent.gameObject);

        // For each drop, a small force is made on the object to move it a bit

        // Ammo is dropped
        Instantiate(parent.GetComponent<Enemy_Controller>().ammoDrop, parent.position, Quaternion.identity)
            .GetComponent<Rigidbody2D>().AddForce(new Vector2(-1f, 0.5f), ForceMode2D.Impulse);

        // Power-up can also be dropped, with a probability that depends on the difficulty
        float probability = Random.value;
        GameObject item = null;
        double probabilityAux = 1 + 0.25 * (3 - MainMenu.difficulty);
        
        if (probability < 0.025 * probabilityAux)
        {
            item = Instantiate(parent.GetComponent<Enemy_Controller>().Droppables[0], parent.position,
                Quaternion.identity);
        }
        else if (probability < 0.05 * probabilityAux)
        {
            item = Instantiate(parent.GetComponent<Enemy_Controller>().Droppables[1], parent.position,
                Quaternion.identity);
        }
        else if (probability < 0.075 * probabilityAux)
        {
            item = Instantiate(parent.GetComponent<Enemy_Controller>().Droppables[2], parent.position,
                Quaternion.identity);
        }
        else if (probability < 0.125 * probabilityAux)
        {
            item = Instantiate(parent.GetComponent<Enemy_Controller>().Droppables[3], parent.position,
                Quaternion.identity);
        }

        if (item)
            item.GetComponent<Rigidbody2D>().AddForce(new Vector2(1f, 0.5f), ForceMode2D.Impulse);
    }
}