using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private Player_Controller playerController;

    public bool DoubleSpeed { get; private set; }
    public bool SlowMotion { get; private set; }
    public bool ShieldProtection { get; private set; }

    public float powerUpDuration = 15f;

    private void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Controller>();
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
                break;
            case nameof(SlowMotion):
                SlowMotion = true;
                break;
            case nameof(ShieldProtection):
                ShieldProtection = true;
                break;
            default:
                return false;
        }

        StartCoroutine(TimePowerUp(powerUp));

        return true;
    }

    private IEnumerator TimePowerUp(string powerUp)
    {
        yield return new WaitForSeconds(powerUpDuration);

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
                break;
            case nameof(ShieldProtection):
                ShieldProtection = false;
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
