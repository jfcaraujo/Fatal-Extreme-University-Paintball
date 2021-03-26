using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the health of the player.
/// </summary>
public class HealthController : MonoBehaviour
{
    // Health
    private int numPaperTowels;
    public int startingPaperTowels = 5;
    public int maxPaperTowels = 10;

    // The player needs to be done the value in maxDamage
    // to trigger a paper towel use
    private int damageDone = 0;
    public int maxDamage = 3;

    public bool invulnerable = false;

    private Animator animator;
    public AudioManager audioManager;
    public PowerUpController powerUpController;
    public Text healthDisplay;
    public GameObject gameOverUI;

    // Events to tell listeners when the healing starts and stops
    public delegate void OnHeal();
    public event OnHeal onHeal;
    public event OnHeal onStopHeal;

    void Start()
    {
        animator = GetComponent<Animator>();

        numPaperTowels = startingPaperTowels;
        healthDisplay.text = numPaperTowels.ToString();

        damageDone = 0;
    }

    /// <summary>
    /// Increases the number of paper towels.
    /// </summary>
    /// <param name="num">Number of paper towels to be added.</param>
    /// <returns>If paper towels were added.</returns>
    public bool AddPaperTowels(int num)
    {
        if (numPaperTowels >= maxPaperTowels)
            return false;

        numPaperTowels = Mathf.Min(numPaperTowels + num, maxPaperTowels);
        healthDisplay.text = numPaperTowels.ToString();

        audioManager.PlaySound("PaperRip");

        return true;
    }

    /// <summary>
    /// Damages the player.
    /// </summary>
    /// <param name="damage">Amount of damage done to the player.</param>
    /// <param name="hitFront">If the player was hit in the front.</param>
    /// <returns>If the damage was applied.</returns>
    public bool Damage(int damage, bool hitFront)
    {
        if (invulnerable)
            return false;

        damageDone += damage;

        // If max damage was reached, it will heal
        if (damageDone < maxDamage)
            return true;

        // Vulnerability is set to true here
        // Has to be set to false at the end of the Heal animation
        invulnerable = true;
        powerUpController.DisableAll();

        // Disable collisions between player and enemies
        // Has to be reenabled at the end of the Heal animation
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        onHeal?.Invoke();

        // No more paper towels, player has lost
        if (numPaperTowels <= 0)
        {
            animator.SetTrigger("Lose");
            gameOverUI.SetActive(true);
            Time_Display.Stop();
        }

        if (hitFront)
            animator.SetTrigger("HitFront");
        else
            animator.SetTrigger("HitBack");

        // Damage sound is randomly picked
        switch (Random.Range(0, 2))
        {
            case 0:
                audioManager.PlaySound("Damage1");
                break;
            default:
                audioManager.PlaySound("Damage2");
                break;
        }

        damageDone = 0;

        // Paper towel has been used, decrease by one
        numPaperTowels = Mathf.Max(numPaperTowels - 1, 0);
        healthDisplay.text = numPaperTowels.ToString();

        return true;
    }

    /// <summary>
    /// Changes the player back to its normal state.
    /// </summary>
    public void StopHeal()
    {
        invulnerable = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        onStopHeal?.Invoke();
    }
}
