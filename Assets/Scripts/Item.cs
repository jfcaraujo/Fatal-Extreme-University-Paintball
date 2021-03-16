using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private void Start()
    {
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
            if (ConsumeItem())
                Destroy(gameObject);
        }
    }

    protected abstract bool ConsumeItem();
}