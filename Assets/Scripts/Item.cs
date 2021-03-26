using UnityEngine;

/// <summary>
/// Abstract class to handle items from the game.
/// </summary>
public abstract class Item : MonoBehaviour
{
    private void Start()
    {
        // At the start, collisions between normal colliders (not triggers)
        // of the item object and the player are disabled
        // Those colliders are only used for collision between the item and the environment
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Collider2D playerCollider = player.GetComponent<Collider2D>();

        Collider2D[] itemColliders = GetComponents<Collider2D>();

        foreach (var itemCollider in itemColliders)
        {
            if (itemCollider.isTrigger) continue;
            Physics2D.IgnoreCollision(itemCollider, playerCollider);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Item is only destroyed if it is in fact consumed
            if (ConsumeItem())
                Destroy(gameObject);
        }
    }

    /// <summary>
    /// Consumes the item, applying its effect.
    /// </summary>
    /// <returns>If the item was consumed.</returns>
    protected abstract bool ConsumeItem();
}