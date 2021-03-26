using UnityEngine;

/// <summary>
/// Script to handle the Backpack item.
/// </summary>
public class BackpackItem : Item
{
    protected override bool ConsumeItem()
    {
        PowerUpController powerUpController = GameObject.FindObjectOfType<PowerUpController>();

        return powerUpController.ActivatePowerUp("ShieldProtection");
    }
}
