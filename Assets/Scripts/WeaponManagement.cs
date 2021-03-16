using UnityEngine;
using UnityEngine.UI;

public class WeaponManagement : MonoBehaviour
{
    public int selectedWeapon = 0;
    int previousSelectedWeapon = 0;
    int penultimateWeapon = 0;
    public Transform weaponUI;
    public Text ammoDisplay;


    // Start is called before the first frame update
    private void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("SwitchLastWeapon")) {
            selectedWeapon = penultimateWeapon;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon == 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }

        if(previousSelectedWeapon != selectedWeapon)
            SelectWeapon();
    }

    private void SelectWeapon()
    {
        penultimateWeapon = previousSelectedWeapon;
        previousSelectedWeapon = selectedWeapon;

        int i = 0;
        foreach (Transform weapon in transform)
        {
            weaponUI.GetChild(i).gameObject.SetActive(i == selectedWeapon);//update ui sprite
            weapon.gameObject.SetActive(i == selectedWeapon);
            i++;
        }
        ammoDisplay.text=gameObject.GetComponentInChildren<Weapon>(false).remainingAmmo.ToString();

    }
}
