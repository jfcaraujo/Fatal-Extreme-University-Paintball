using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject splatterPrefab;
    public float maxDistance = 20f;

    private bool exists = true;
    private float travelledDistance = 0f;
    private Rigidbody2D m_Rigidbody2D;
    private Color color;

    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        color = gameObject.GetComponent<SpriteRenderer>().color;
    }
    
    private void Update()
    {
        travelledDistance += m_Rigidbody2D.velocity.magnitude * Time.deltaTime;

        if (travelledDistance <= maxDistance) return;
        
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

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (!exists)
            return;
        
        GameObject hitObject = hitInfo.gameObject;

        GameObject[] hitGameObjects = new GameObject[] { hitObject };

        if (hitObject.layer == LayerMask.NameToLayer("Player"))
        {
            Transform legRight = hitObject.transform.Find("LegRight");
            Transform legLeft = hitObject.transform.Find("LegLeft");
            Transform body = hitObject.transform.Find("body");
            Transform head = body.Find("Head");

            hitGameObjects = new GameObject[] { legRight.gameObject, legLeft.gameObject, body.gameObject, head.gameObject };
        }
        else if (hitObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            GameObject obj = hitObject.GetComponentInChildren<SpriteRenderer>().gameObject;

            hitGameObjects = new GameObject[] { obj };
        }

        DestroyPellet(hitGameObjects);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Damage entity (if applicable)
        Enemy_Controller enemy = hitObject.GetComponent<Enemy_Controller>();
        if (enemy != null)
        {
            bool hitFront = enemy.getFacingRight() && rb.velocity.x < 0 ||
                            !enemy.getFacingRight() && rb.velocity.x > 0;

            enemy.Damage(1, hitFront);
        }
        HealthController healthController = hitObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            Player_Controller pc = hitObject.GetComponent<Player_Controller>();

            var velocity = rb.velocity;
            bool hitFront = pc.m_FacingRight && velocity.x < 0 ||
                            !pc.m_FacingRight && velocity.x > 0;

            healthController.Damage(1, hitFront);
        }
    }

    private void DestroyPellet(GameObject[] hitObjectsList)
    {
        if (!exists)
            return;

        exists = false;

        Vector3 position = gameObject.transform.position;

        Destroy(gameObject);

        if (hitObjectsList == null)
            return;

        foreach (var hitObject in hitObjectsList)
        {
            PlaceSplatter(position, hitObject);
        }
    }

    private void PlaceSplatter(Vector3 position, GameObject hitObject)
    {
        if (!hitObject)
            return;

        // Places paint splatter
        GameObject splatterObject = Instantiate(splatterPrefab, hitObject.transform, true);
        splatterObject.transform.position = position;

        SpriteRenderer splatterSR = splatterObject.GetComponent<SpriteRenderer>();

        // Set splatter color to pellet color
        splatterSR.color = color;

        if (hitObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // On the ground, the splatter is scaled so it looks angle
            splatterObject.transform.localScale = Vector3.Scale(splatterObject.transform.localScale, new Vector3(1, 0.75f, 1));

            // Ground splatters are visible on the "Default" sorting layer,
            // with order 5
            splatterSR.sortingOrder = 5;
        }
        else if (hitObject.layer == LayerMask.NameToLayer("Obstacles"))
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
            // Player splatters are visible on the "Player" sorting layer
            splatterSR.sortingLayerName = "Player";

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
        else if (hitObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Enemy splatters are visible on the "Player" sorting layer,
            // with order 6
            splatterSR.sortingLayerName = "Player";
            splatterSR.sortingOrder = 6;
        }
    }
}
