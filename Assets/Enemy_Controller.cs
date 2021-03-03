using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
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
    private bool jump = false;
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
    }

    void FixedUpdate()
    {
        /*m_Grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        foreach (var temp_collider in colliders)
        {
            if (temp_collider.gameObject != gameObject)
            {
                m_Grounded = true;
            }
        }*/
        
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

        jump = false;
    }
    
    private void Move(float move)
    {
        animator.SetFloat(HorizontalMove, Mathf.Abs(move));
        var velocity = m_Rigidbody2D.velocity;
        Vector3 targetVelocity = new Vector2(move, velocity.y);
        m_Rigidbody2D.velocity =
            Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        /*if (!m_Grounded || !jump) return;
        // Add a vertical force to the player.
        m_Grounded = false;
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));*/
    }

    IEnumerator Shoot()
    {
        allowFire = false;

        GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
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
        health-=damage;
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