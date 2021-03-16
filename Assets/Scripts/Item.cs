using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Collider2D[] playerColliders = player.GetComponents<Collider2D>();

        Collider2D[] itemColliders = GetComponents<Collider2D>();

        foreach (var itemCollider in itemColliders)
        {
            if (itemCollider.isTrigger) continue;
            
            foreach (var playerCollider in playerColliders)
            {
                Physics2D.IgnoreCollision(itemCollider, playerCollider);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(ConsumeItem())
                Destroy(gameObject);
        }
    }

    protected abstract bool ConsumeItem();
}
