using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    private static readonly List<Color> colors = new List<Color>()
    {
        Color.red, Color.green, Color.blue
    };

    private int colorIndex = -1;

    [SerializeField] private float m_JumpForce = 10.5f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;

    private Animator animator;

    public AudioManager audioManager;

    const float k_GroundedRadius = .2f;
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_FacingRight = true;
    private bool m_Grounded;
    public float runSpeed = 8f;
    private Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;

    private float seeingDistance = 15f;

    public float fireCooldown = 0.4f; // Fire cooldown in seconds
    [SerializeField] private float health = 1;
    private bool allowFire = true;
    public GameObject ammoDrop;

    private HealthController healthController;

    // fleeing: 0 = not fleeing; 1 = fleeing right; -1 = fleeing left
    private int fleeingDirection = 0;
    private bool roaming;
    private int roamingDirection = 1;

    private bool overcomingObstacle = false;
    private float overcomingDistanceToStop = 0;
    private bool overcomingRight = false;

    public delegate void OnEnemyDeath();

    public event OnEnemyDeath onEnemyDeath;

    public bool inputBlocked = false;

    public GameObject[] Droppables;

    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
        healthController = playerObject.GetComponent<HealthController>();
        healthController.onHeal += Flee;
        healthController.onStopHeal += StopFlee;
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

        float playerDistance = player.position.x - gameObject.transform.position.x;
        bool playerIsRight = playerDistance > 0;
        playerDistance = Math.Abs(playerDistance);

        if (inputBlocked) //if dying
        {
            Move(0);
            return;
        }

        if (fleeingDirection != 0) //if fleeing
        {
            Move(fleeingDirection * runSpeed);
        }
        else //if not fleeing
        {
            CheckLevel();
            if (roaming)
            {
                Flip(roamingDirection == 1);
                Move(roamingDirection * runSpeed);
                RaycastHit2D raycast = Physics2D.Raycast(transform.position,
                    roamingDirection * Vector2.right,
                    2, LayerMask.GetMask("Ground", "UpperGround"));

                if (raycast.collider) //if near a wall
                {
                    roamingDirection *= -1;
                }
            }
            else //chasing player if is in same level
            {
                Flip(playerIsRight); //flip in the direction of the player

                if (playerDistance <= 5) //if close
                {
                    if (m_Grounded) Jump();

                    //check if sees player
                    RaycastHit2D raycast = Physics2D.Raycast(
                        new Vector2(transform.position.x, transform.position.y - 0.144f),
                        (playerIsRight ? 1 : -1) * Vector2.right,
                        6, LayerMask.GetMask("Player", "Obstacles"));
                    if (raycast.transform && raycast.transform.gameObject.layer == LayerMask.NameToLayer("Player")
                    ) //if sees player shoot
                    {
                        Move(0);
                        if (allowFire)
                            StartCoroutine(Shoot());
                        return;
                    }
                    else if (!overcomingObstacle && playerDistance < 1.8) //if really close stop moving
                    {
                        Move(0);
                        return;
                    }

                    if (raycast.transform && raycast.transform.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
                    {
                        overcomingObstacle = true;
                        overcomingDistanceToStop = playerDistance;
                        overcomingRight = playerIsRight;
                        Move((overcomingRight ? 1 : -1) * runSpeed * 1.15f);
                        return;
                    }
                }

                if (overcomingObstacle)
                {
                    if (overcomingDistanceToStop <= playerDistance)
                        overcomingObstacle = false;
                    else
                        Move((overcomingRight ? 1 : -1) * runSpeed * 1.15f);
                }
                else
                {
                    //only reaches if x distance between 1.8 and 5 and can't see player
                    //or if x distance > 5
                    Move((playerIsRight ? 1 : -1) * runSpeed);
                }
            }
        }
    }

    private void Move(float move)
    {
        animator.SetFloat("HorizontalMove", Mathf.Abs(move));
        var oldVelocity = m_Rigidbody2D.velocity;
        Vector3 targetVelocity = new Vector2(move, oldVelocity.y);

        if (m_Grounded && Mathf.Abs(move) > 0.01 && !audioManager.IsSoundPlaying("Run"))
            audioManager.PlaySound("Run");
        else if (!m_Grounded || Mathf.Abs(move) < 0.01)
            audioManager.StopSound("Run");

        m_Rigidbody2D.velocity =
            Vector3.SmoothDamp(oldVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        if (m_Grounded && Math.Abs(m_Rigidbody2D.velocity.x) > 0.5 &&
            Math.Abs(oldVelocity.x) < Math.Abs(2 * m_Rigidbody2D.velocity.x / 3))
        {
            Jump();
        }
    }

    void Jump()
    {
        if (Math.Abs(m_Rigidbody2D.velocity.y) > 0.05) return;
        // Add a vertical force to the player.
        m_Grounded = false;

        audioManager.PlaySound("Jump");

        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
    }

    IEnumerator Shoot()
    {
        allowFire = false;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (colorIndex != -1)
        {
            bullet.GetComponent<SpriteRenderer>().color = colors[colorIndex];
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        bullet.layer = LayerMask.NameToLayer("EnemyBullets");
        rb.AddForce(firePoint.right * bulletForce, ForceMode2D.Impulse);

        audioManager.PlaySound("Gun Shot");

        yield return new WaitForSeconds(fireCooldown);

        allowFire = true;
    }

    private void Flip(bool right)
    {
        if (m_FacingRight && !right || !m_FacingRight && right)
        {
            gameObject.transform.Rotate(0f, 180, 0f);
            m_FacingRight = right;
        }
    }

    public void Damage(float damage, bool hitFront)
    {
        if (health <= 0)
            return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }

        if (hitFront)
            animator.SetTrigger("HitFront");
        else
            animator.SetTrigger("HitBack");

        switch (UnityEngine.Random.Range(0, 2))
        {
            case 0:
                audioManager.PlaySound("Damage1");
                break;
            default:
                audioManager.PlaySound("Damage2");
                break;
        }
    }

    private void CheckLevel()
    {
        float enemyY = transform.position.y;
        float playerY = player.position.y;
        if ((playerY > 4 && enemyY > 5) || //both on top
            (playerY < 5 && enemyY < 5)) //both on bottom
        {
            roaming = false;
        }
        else roaming = true;
    }

    private void Flee()
    {
        bool playerIsRight = player.position.x > gameObject.transform.position.x;

        RaycastHit2D raycast = Physics2D.Raycast(transform.position, (playerIsRight ? -1 : 1) * Vector2.right,
            seeingDistance, LayerMask.GetMask("Ground", "UpperGround"));

        if (raycast.collider == null)
        {
            fleeingDirection = playerIsRight ? -1 : 1;
        }
        else
        {
            fleeingDirection = playerIsRight ? 1 : -1;
        }

        Flip(fleeingDirection == 1);
    }

    private void StopFlee()
    {
        fleeingDirection = 0;
    }

    private void Die()
    {
        animator.SetTrigger("Lose");

        healthController.onHeal -= Flee;
        healthController.onStopHeal -= StopFlee;

        onEnemyDeath?.Invoke();
    }

    public bool getFacingRight()
    {
        return m_FacingRight;
    }
}