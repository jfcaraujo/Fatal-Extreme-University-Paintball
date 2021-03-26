using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the selected weapon of the player
/// </summary>
public class WeaponManagement : MonoBehaviour
{
    public AudioManager audioManager;
    public int selectedWeapon = 0;
    int previousSelectedWeapon = 0;
    int penultimateWeapon = 1;
    public Transform weaponUI;
    public Text ammoDisplay;


    private void Start()
    {
        SelectWeapon();
    }

    private void Update()
    {
        if (Input.GetButtonDown("SwitchLastWeapon"))//if Q is pressed change weapon
        {
            selectedWeapon = penultimateWeapon;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)//if scroll up
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)//if scroll down
        {
            if (selectedWeapon == 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }

        if (previousSelectedWeapon != selectedWeapon)//if selected weapon changed, change active weapon
            SelectWeapon();
    }

    ///<summary>
    /// Changes active weapon to selected weapon.
    /// Also updates display
    ///</summary>
    private void SelectWeapon()
    {
        penultimateWeapon = previousSelectedWeapon;
        previousSelectedWeapon = selectedWeapon;

        int i = 0;
        foreach (Transform weapon in transform)
        {
            weaponUI.GetChild(i).gameObject.SetActive(i == selectedWeapon);//update ui sprite
            weapon.gameObject.SetActive(i == selectedWeapon);//activate weapon selected and deactivate the others
            i++;
        }
        ammoDisplay.text = gameObject.GetComponentInChildren<Weapon>(false).remainingAmmo.ToString();//update ui ammo amount

    }

    /// <summary>
    /// Increases ammo amount for a weapon
    /// </summary>
    /// <param name="weaponName">Name of weapon to add ammo to.</param>
    /// <param name="amount">Amount to be added.</param>
    public bool AddAmmo(string weaponName, int amount)
    {
        foreach (Transform weapon in transform)
        {
            if (weapon.name == weaponName)
            {
                Weapon weaponScript = weapon.GetComponent<Weapon>();
                bool consumed = weaponScript.GainAmmo(amount);

                if(consumed)
                    audioManager.PlaySound("Ammo");

                return consumed;
            }
        }

        return false;
    }
}
