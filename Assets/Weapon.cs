using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firepoint;

    public float bulletForce = 20f;

    // Fire cooldown in seconds
    public float fireCooldown = 0.4f;
    private bool allowFire = true;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && allowFire)
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        allowFire = false;

        GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(-firepoint.right * bulletForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(fireCooldown);

        allowFire = true;
    }
}
