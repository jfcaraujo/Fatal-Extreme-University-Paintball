﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to handle the objects that act as projectiles.
/// </summary>
public class Bullet : MonoBehaviour
{
    // Splatter prefab to be placed
    public GameObject splatterPrefab;

    // Maximum distance the bullet can travel before being destroyed
    public float maxDistance = 20f;

    // These flags are used to give specific behaviour to an object
    [SerializeField] private bool isTrap = false;
    [SerializeField] private bool isGrenade = false;

    private bool exists = true;
    private float travelledDistance = 0f;

    private Rigidbody2D m_Rigidbody2D;
    private SpriteRenderer spriteRenderer;
    private Collider2D bulletCollider;

    // To keep track of the collider that is deactivated before the object is sent
    // (this is just used for traps or grenades)
    private Collider2D deactivatedCollider;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        deactivatedCollider = null;

        // If object is grenade or trap, deactivate the collisions between it and the upper ground floor
        // Upper ground floor collider will be stored so it can be activated later
        if (isGrenade && !isTrap)
        {
            RaycastHit2D raycast = Physics2D.Raycast(new Vector3(transform.position.x, 4, transform.position.z), Vector2.up,
                    Mathf.Infinity, LayerMask.GetMask("UpperGround"));

            if (raycast.collider)
            {
                deactivatedCollider = raycast.collider;

                Physics2D.IgnoreCollision(deactivatedCollider, bulletCollider, true);
            }
        }
    }

    private void Update()
    {
        if (!exists)
            return;

        // Travelled distance is recorded to trigger destruction
        travelledDistance += m_Rigidbody2D.velocity.magnitude * Time.deltaTime;

        // Reactivate previously deactivated collider, when object has reached its trajectory peak
        // (only applicable when object is thrown upwards, when it goes downwards the collision does not need to be reactivated)
        if (isGrenade && !isTrap && travelledDistance > 0)
        {
            if (Mathf.Abs(m_Rigidbody2D.velocity.y) <= 0.01)
            {
                Physics2D.IgnoreCollision(deactivatedCollider, bulletCollider, false);
            }
        }

        if (travelledDistance <= maxDistance) return;

        // Paint background surfaces
        PaintOtherSurfaces();
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (!exists)
            return;

        GameObject hitObject = hitInfo.gameObject;

        // Bullet knockback
        if (hitInfo.attachedRigidbody != null)
            hitInfo.attachedRigidbody.AddForce(m_Rigidbody2D.velocity.normalized * 15, ForceMode2D.Impulse);

        // If hit object is a bullet, only paint background surfaces
        if (hitObject.layer == LayerMask.NameToLayer("PlayerBullets")
            || hitObject.layer == LayerMask.NameToLayer("EnemyBullets")
            || hitObject.layer == LayerMask.NameToLayer("Grenade"))
        {
            PaintOtherSurfaces();
            return;
        }

        GameObject[] hitGameObjects = new GameObject[] { hitObject };

        // If hit object is the Player, get body parts of the player to have splatters placed
        if (hitObject.layer == LayerMask.NameToLayer("Player"))
        {
            Transform legRight = hitObject.transform.Find("LegRight");
            Transform legLeft = hitObject.transform.Find("LegLeft");
            Transform body = hitObject.transform.Find("body");
            Transform head = body.Find("Head");

            hitGameObjects = new GameObject[]
                {legRight.gameObject, legLeft.gameObject, body.gameObject, head.gameObject};
        }
        else if (hitObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            GameObject obj = hitObject.GetComponentInChildren<SpriteRenderer>().gameObject;

            hitGameObjects = new GameObject[] { obj };
        }

        DestroyPellet(hitGameObjects);

        // Damage entity (if applicable)
        Enemy_Controller enemy = hitObject.GetComponent<Enemy_Controller>();
        if (enemy != null)
        {
            // Check if the bullet hit the enemy on the back to trigger animation
            bool hitFront = enemy.getFacingRight() && m_Rigidbody2D.velocity.x < 0 ||
                            !enemy.getFacingRight() && m_Rigidbody2D.velocity.x > 0;

            enemy.Damage(1, hitFront);
        }

        HealthController healthController = hitObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            Player_Controller pc = hitObject.GetComponent<Player_Controller>();

            // Check if the bullet hit the player on the back to trigger animation
            var velocity = m_Rigidbody2D.velocity;
            bool hitFront = pc.m_FacingRight && velocity.x < 0 ||
                            !pc.m_FacingRight && velocity.x > 0;

            healthController.Damage(isGrenade ? healthController.maxDamage : 1, hitFront);
        }
    }

    /// <summary>
    /// Places splatters on the objects/walls behind the bullet.
    /// </summary>
    void PaintOtherSurfaces()
    {
        RaycastHit2D[] raycasts = Physics2D.RaycastAll(transform.position, Vector3.back);

        List<GameObject> hitObjectsList = new List<GameObject>();

        // If there is an environment object, the splatter will only be placed on it
        // If not, the splatter will be placed in every BackgroundWall captured by the raycast
        foreach (var raycast in raycasts)
        {
            if (raycast.transform.gameObject.layer == LayerMask.NameToLayer("EnvironmentObjects"))
            {
                hitObjectsList = new List<GameObject> { raycast.transform.gameObject };
                break;
            }

            if (raycast.transform.gameObject.layer == LayerMask.NameToLayer("BackgroundWall"))
            {
                // BackgroundWall layer is used only for colliders and masks, so we should send the parent
                hitObjectsList.Add(raycast.transform.parent.gameObject);
            }
        }

        DestroyPellet(hitObjectsList.ToArray());
    }

    /// <summary>
    /// Destroys the object.
    /// </summary>
    /// <param name="hitObjectsList">List of hit objects</param>
    private void DestroyPellet(GameObject[] hitObjectsList)
    {
        if (!exists)
            return;

        exists = false;

        Vector3 position = gameObject.transform.position;

        if (hitObjectsList != null)
        {
            foreach (var hitObject in hitObjectsList)
            {
                PlaceSplatter(position, hitObject);
            }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Places a splatter.
    /// </summary>
    /// <param name="position">Position where the splatter will be placed.</param>
    /// <param name="hitObject">Object where the splatter will be placed.</param>
    private void PlaceSplatter(Vector3 position, GameObject hitObject)
    {
        if (!hitObject)
            return;

        // Places paint splatter
        GameObject splatterObject = Instantiate(splatterPrefab, hitObject.transform, true);

        // Z should always be positive
        if (isGrenade)
        {
            splatterObject.transform.position = new Vector3(position.x, position.y - 0.3f, Mathf.Abs(position.z));
            // Grenade splatters are bigger
            splatterObject.transform.localScale = splatterObject.transform.localScale * 3;
        }
        else
        {
            splatterObject.transform.position = new Vector3(position.x, position.y, Mathf.Abs(position.z));
        }

        Splatter splatterScript = splatterObject.GetComponent<Splatter>();

        if (splatterScript != null)
            splatterScript.isGrenade = isGrenade;

        SpriteRenderer splatterSR = splatterObject.GetComponent<SpriteRenderer>();

        // Set splatter color to pellet color
        if (isGrenade)
            splatterSR.color = Color.red;
        else
            splatterSR.color = spriteRenderer.color;

        if (hitObject.layer == LayerMask.NameToLayer("Ground") ||
            hitObject.layer == LayerMask.NameToLayer("UpperGround"))
        {
            // On the ground, the splatter is scaled so it looks angle
            splatterObject.transform.localScale =
                Vector3.Scale(splatterObject.transform.localScale, new Vector3(1, 0.75f, 1));

            // Ground splatters are visible on the "Default" sorting layer,
            // with order 5
            splatterSR.sortingOrder = 5;
        }
        else if (hitObject.layer == LayerMask.NameToLayer("Obstacles") || hitObject.layer == LayerMask.NameToLayer("UpperObstacles"))
        {
            // Obstacle splatters are visible on the "Environment" sorting layer,
            // with order 6
            splatterSR.sortingLayerName = "Environment";
            splatterSR.sortingOrder = 6;
        }
        else if (hitObject.layer == LayerMask.NameToLayer("BackgroundWall"))
        {
            // Darken paint in wall
            splatterSR.color *= 0.95f;

            // Wall splatters are visible on the "Default" sorting layer,
            // with order 4
            splatterSR.sortingOrder = 4;
        }
        else if (hitObject.layer == LayerMask.NameToLayer("EnvironmentObjects"))
        {
            // Environment Object splatters are visible on the "Environment" sorting layer,
            // with order 1
            splatterSR.sortingLayerName = "Environment";
            splatterSR.sortingOrder = 1;
        }
        else if (hitObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Player splatters are visible on the "PlayerFront" and "PlayerBack" sorting layer
            splatterSR.sortingLayerName = "PlayerFront";

            // Different order for each body part
            if (hitObject.name == "LegLeft")
            {
                splatterSR.sortingOrder = 1;
            }
            else if (hitObject.name == "LegRight")
            {
                splatterSR.sortingOrder = 2;
            }
            else if (hitObject.name == "body")
            {
                splatterSR.sortingOrder = 3;
            }
            else if (hitObject.name == "Head")
            {
                splatterSR.sortingOrder = 4;
            }
        }
        else if (hitObject.layer == LayerMask.NameToLayer("BulletShield"))
        {
            // Bullet shield splatters are visible on the "PlayerFront" and "PlayerBack" sorting layer
            // with order 5
            splatterSR.sortingLayerName = "PlayerFront";
            splatterSR.sortingOrder = 5;
        }
        else if (hitObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Enemy splatters are visible on the "Player" sorting layer,
            // with order 6
            splatterSR.sortingLayerName = "PlayerFront";
            splatterSR.sortingOrder = 6;
        }
    }

    private void OnDestroy()
    {
        // Message is sent to trigger drop trap object destruction
        if (isGrenade)
            SendMessageUpwards("OnChildDestroy", SendMessageOptions.DontRequireReceiver);
    }
}