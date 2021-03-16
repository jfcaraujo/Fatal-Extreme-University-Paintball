using System.Collections.Generic;
using UnityEngine;

public class Healing : StateMachineBehaviour
{
    List<GameObject> splatters;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        splatters = new List<GameObject>();

        Transform legRight = animator.gameObject.transform.Find("LegRight");
        Transform legLeft = animator.gameObject.transform.Find("LegLeft");
        Transform body = animator.gameObject.transform.Find("body");
        Transform head = body.Find("Head");

        foreach (Transform item in new Transform[] { legRight, legLeft, body, head })
        {
            for (int i = 0; i < item.childCount; i++)
            {
                if (item.GetChild(i).CompareTag("Splatter"))
                    splatters.Add(item.GetChild(i).gameObject);
            }
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Transform paperTowel = animator.gameObject.transform.Find("body").Find("PaperTowel");

        List<GameObject> splattersToDelete = new List<GameObject>();

        foreach (GameObject splatter in splatters)
        {
            if (Mathf.Abs((paperTowel.position - splatter.transform.position).magnitude) < 0.25)
            {
                splattersToDelete.Add(splatter);
                Destroy(splatter);
            }
        }

        splatters.RemoveAll(x => splattersToDelete.Contains(x));
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Player_Controller>().inputBlocked = false;
        animator.gameObject.GetComponent<HealthController>().StopHeal();

        // Remove any splatters left
        foreach (GameObject splatter in splatters)
        {
            Destroy(splatter);
        }

        splatters.Clear();
    }
}
