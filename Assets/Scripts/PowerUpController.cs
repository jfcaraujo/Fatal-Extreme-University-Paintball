using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the power-ups in the game. Every effect is for the player.
/// Names of power-ups are the name of the properties.
/// </summary>
public class PowerUpController : MonoBehaviour
{
    public AudioManager audioManager;
    public Player_Controller playerController;
    public Backpack backpack;
    public PowerUpDisplay powerUpDisplay;

    public bool SpeedUp { get; private set; }
    public bool SlowMotion { get; private set; }
    public bool ShieldProtection { get; private set; }

    public float powerUpDuration = 15f;
    public float slowMotionFactor = 0.4f;

    private void Start()
    {
        // Adjust power-up display to the correct duration
        powerUpDisplay.SetMaxDuration((int)powerUpDuration);
    }

    /// <summary>
    /// Activates a power-up.
    /// </summary>
    /// <param name="powerUp">Name of power-up to be activated.</param>
    /// <returns>If the power-up was activated.</returns>
    public bool ActivatePowerUp(string powerUp)
    {
        if (IsAnyActive() || playerController.inputBlocked)
            return false;

        switch (powerUp)
        {
            case nameof(SpeedUp):
                SpeedUp = true;
                playerController.speedUp = true;
                powerUpDisplay.StartNewPowerUp(1);
                audioManager.PlaySound("Drinking");
                break;
            case nameof(SlowMotion):
                SlowMotion = true;
                TimeController.ChangeTime(slowMotionFactor);
                powerUpDisplay.StartNewPowerUp(2);
                audioManager.PlaySound("Drinking");
                break;
            case nameof(ShieldProtection):
                ShieldProtection = true;
                backpack.gameObject.SetActive(true);
                powerUpDisplay.StartNewPowerUp(0);
                audioManager.PlaySound("Zipper");
                break;
            default:
                return false;
        }

        // Starts the coroutine that will wait a specified time to disable the power-up
        StartCoroutine(TimePowerUp(powerUp));

        return true;
    }

    /// <summary>
    /// Waits the specified time to disable the power-up.
    /// </summary>
    /// <param name="powerUp">Name of power-up to be disabled.</param>
    private IEnumerator TimePowerUp(string powerUp)
    {
        if (powerUp == nameof(SlowMotion))
            yield return new WaitForSeconds(powerUpDuration * slowMotionFactor);
        else
            yield return new WaitForSeconds(powerUpDuration);

        DisablePowerUp(powerUp);
    }

    /// <summary>
    /// Checks if any of the power-ups is active.
    /// </summary>
    /// <returns>If any power-up is active.</returns>
    private bool IsAnyActive()
    {
        return SpeedUp || ShieldProtection || SlowMotion;
    }

    /// <summary>
    /// Disables power-up.
    /// </summary>
    /// <param name="powerUp">Name of power-up to be disabled.</param>
    private void DisablePowerUp(string powerUp)
    {
        switch (powerUp)
        {
            case nameof(SpeedUp):
                SpeedUp = false;
                playerController.speedUp = false;
                break;
            case nameof(SlowMotion):
                SlowMotion = false;
                TimeController.ResetTime();
                break;
            case nameof(ShieldProtection):
                ShieldProtection = false;
                backpack.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Disables any active power-up.
    /// </summary>
    public void DisableAll()
    {
        if (SpeedUp) DisablePowerUp(nameof(SpeedUp));
        if (SlowMotion) DisablePowerUp(nameof(SlowMotion));
        if (ShieldProtection) DisablePowerUp(nameof(ShieldProtection));
        powerUpDisplay.EndDisplay();
    }
}