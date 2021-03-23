using UnityEngine;

public class DropTrap : MonoBehaviour
{
    public AudioManager audioManager;
    private Rigidbody2D m_rigidbody2D;

    public delegate void OnTrapSprung();

    public event OnTrapSprung onTrapSprung;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody2D = gameObject.GetComponentInChildren<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //activated when rectangle is hit
        if (other.name == "Player")
        {
            onTrapSprung?.Invoke();
            if (m_rigidbody2D != null)
            {
                audioManager.PlaySound("Release");

                m_rigidbody2D.gravityScale = 1.5f;
                m_rigidbody2D.angularVelocity = 100f;
            }

            Destroy(gameObject.GetComponent<Collider2D>());
        }
    }
}