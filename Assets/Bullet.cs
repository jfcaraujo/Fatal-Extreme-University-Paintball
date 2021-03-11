using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject splatterPrefab;

    private bool exists = true;

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (!exists)
            return;

        exists = false;

        Vector3 position = gameObject.transform.position;

        // Places paint splatter
        GameObject splatterObject = Instantiate(splatterPrefab, position, Quaternion.identity, hitInfo.transform);

        SpriteRenderer splatterSR = splatterObject.GetComponent<SpriteRenderer>();

        // Set splatter color to pellet color
        splatterSR.color = gameObject.GetComponent<SpriteRenderer>().color;

        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // On the ground, the splatter is scaled so it looks angle
            splatterObject.transform.localScale = Vector3.Scale(splatterObject.transform.localScale, new Vector3(1, 0.75f, 1));

            // Ground splatters are visible on the "Default" sorting layer,
            // with order 5
            splatterSR.sortingOrder = 5;
        }
        else if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            // Obstacle splatters are visible on the "Environment" sorting layer,
            // with order 6
            splatterSR.sortingLayerName = "Environment";
            splatterSR.sortingOrder = 6;
        }

        // Destroys pellet
        Destroy(gameObject);

        // Damage entity (if applicable)
        Enemy_Controller enemy = hitInfo.GetComponent<Enemy_Controller>();
        if (enemy != null)
        {
            enemy.Damage(1);
        }
        HealthController healthController = hitInfo.gameObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            healthController.Damage(1);
        }
    }
}
