using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    private static readonly List<Color> colors = new List<Color>()
    {
        new Color(1f, 0f, 0f, 1f), // Red
        new Color(0f, 1f, 0f, 1f), // Green
        new Color(0f, 0f, 1f, 1f), // Blue 
    };

    private int colorIndex = -1;

    [SerializeField] private float m_JumpForce = 500f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;

    public Animator animator;

    const float k_GroundedRadius = .2f;
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_FacingRight = true;
    private bool m_Grounded;
    public float runSpeed = 8f;
    public Transform target;
    public Transform firepoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;

    public float fireCooldown = 0.4f; // Fire cooldown in seconds
    [SerializeField] private float health = 1;
    private bool allowFire = true;
    public GameObject ammoDrop;
    private static readonly int HorizontalMove = Animator.StringToHash("HorizontalMove");


    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (colors.Count > 0)
        {
            // Use instance ID as an unique seed
            System.Random rnd = new System.Random(gameObject.GetInstanceID());
            colorIndex = rnd.Next(colors.Count);
        }
    }

    void FixedUpdate()
    {
        m_Grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        foreach (var temp_collider in colliders)
        {
            if (temp_collider.gameObject != gameObject)
            {
                m_Grounded = true;
            }
        }

        //flip the player
        if (target.position.x - gameObject.transform.position.x < 0 && m_FacingRight) Flip();
        if (target.position.x - gameObject.transform.position.x > 0 && !m_FacingRight) Flip();
        if (Mathf.Abs(target.position.x - gameObject.transform.position.x) > 5)
        {
            if (target.position.x < gameObject.transform.position.x)
            {
                Move(-runSpeed);
            }
            else
            {
                Move(runSpeed);
            }
        }
        else
        {
            Move(0);
            if (allowFire)
                StartCoroutine(Shoot());
        }
    }

    private void Move(float move)
    {
        animator.SetFloat(HorizontalMove, Mathf.Abs(move));
        var velocity = m_Rigidbody2D.velocity;
        Vector3 targetVelocity = new Vector2(move, velocity.y);
        m_Rigidbody2D.velocity =
            Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        if (m_Grounded && Math.Abs(velocity.x) < Math.Abs(m_Rigidbody2D.velocity.x / 2)) { Jump(); }
    }

    void Jump()
    {
        if (m_Rigidbody2D.velocity.y > 0.05) return;
        // Add a vertical force to the player.
        m_Grounded = false;
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
    }

    IEnumerator Shoot()
    {
        allowFire = false;

        GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);

        if (colorIndex != -1)
        {
            bullet.GetComponent<SpriteRenderer>().color = colors[colorIndex];
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        bullet.layer = LayerMask.NameToLayer("EnemyBullets");
        rb.AddForce(firepoint.right * bulletForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(fireCooldown);

        allowFire = true;
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        gameObject.transform.Rotate(0f, 180, 0f);
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //TODO add animation
        Destroy(gameObject);
        Instantiate(ammoDrop, gameObject.transform.position, Quaternion.identity);
    }
}