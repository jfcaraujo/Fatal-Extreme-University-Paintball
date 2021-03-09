using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    // Health
    private int numPaperTowels;
    public int startingPaperTowels = 5;
    public int maxPaperTowels = 10;
    public bool invulnerable = false;

    private Animator animator;
    public Text healthDisplay;
    public delegate void OnHeal();
    public event OnHeal onHeal;
    public event OnHeal onStopHeal;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        numPaperTowels = startingPaperTowels;
        healthDisplay.text = numPaperTowels.ToString();
    }

    public bool AddPaperTowels(int num)
    {
        if (numPaperTowels >= maxPaperTowels)
            return false;

        numPaperTowels = Mathf.Min(numPaperTowels + num, maxPaperTowels);
        healthDisplay.text = numPaperTowels.ToString();
        return true;
    }

    public bool Damage(int damage)
    {
        if (invulnerable)
            return false;

        // Vulnerability is set to true here
        // Has to be set to false at the end of the Heal animation
        invulnerable = true;

        // Disable collisions between player and enemies
        // Has to be reenabled at the end of the Heal animation
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        numPaperTowels = Mathf.Max(numPaperTowels - damage, 0);
        healthDisplay.text = numPaperTowels.ToString();
        onHeal?.Invoke();
        if (numPaperTowels > 0)
            animator.SetTrigger("Heal");
        else
            // TODO: create leave animation
            animator.SetTrigger("Leave");

        return true;
    }

    public void StopHeal()
    {
        invulnerable = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        onStopHeal?.Invoke();
    }
}
