using UnityEngine;

/// <summary>
/// Script to handle the Energy Drink item.
/// </summary>
public class EnergyDrink : Item
{
    protected override bool ConsumeItem()
    {
        PowerUpController powerUpController = GameObject.FindObjectOfType<PowerUpController>();

        return powerUpController.ActivatePowerUp("SlowMotion");
    }
}
