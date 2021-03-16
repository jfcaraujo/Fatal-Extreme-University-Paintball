using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmo : Item
{
    public string weaponName = "Pistol";
    public int ammo = 10;
    
    protected override bool ConsumeItem()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Weapon[] weapons = player.GetComponentsInChildren<Weapon>(true);

        foreach (var weapon in weapons)
        {
            if(weapon.name == weaponName) {
                return weapon.gainAmmo(ammo);
            }
        }

        return false;
    }
}
