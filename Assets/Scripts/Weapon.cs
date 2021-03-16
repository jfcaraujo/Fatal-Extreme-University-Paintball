using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    private Camera cam;

    private Vector2 mousePos;

    private Player_Controller playerController;
    public GameObject bulletPrefab;
    private Transform firePoint = null;
    private Transform rotationCenter = null;
    private Text ammoDisplay;

    public bool automaticFire = false;
    public float bulletForce = 20f;

    public float fireCooldown = 0.4f; // Fire cooldown in seconds
    public int remainingAmmo = 10;
    public int maxAmmo = 150;
    private bool allowFire = true;

    private void Start()
    {
        firePoint = gameObject.transform.Find("FirePoint");
        rotationCenter = gameObject.transform.parent.parent.Find("Center");

        cam = FindObjectOfType<Camera>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<Player_Controller>();
        ammoDisplay = GameObject.Find("Ammo Amount").GetComponent<Text>();
    }

    private void Update()
    {
        if (playerController.inputBlocked)
            return;

        bool shouldFire = automaticFire ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
        if (shouldFire && allowFire && remainingAmmo > 0)
        {
            StartCoroutine(Shoot());
        }

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        if (playerController.inputBlocked)
            return;

        Aim();
    }

    private void Aim()
    {
        var centerPosition = rotationCenter.position;
        Vector2 center = new Vector2(centerPosition.x, centerPosition.y);

        Vector2 lookDir = mousePos - center;
        float lookAngle = -(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 180f);

        var position = firePoint.position;
        Vector2 gunDir = new Vector2(position.x, position.y) - center;
        float gunAngle = -(Mathf.Atan2(gunDir.y, gunDir.x) * Mathf.Rad2Deg - 180f);

        gameObject.transform.RotateAround(centerPosition, Vector3.back, lookAngle - gunAngle);

        if ((lookAngle < 85 || lookAngle > 275) && playerController.m_FacingRight)
        {
            playerController.Flip();
        }
        else if (lookAngle > 95 && lookAngle < 265 && !playerController.m_FacingRight)
        {
            playerController.Flip();
        }
    }

    IEnumerator Shoot()
    {
        remainingAmmo--;
        allowFire = false;
        ammoDisplay.text = remainingAmmo.ToString();


        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(-firePoint.right * bulletForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(fireCooldown);

        allowFire = true;
    }

    public bool gainAmmo(int amount)
    {
        if (remainingAmmo >= maxAmmo)
            return false;

        remainingAmmo = Mathf.Min(remainingAmmo + amount, maxAmmo);

        if (gameObject.activeSelf)
            ammoDisplay.text = remainingAmmo.ToString();
        return true;
    }
}