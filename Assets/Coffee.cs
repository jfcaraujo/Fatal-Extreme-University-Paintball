using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coffee : Item
{
    protected override bool ConsumeItem()
    {
        PowerUpController powerUpController = GameObject.FindObjectOfType<PowerUpController>();

        return powerUpController.ActivatePowerUp("DoubleSpeed");
    }
}
