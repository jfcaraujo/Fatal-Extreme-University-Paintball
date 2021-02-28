using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: trigger enemy damage
        
        Vector3 position = gameObject.transform.position;
        Destroy(gameObject);

        HealthController healthController = other.gameObject.GetComponent<HealthController>();

        if (healthController != null)
        {
            healthController.Damage(1);
        }

        GameObject effect = Instantiate(hitEffect, position, Quaternion.identity);
        Destroy(effect, 0.6f);
    }
}
