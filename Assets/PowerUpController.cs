using System.Collections;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private Player_Controller playerController;
    private TimeController timeController;

    public bool DoubleSpeed { get; private set; }
    public bool SlowMotion { get; private set; }
    public bool ShieldProtection { get; private set; }

    public float powerUpDuration = 15f;
    public float slowMotionFactor = 0.4f;

    public PowerUpDisplay powerUpDisplay;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Controller>();
        timeController = GameObject.FindObjectOfType<TimeController>();
        powerUpDisplay.SetMaxDuration((int) powerUpDuration);
    }

    public bool ActivatePowerUp(string powerUp)
    {
        if (IsAnyActive())
            return false;
        powerUpDisplay.StartNewPowerUp(1);
        switch (powerUp)
        {
            case nameof(DoubleSpeed):
                DoubleSpeed = true;
                playerController.doubleSpeed = true;
                break;
            case nameof(SlowMotion):
                SlowMotion = true;
                timeController.SlowDownTime(slowMotionFactor);
                break;
            case nameof(ShieldProtection):
                ShieldProtection = true;
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Backpack backpack = player.GetComponentInChildren<Backpack>(true);
                backpack.gameObject.SetActive(true);
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

    public bool IsAnyActive()
    {
        return DoubleSpeed || ShieldProtection || SlowMotion;
    }

    public void DisablePowerUp(string powerUp)
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
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Backpack backpack = player.GetComponentInChildren<Backpack>(true);
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
