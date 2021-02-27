using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private Collider2D itemCollider;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Collider2D[] playerColliders = player.GetComponents<Collider2D>();

        Collider2D[] itemColliders = GetComponents<Collider2D>();

        for (int i = 0; i < itemColliders.Length; i++)
        {
            if (!itemColliders[i].isTrigger)
            {
                for (int j = 0; j < playerColliders.Length; j++)
                {
                    Physics2D.IgnoreCollision(itemColliders[i], playerColliders[j]);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if(ConsumeItem())
                Destroy(gameObject);
        }
    }

    protected abstract bool ConsumeItem();
}
