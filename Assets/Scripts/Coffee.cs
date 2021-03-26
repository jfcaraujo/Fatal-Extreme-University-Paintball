using UnityEngine;

/// <summary>
/// Script to handle the Coffee item.
/// </summary>
public class Coffee : Item
{
    protected override bool ConsumeItem()
    {
        PowerUpController powerUpController = GameObject.FindObjectOfType<PowerUpController>();

        return powerUpController.ActivatePowerUp("SpeedUp");
    }
}
