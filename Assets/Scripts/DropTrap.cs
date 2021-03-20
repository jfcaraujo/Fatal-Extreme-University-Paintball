using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DropTrap : MonoBehaviour
{
    [SerializeField] private bool parentComponent;
    Rigidbody2D m_rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody2D = gameObject.GetComponentInChildren<Rigidbody2D>();
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parentComponent) //activated when rectangle is hit
        {
            if (other.name == "Player")
            {
                m_rigidbody2D.gravityScale = 1.5f;
                m_rigidbody2D.angularVelocity = 100f;
                Destroy(gameObject.GetComponent<Collider2D>());
            }
        }
        /*else //activated when circle collider (flask itself) is hit
        {
            Debug.Log("collision with " + other.name);
            Bullet.BulletProcessing(other);
        }*/
    }
}