using UnityEngine;

public class WeaponAmmo : Item
{
    public string weaponName = "Pistol";
    public int ammo = 10;
    
    protected override bool ConsumeItem()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        WeaponManagement weaponManagement = player.GetComponentInChildren<WeaponManagement>();

        if(weaponManagement == null)
            return false;

        return weaponManagement.AddAmmo(weaponName, ammo);
    }
}
