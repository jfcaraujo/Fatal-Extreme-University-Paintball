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

        for (int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].name == weaponName) {
                return weapons[i].gainAmmo(ammo);
            }
        }

        return false;
    }
}
