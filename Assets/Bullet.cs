using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject splatterPrefab;
    public float maxDistance = 20f;

    private bool exists = true;
    private float travelledDistance = 0f;

    private void Update()
    {
        travelledDistance += GetComponent<Rigidbody2D>().velocity.magnitude * Time.deltaTime;

        if (travelledDistance > maxDistance)
        {
            RaycastHit2D[] raycasts = Physics2D.RaycastAll(transform.position, Vector3.back);

            List<GameObject> hitObjectsList = new List<GameObject>();

            // If there is an environment object, the splatter will only be placed on it
            // If not, the splatter will be placed in every BackgroundWall captured by the raycast
            for (int i = 0; i < raycasts.Length; i++)
            {
                if (raycasts[i].transform.gameObject.layer == LayerMask.NameToLayer("EnvironmentObjects"))
                {
                    hitObjectsList = new List<GameObject> { raycasts[i].transform.gameObject };
                    break;
                }

                if (raycasts[i].transform.gameObject.layer == LayerMask.NameToLayer("BackgroundWall"))
                {
                    // BackgroundWall layer is used only for colliders and masks, so we should send the parent
                    hitObjectsList.Add(raycasts[i].transform.parent.gameObject);
                }
            }

            DestroyPellet(hitObjectsList.ToArray());
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
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

        DestroyPellet(hitGameObjects);

        // Damage entity (if applicable)
        Enemy_Controller enemy = hitObject.GetComponent<Enemy_Controller>();
        if (enemy != null)
        {
            enemy.Damage(1);
        }
        HealthController healthController = hitObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            healthController.Damage(1);
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

        for (int i = 0; i < hitObjectsList.Length; i++)
        {
            PlaceSplatter(position, hitObjectsList[i]);
        }
    }

    private void PlaceSplatter(Vector3 position, GameObject hitObject)
    {
        if (hitObject == null)
            return;

        // Places paint splatter
        GameObject splatterObject = Instantiate(splatterPrefab, hitObject.transform, true);
        splatterObject.transform.position = position;

        SpriteRenderer splatterSR = splatterObject.GetComponent<SpriteRenderer>();

        // Set splatter color to pellet color
        splatterSR.color = gameObject.GetComponent<SpriteRenderer>().color;

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
