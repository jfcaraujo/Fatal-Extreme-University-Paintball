using System.Collections.Generic;
using UnityEngine;

public class Healing : StateMachineBehaviour
{
    List<GameObject> splatters;
    Transform[] transforms;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        splatters = new List<GameObject>();

        Transform legRight = animator.gameObject.transform.Find("LegRight");
        Transform legLeft = animator.gameObject.transform.Find("LegLeft");
        Transform body = animator.gameObject.transform.Find("body");
        Transform head = body.Find("Head");

        transforms = new Transform[] { legRight, legLeft, body, head };

        GetSplatters();
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

                if (splatter)
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

        GetSplatters();

        // Remove any splatters left
        foreach (GameObject splatter in splatters)
        {
            if (splatter)
                Destroy(splatter);
        }

        splatters.Clear();
    }

    private void GetSplatters()
    {
        foreach (Transform item in transforms)
        {
            for (int i = 0; i < item.childCount; i++)
            {
                GameObject splatter = item.GetChild(i).gameObject;

                if (item.GetChild(i).CompareTag("Splatter"))
                    splatters.Add(splatter);
            }
        }
    }
}
