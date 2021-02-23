using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagement : MonoBehaviour
{
    public int selectedWeapon = 0;
    int previousSelectedWeapon = 0;
    int penultimateWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
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

    void SelectWeapon()
    {
        penultimateWeapon = previousSelectedWeapon;
        previousSelectedWeapon = selectedWeapon;

        int i = 0;
        foreach (Transform weapon in transform)
        {
            weapon.gameObject.SetActive(i == selectedWeapon);
            i++;
        }
    }
}
