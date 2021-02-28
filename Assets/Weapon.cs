using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Camera cam;

    private Vector2 mousePos;

    private Player_Controller playerController;
    public GameObject bulletPrefab;
    private Transform firepoint = null;
    private Transform rotationCenter = null;

    public bool automaticFire = false;
    public float bulletForce = 20f;

    public float fireCooldown = 0.4f; // Fire cooldown in seconds
    public int remainingAmmo = 10;
    public int maxAmmo = 150;
    private bool allowFire = true;

    void Start()
    {
        firepoint = gameObject.transform.Find("FirePoint");
        rotationCenter = gameObject.transform.parent.parent.Find("Center");

        cam = FindObjectOfType<Camera>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<Player_Controller>();
    }

    void Update()
    {
        if (playerController.inputBlocked)
            return;

        bool shouldFire = automaticFire ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
        if (shouldFire && allowFire && remainingAmmo > 0)
        {
            remainingAmmo--;
            StartCoroutine(Shoot());
        }

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        if (playerController.inputBlocked)
            return;

        Aim();
    }

    void Aim()
    {
        var centerPosition = rotationCenter.position;
        Vector2 center = new Vector2(centerPosition.x, centerPosition.y);

        Vector2 lookDir = mousePos - center;
        float lookAngle = -(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 180f);

        Vector2 gunDir = new Vector2(firepoint.position.x, firepoint.position.y) - center;
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
        allowFire = false;

        GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(-firepoint.right * bulletForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(fireCooldown);

        allowFire = true;
    }

    public bool gainAmmo(int amount)
    {
        if (remainingAmmo >= maxAmmo)
            return false;

        remainingAmmo = Mathf.Min(remainingAmmo + amount, maxAmmo);

        return true;
    }
}
