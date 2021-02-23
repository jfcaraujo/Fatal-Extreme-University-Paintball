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

    public float bulletForce = 20f;

    // Fire cooldown in seconds
    public float fireCooldown = 0.4f;
    private bool allowFire = true;

    void Start() {
        firepoint = gameObject.transform.Find("FirePoint");
        rotationCenter = gameObject.transform.parent.parent.Find("Center");

        cam = FindObjectOfType<Camera>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<Player_Controller>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && allowFire)
        {
            StartCoroutine(Shoot());
        }

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate() {
        Aim();
    }

    void Aim()
    {
        Vector2 center = new Vector2(rotationCenter.position.x, rotationCenter.position.y);

        Vector2 lookDir = mousePos - center;
        float lookAngle = -(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 180f);

        Vector2 gunDir = new Vector2(firepoint.position.x, firepoint.position.y) - center;
        float gunAngle = -(Mathf.Atan2(gunDir.y, gunDir.x) * Mathf.Rad2Deg - 180f);

        Debug.Log(lookAngle + " " + gunAngle);

        gameObject.transform.RotateAround(rotationCenter.position, Vector3.back, lookAngle - gunAngle);

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
}
