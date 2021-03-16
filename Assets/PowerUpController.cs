using System.Collections;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public Player_Controller playerController;
    public TimeController timeController;
    public Backpack backpack;

    public bool DoubleSpeed { get; private set; }
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
        if (IsAnyActive())
            return false;
        
        switch (powerUp)
        {
            case nameof(DoubleSpeed):
                DoubleSpeed = true;
                playerController.doubleSpeed = true;
                powerUpDisplay.StartNewPowerUp(1);
                break;
            case nameof(SlowMotion):
                SlowMotion = true;
                timeController.SlowDownTime(slowMotionFactor);
                powerUpDisplay.StartNewPowerUp(2);
                break;
            case nameof(ShieldProtection):
                ShieldProtection = true;
                backpack.gameObject.SetActive(true);
                powerUpDisplay.StartNewPowerUp(0);
                break;
            default:
                return false;
        }

        StartCoroutine(TimePowerUp(powerUp));

        return true;
    }

    private IEnumerator TimePowerUp(string powerUp)
    {
        yield return new WaitForSecondsRealtime(powerUpDuration);

        DisablePowerUp(powerUp);
    }

    private bool IsAnyActive()
    {
        return DoubleSpeed || ShieldProtection || SlowMotion;
    }

    private void DisablePowerUp(string powerUp)
    {
        switch (powerUp)
        {
            case nameof(DoubleSpeed):
                DoubleSpeed = false;
                playerController.doubleSpeed = false;
                break;
            case nameof(SlowMotion):
                SlowMotion = false;
                timeController.ResetTime();
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
        if (DoubleSpeed) DisablePowerUp(nameof(DoubleSpeed));
        if (SlowMotion) DisablePowerUp(nameof(SlowMotion));
        if (ShieldProtection) DisablePowerUp(nameof(ShieldProtection));
    }
}
