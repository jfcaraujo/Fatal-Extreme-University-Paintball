using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to handle the enemies.
/// </summary>
public class Enemy_Controller : MonoBehaviour
{
    /// List of colors to assign to the fired pellets
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
    private Rigidbody2D m_Rigidbody2D;
    private Collider2D m_Collider2D;
    public GameObject bulletPrefab;
    public GameObject grenadePrefab;
    private Transform player;
    public Transform firePoint;
    public WeaponAmmo ammoDrop;
    private HealthController playersHealthController;
    public GameObject[] Droppables;

    const float k_GroundedRadius = .2f;
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_FacingRight = true;
    private bool m_Grounded;
    public float runSpeed = 8f;
    public float bulletForce = 20f;

    public float fireCooldown = 0.4f; // Fire cooldown in seconds
    public float grenadeCooldown = 1f; // Grenade cooldown in seconds
    [SerializeField] private float health = 1;
    private bool allowFire = true;
    private bool allowGrenade = true;
    public bool inputBlocked = false;

    // fleeing: 0 = not fleeing; 1 = fleeing right; -1 = fleeing left
    private int fleeingDirection = 0;
    private bool roaming;
    private int roamingDirection = 1;
    private int maxRoamingDistance = 30;

    private bool overcomingObstacle = false;
    private float overcomingDistanceToStop = 0;
    private bool overcomingRight = false;

    public float sightDistance = 7.5f;

    public delegate void OnEnemyDeath();
    public event OnEnemyDeath onEnemyDeath;

    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Collider2D = GetComponent<Collider2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
        playersHealthController = playerObject.GetComponent<HealthController>();

        // Subscribe to player health events
        // Enemy will flee when player is healing
        playersHealthController.onHeal += Flee;
        playersHealthController.onStopHeal += StopFlee;

        // Select color of the pellet for this enemy
        if (colors.Count > 0)
        {
            // Use instance ID as an unique seed
            System.Random rnd = new System.Random(gameObject.GetInstanceID());
            colorIndex = rnd.Next(colors.Count);
        }

        if (MainMenu.difficulty == 1) maxRoamingDistance = 50;
        if (MainMenu.difficulty == 2) maxRoamingDistance = 40;
    }

    void FixedUpdate()
    {
        // Checks if enemy is touching the ground
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
        float playerDistanceAbs = Math.Abs(playerDistance);

        if (inputBlocked) //if dying
        {
            Move(0);
            return;
        }

        // Check for obstacle in front, so it jumps before
        RaycastHit2D raycastObstacle = Physics2D.Raycast(
                                    new Vector2(transform.position.x, transform.position.y),
                                    (m_FacingRight ? 1 : -1) * Vector2.right,
                                    sightDistance / 2, LayerMask.GetMask("Obstacles", "UpperObstacles"));

        if (fleeingDirection != 0) //if fleeing
        {
            if (raycastObstacle.transform)
            {
                Jump();
            }

            Move(fleeingDirection * runSpeed);
        }
        else //if not fleeing
        {
            CheckLevel();
            if (roaming)
            {
                Flip(roamingDirection == 1);

                if (allowGrenade)
                {
                    // If enemy can throw grenade, search for player under/above
                    RaycastHit2D raycastPlayer = Physics2D.Raycast(transform.position, (IsOnUpperLevel() ? -1 : 1) * Vector2.up,
                    Mathf.Infinity, LayerMask.GetMask("Player"));

                    if (raycastPlayer.collider)
                    {
                        // If player is found, throw grenade in their direction
                        GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
                        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
                        rb.angularVelocity = 100f;
                        rb.gravityScale = 1.5f;

                        rb.velocity = new Vector2(0, IsOnUpperLevel() ? -1 : 15);

                        Collider2D cl = grenade.GetComponent<Collider2D>();

                        // Disable collisions between this enemy and the grenade
                        Physics2D.IgnoreCollision(m_Collider2D, cl);

                        // Start cooldown counter to limit grenade throwing
                        allowGrenade = false;
                        StartCoroutine(GrenadeCooldown());
                    }
                }

                // Checks for walls in the roaming direction
                RaycastHit2D raycast = Physics2D.Raycast(transform.position,
                    roamingDirection * Vector2.right,
                    2, LayerMask.GetMask("Ground", "UpperGround"));

                // if near a wall or too far from enemy, turn back
                if (raycast.collider || -playerDistance * roamingDirection > maxRoamingDistance)
                {
                    roamingDirection *= -1;
                }

                if (raycastObstacle.transform)
                {
                    Jump();
                }

                Move(roamingDirection * runSpeed);

            }
            else //chasing player if is in same level
            {
                Flip(playerIsRight); //flip in the direction of the player

                if (playerDistanceAbs <= sightDistance) //if close
                {
                    Jump();

                    //check if sees player
                    RaycastHit2D raycast = Physics2D.Raycast(
                        new Vector2(transform.position.x, transform.position.y - 0.144f),
                        (playerIsRight ? 1 : -1) * Vector2.right,
                        sightDistance + 1, LayerMask.GetMask("Player", "Obstacles", "UpperObstacles"));

                    //if sees player shoot
                    if (raycast.transform
                        && raycast.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        Move(0);
                        if (allowFire)
                            StartCoroutine(Shoot((playerDistance / sightDistance) * 0.14f, (playerDistance / sightDistance) * 1f));
                        return;
                    }
                    else if (!overcomingObstacle && playerDistanceAbs < 1.8) //if really close stop moving
                    {
                        Move(0);
                        return;
                    }

                    // If it sees an obstacle, it will start overcoming it
                    if (raycast.transform && raycast.transform.gameObject.layer != LayerMask.NameToLayer("Player"))
                    {
                        overcomingObstacle = true;
                        overcomingDistanceToStop = playerDistanceAbs;
                        overcomingRight = playerIsRight;
                        Move((overcomingRight ? 1 : -1) * runSpeed * 1.15f);
                        return;
                    }
                }

                // If it is overcoming an obstacle, it will travel a specified distance
                // to get behind the player and flank them
                if (overcomingObstacle)
                {
                    // If distance has been cleared, return to normal behaviour
                    if (overcomingDistanceToStop <= playerDistanceAbs)
                        overcomingObstacle = false;
                    else
                        Move((overcomingRight ? 1 : -1) * runSpeed * 1.15f);
                }
                else
                {
                    // if x distance > sightDistance && x distance > 1.8 && not seeing any object
                    Move((playerIsRight ? 1 : -1) * runSpeed);
                }
            }
        }
    }

    /// <summary>
    /// Moves enemy with the specified speed (from -runSpeed to runSpeed).
    /// </summary>
    /// <param name="move">Target horizontal velocity.</param>
    private void Move(float move)
    {
        animator.SetFloat("HorizontalMove", Mathf.Abs(move));

        var oldVelocity = m_Rigidbody2D.velocity;
        Vector3 targetVelocity = new Vector2(move, oldVelocity.y);

        // Run sound is only played if the enemy is moving and sound was not already playing
        if (m_Grounded && Mathf.Abs(move) > 0.01 && !audioManager.IsSoundPlaying("Run"))
            audioManager.PlaySound("Run");
        else if (!m_Grounded || Mathf.Abs(move) < 0.01)
            audioManager.StopSound("Run");

        // Smoothly change velocity to target velocity
        m_Rigidbody2D.velocity =
            Vector3.SmoothDamp(oldVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        // The enemy jumps if the current velocity is much higher than the old velocity
        if (Math.Abs(m_Rigidbody2D.velocity.x) > 0.5 &&
            Math.Abs(oldVelocity.x) < Math.Abs(2 * m_Rigidbody2D.velocity.x / 3))
        {
            Jump();
        }
    }

    /// <summary>
    /// Makes the enemy jump. Only works if enemy is grounded.
    /// </summary>
    void Jump()
    {
        if (!m_Grounded)
            return;

        // Makes sure enemy is not still moving vertically
        if (Math.Abs(m_Rigidbody2D.velocity.y) > 0.05) return;

        m_Grounded = false;

        audioManager.PlaySound("Jump");

        // Add a vertical force to enemy
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Shoots the enemy weapon.
    /// </summary>
    /// <param name="delay">Number of seconds before shooting.</param>
    /// <param name="innacuracy">Probability of innacuracy of the shot.</param>
    IEnumerator Shoot(float delay, float innacuracy)
    {
        allowFire = false;

        // Delay before shooting
        yield return new WaitForSeconds(delay);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (colorIndex != -1)
        {
            // Change color of pellet to previously defined
            bullet.GetComponent<SpriteRenderer>().color = colors[colorIndex];
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        bullet.layer = LayerMask.NameToLayer("EnemyBullets");

        // Gets random innacuracy value to change the bullets trajectory
        // (it probably won't come out straight from the barrel)
        float randomInnacuracy = UnityEngine.Random.Range(-innacuracy, innacuracy);

        rb.AddForce(new Vector2(firePoint.right.x * bulletForce, randomInnacuracy * 5), ForceMode2D.Impulse);

        audioManager.PlaySound("Gun Shot");

        // Starts shooting cooldown to limit firing
        yield return new WaitForSeconds(fireCooldown);

        allowFire = true;
    }

    /// <summary>
    /// Starts grenade cooldown.
    /// </summary>
    IEnumerator GrenadeCooldown()
    {
        yield return new WaitForSeconds(grenadeCooldown);

        allowGrenade = true;
    }

    /// <summary>
    /// Flips the enemy to the specified direction.
    /// </summary>
    /// <param name="right">If the new direction is right.</param>
    private void Flip(bool right)
    {
        if (m_FacingRight && !right || !m_FacingRight && right)
        {
            gameObject.transform.Rotate(0f, 180, 0f);
            m_FacingRight = right;
        }
    }

    /// <summary>
    /// Deal damage to the enemy.
    /// </summary>
    /// <param name="damage">Amount of damage to be dealt.</param>
    /// <param name="hitFront">If the enemy was hit from the front.</param>
    public void Damage(float damage, bool hitFront)
    {
        // If the enemy is already dead, exit
        if (health <= 0)
            return;

        health -= damage;

        // If no health, dies
        if (health == 0)
            Die();

        if (hitFront)
            animator.SetTrigger("HitFront");
        else
            animator.SetTrigger("HitBack");

        // Damage sound is randomly picked
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

    /// <summary>
    /// Checks if enemy is on upper level (y above 5).
    /// </summary>
    /// <returns>If enemy is on upper level.</returns>
    private bool IsOnUpperLevel()
    {
        return transform.position.y > 5;
    }

    /// <summary>
    /// Sets roaming variable according to enemy level position relative to player.
    /// </summary>
    private void CheckLevel()
    {
        float playerY = player.position.y;
        if ((playerY > 5 && IsOnUpperLevel()) || //both on top
            (playerY < 5 && !IsOnUpperLevel())) //both on bottom
        {
            roaming = false;
        }
        else roaming = true;
    }

    /// <summary>
    /// Runs away from the player.
    /// </summary>
    private void Flee()
    {
        bool playerIsRight = player.position.x > gameObject.transform.position.x;

        // Checks if there is a wall nearby, and if so, it runs in the other direction
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, (playerIsRight ? -1 : 1) * Vector2.right,
            15f, LayerMask.GetMask("Ground", "UpperGround"));

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

    /// <summary>
    /// Returns enemy to normal behaviour.
    /// </summary>
    private void StopFlee()
    {
        fleeingDirection = 0;
    }

    /// <summary>
    /// Triggers lose animation and unsubcribes events.
    /// </summary>
    private void Die()
    {
        animator.SetTrigger("Lose");

        playersHealthController.onHeal -= Flee;
        playersHealthController.onStopHeal -= StopFlee;

        onEnemyDeath?.Invoke();
    }

    /// <summary>
    /// Checks if enemy is facing right.
    /// </summary>
    /// <returns>If enemy is facing right</returns>
    public bool getFacingRight()
    {
        return m_FacingRight;
    }
}