using UnityEngine;

public class BackpackItem : Item
{
    protected override bool ConsumeItem()
    {
        PowerUpController powerUpController = GameObject.FindObjectOfType<PowerUpController>();

        return powerUpController.ActivatePowerUp("ShieldProtection");
    }
}
