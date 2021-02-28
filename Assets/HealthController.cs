using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    // Health
    private int numPaperTowels;
    public int startingPaperTowels = 5;
    public int maxPaperTowels = 10;
    public bool invulnerable = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        numPaperTowels = startingPaperTowels;
    }

    public bool AddPaperTowels(int num)
    {
        if (numPaperTowels >= maxPaperTowels)
            return false;

        numPaperTowels = Mathf.Min(numPaperTowels + num, maxPaperTowels);

        return true;
    }

    public bool Damage(int damage)
    {
        if (invulnerable)
            return false;

        // Vulnerability is set to true here
        // Has to be set to false at the end of the Heal animation
        invulnerable = true;

        numPaperTowels = Mathf.Max(numPaperTowels - damage, 0);

        if (numPaperTowels > 0)
            animator.SetTrigger("Heal");
        else
            // TODO: create leave animation
            animator.SetTrigger("Leave");

        return true;
    }
}
