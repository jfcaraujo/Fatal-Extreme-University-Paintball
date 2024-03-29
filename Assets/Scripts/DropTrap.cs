﻿using UnityEngine;

/// <summary>
/// Script to handle the Drop Trap objects.
/// </summary>
public class DropTrap : MonoBehaviour
{
    public AudioManager audioManager;
    private Rigidbody2D m_rigidbody2D;

    public delegate void OnTrapSprung();
    // Event invoked when trap is sprung
    public event OnTrapSprung onTrapSprung;

    void Start()
    {
        m_rigidbody2D = gameObject.GetComponentInChildren<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Activated when rectangle is hit
        if (other.name == "Player")
        {
            if (m_rigidbody2D != null)
            {
                audioManager.PlaySound("Release");

                m_rigidbody2D.gravityScale = 1.5f;
                m_rigidbody2D.angularVelocity = 100f;
            }

            Destroy(gameObject.GetComponent<Collider2D>());
        }
    }

    private void OnChildDestroy()
    {
        onTrapSprung?.Invoke();
        Destroy(gameObject);
    }
}