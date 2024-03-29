using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages a weapon of the player
/// </summary>
public class Weapon : MonoBehaviour
{
    private Camera cam;

    private Vector2 mousePos;

    public AudioManager audioManager;

    public Player_Controller playerController;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform rotationCenter;
    public Text ammoDisplay;

    public bool automaticFire = false;
    public float bulletForce = 20f;

    public float fireCooldown = 0.4f; // Fire cooldown in seconds
    public int remainingAmmo = 10;
    public int maxAmmo = 150;
    private bool allowFire = true;

    private float timeWhenDisabled = -1;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
    }

    private void OnEnable()
    {
        if (!allowFire && timeWhenDisabled != -1)//if when disabled there was cooldown left
        {
            float cooldownLeft = fireCooldown - (Time.time - timeWhenDisabled);

            if (cooldownLeft > 0)
                StartCoroutine(Cooldown(cooldownLeft));
            else
                allowFire = true;
        }
        // It should never enter here, but just in case
        else
            allowFire = true;
    }

    private void Update()
    {
        if (playerController.inputBlocked)//if player is healing
            return;

        bool shouldFire = automaticFire ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
        if (shouldFire && allowFire && remainingAmmo > 0 && !PauseMenu.gameIsPaused)
        {
            Shoot();
        }

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        if (playerController.inputBlocked)
            return;

        Aim();
    }

    ///<summary>
    /// Changes position of weapon sprite and flips player depending on mouse position
    ///</summary>
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

    ///<summary>
    /// Sends a bullet prefab flying out of the weapon
    ///</summary>
    private void Shoot()
    {
        remainingAmmo--;
        allowFire = false;
        ammoDisplay.text = remainingAmmo.ToString();


        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(-firePoint.right * bulletForce, ForceMode2D.Impulse);

        audioManager.PlaySound("Gun Shot");

        StartCoroutine(Cooldown(fireCooldown));
    }

    ///<summary>
    /// Allows fire again at the end of the weapon cooldown
    ///</summary>
    IEnumerator Cooldown(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        allowFire = true;
    }

    ///<summary>
    /// Adds ammo to the player. Fails if he already has the maximum amount of that ammo
    ///</summary>
    /// <param name="amount"> Amount to add </param>
    /// <returns> If the ammo was successfully added </returns>
    public bool GainAmmo(int amount)
    {
        if (remainingAmmo >= maxAmmo)
            return false;
        remainingAmmo = Mathf.Min(remainingAmmo + amount, maxAmmo);

        if (gameObject.activeSelf)
            ammoDisplay.text = remainingAmmo.ToString();

        return true;
    }

    private void OnDisable()
    {
        timeWhenDisabled = Time.time;//In case it was disabled in the middle of cooldown
    }
}