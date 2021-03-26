using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to handle the healing state of the player.
/// </summary>
public class Healing : StateMachineBehaviour
{
    List<GameObject> splatters;
    Transform[] transforms;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        splatters = new List<GameObject>();

        // When the state starts, every splatter from every body part
        // is retrieved to be removed throughout the animation
        Transform legRight = animator.gameObject.transform.Find("LegRight");
        Transform legLeft = animator.gameObject.transform.Find("LegLeft");
        Transform body = animator.gameObject.transform.Find("body");
        Transform head = body.Find("Head");

        transforms = new Transform[] { legRight, legLeft, body, head };

        GetSplatters();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        // Throughout the animation, the paper towel position will be checked
        // If it overlaps with a splatter, that splatter will be removed

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

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // When the healing stops, player interactions are reactivated
        animator.gameObject.GetComponent<Player_Controller>().inputBlocked = false;
        animator.gameObject.GetComponent<HealthController>().StopHeal();

        // In case any splatters were added to the player object after this state was entered
        // We retrieve them again to be removed in the next instructions
        GetSplatters();

        // Remove any splatters left
        foreach (GameObject splatter in splatters)
        {
            if (splatter)
                Destroy(splatter);
        }

        splatters.Clear();
    }

    /// <summary>
    /// Gets all the splatters in the player object.
    /// </summary>
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
