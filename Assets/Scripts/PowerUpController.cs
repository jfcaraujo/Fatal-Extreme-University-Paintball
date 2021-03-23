using System.Collections;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public AudioManager audioManager;
    public Player_Controller playerController;
    public Backpack backpack;

    public bool SpeedUp { get; private set; }
    public bool SlowMotion { get; private set; }
    public bool ShieldProtection { get; private set; }

    public float powerUpDuration = 15f;
    public float slowMotionFactor = 0.4f;

    public PowerUpDisplay powerUpDisplay;

    private void Start()
    {
        powerUpDisplay.SetMaxDuration((int) powerUpDuration);
    }

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

        StartCoroutine(TimePowerUp(powerUp));

        return true;
    }

    private IEnumerator TimePowerUp(string powerUp)
    {
        if (powerUp == nameof(SlowMotion))
            yield return new WaitForSeconds(powerUpDuration*slowMotionFactor);
        else
            yield return new WaitForSeconds(powerUpDuration);

        DisablePowerUp(powerUp);
    }

    private bool IsAnyActive()
    {
        return SpeedUp || ShieldProtection || SlowMotion;
    }

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

    public void DisableAll()
    {
        if (SpeedUp) DisablePowerUp(nameof(SpeedUp));
        if (SlowMotion) DisablePowerUp(nameof(SlowMotion));
        if (ShieldProtection) DisablePowerUp(nameof(ShieldProtection));
        powerUpDisplay.EndDisplay();
    }
}