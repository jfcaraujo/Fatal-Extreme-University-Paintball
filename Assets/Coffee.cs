using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coffee : Item
{
    protected override bool ConsumeItem()
    {
        return powerUpController.ActivatePowerUp("DoubleSpeed");
    }
}
