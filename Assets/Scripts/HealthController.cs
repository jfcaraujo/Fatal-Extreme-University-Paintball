using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public AudioManager audioManager;

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

    public PowerUpController powerUpController;

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

    public bool Damage(int damage, bool hitFront)
    {
        if (invulnerable)
            return false;

        // Vulnerability is set to true here
        // Has to be set to false at the end of the Heal animation
        invulnerable = true;
        powerUpController.DisableAll();

        // Disable collisions between player and enemies
        // Has to be reenabled at the end of the Heal animation
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        onHeal?.Invoke();

        // TODO: Invoke another event to signal that the player has lost and put enemies fighting against each other
        if (numPaperTowels <= 0)
        {
            animator.SetTrigger("Lose");
        }

        if (hitFront)
            animator.SetTrigger("HitFront");
        else
            animator.SetTrigger("HitBack");

        switch (Random.Range(0, 2))
        {
            case 0:
                audioManager.PlaySound("Damage1");
                break;
            default:
                audioManager.PlaySound("Damage2");
                break;
        }

        numPaperTowels = Mathf.Max(numPaperTowels - damage, 0);
        healthDisplay.text = numPaperTowels.ToString();
        return true;
    }

    public void StopHeal()
    {
        invulnerable = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        onStopHeal?.Invoke();
    }
}
