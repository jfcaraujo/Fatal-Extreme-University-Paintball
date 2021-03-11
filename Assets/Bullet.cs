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
        DestroyPellet(new GameObject[] { hitInfo.gameObject });
    }

    private void DestroyPellet(GameObject[] hitObjectsList)
    {
        if (!exists)
            return;

        exists = false;

        Vector3 position = gameObject.transform.position;

        // Destroys pellet
        Destroy(gameObject);

        if (hitObjectsList == null || hitObjectsList.Length == 0)
            return;

        for (int i = 0; i < hitObjectsList.Length; i++)
        {
            GameObject hitObject = hitObjectsList[i];

            if (hitObject == null)
                continue;

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

            // Damage entity (if applicable)
            Enemy_Controller enemy = gameObject.GetComponent<Enemy_Controller>();
            if (enemy != null)
            {
                enemy.Damage(1);
            }
            HealthController healthController = gameObject.GetComponent<HealthController>();
            if (healthController != null)
            {
                healthController.Damage(1);
            }
        }
    }
}
