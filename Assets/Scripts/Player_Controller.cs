using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Script to handle the player movement and interaction with the environment.
/// </summary>
public class Player_Controller : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 10f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;

    public AudioManager audioManager;
    public Animator animator;
    private Rigidbody2D m_Rigidbody2D;
    private Collider2D playerCollider;

    const float k_GroundedRadius = .1f;
    private Vector3 m_Velocity = Vector3.zero;
    public bool m_FacingRight = false;
    private bool m_Grounded;
    private bool jump = false;
    private bool dropDown = false;
    private float horizontalMove = 0f;
    public float runSpeed = 8f;
    public float dropDownInterval = 0.25f;
    public float dropDownForce = 1f;
    public bool speedUp = false;
    public bool inputBlocked = false;

    // Layer collisions asked to be toggled
    private Dictionary<string, bool> collisionsToToggle;

    private void Start()
    {
        collisionsToToggle = new Dictionary<string, bool>();

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        playerCollider = gameObject.GetComponent<Collider2D>();

        // Initially, disables collisions between the player and objects from the upper level
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("UpperGround"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("UpperObstacles"), true);
    }

    private void Update()
    {
        List<string> collisionsToRemove = new List<string>();

        foreach (string layer in collisionsToToggle.Keys)
        {
            // Will check if player is overlapping any object from the layer
            // before activating collisions
            //
            // If the player is overlapping, do not activate collision now
            int numColliders = 5;
            Collider2D[] colliders = new Collider2D[numColliders];
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask(layer));

            if (playerCollider.OverlapCollider(contactFilter, colliders) == 0)
            {
                collisionsToToggle.TryGetValue(layer, out bool value);
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer(layer), !value);
                collisionsToRemove.Add(layer);
            }
        }

        // Remove toggled collisions
        foreach (string layer in collisionsToRemove)
        {
            collisionsToToggle.Remove(layer);
        }

        // Get user input if it is not blocked
        if (inputBlocked)
        {
            horizontalMove = 0;
        }
        else
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed * (speedUp ? 1.5f : 1f);
            if (Input.GetButtonDown("Jump"))
                jump = true;

            if (Input.GetButtonDown("Drop"))
                dropDown = true;
        }

        animator.SetFloat("HorizontalMove", Mathf.Abs(horizontalMove));
    }

    private void FixedUpdate()
    {
        // Checks if player is touching the ground
        m_Grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        foreach (Collider2D temp_collider in colliders)
        {
            // Checks if collider and player are supposed to collide
            bool shouldCollide = !(Physics2D.GetIgnoreCollision(temp_collider, playerCollider)
                                    || Physics2D.GetIgnoreLayerCollision(temp_collider.gameObject.layer, gameObject.layer));

            if (temp_collider.gameObject != gameObject && !temp_collider.isTrigger && shouldCollide)
            {
                m_Grounded = true;

                // Drops down platform
                if (dropDown && temp_collider.gameObject.layer == LayerMask.NameToLayer("EnvironmentObjects"))
                {
                    StartCoroutine(DropDownPlatform(temp_collider));
                }
            }
        }

        animator.SetBool("IsJumping", !m_Grounded);

        Move(horizontalMove);
        jump = false;
        dropDown = false;
    }

    /// <summary>
    /// Moves player with the specified speed (from -runSpeed to runSpeed).
    /// </summary>
    /// <param name="move">Target horizontal velocity.</param>
    private void Move(float move)
    {
        var velocity = m_Rigidbody2D.velocity;

        // Run sound is only played if the player is moving and sound was not already playing
        if (m_Grounded && Mathf.Abs(move) > 0.01 && !audioManager.IsSoundPlaying("Run"))
            audioManager.PlaySound("Run");
        else if (!m_Grounded || Mathf.Abs(move) < 0.01)
            audioManager.StopSound("Run");

        Vector3 targetVelocity = new Vector2(move, velocity.y);

        // Smoothly change velocity to target velocity
        m_Rigidbody2D.velocity =
            Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        if (!m_Grounded || !jump || Mathf.Abs(m_Rigidbody2D.velocity.y) > 0.05) return;

        m_Grounded = false;

        audioManager.PlaySound("Jump");
        // Add a vertical force to the player.
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Disables collision between the player and for him to drop down.
    /// Waits a specific interval before reactivating collisions.
    /// </summary>
    /// <param name="platformCollider">Collider to be disabled.</param>
    private IEnumerator DropDownPlatform(Collider2D platformCollider)
    {
        // Disable collisions between player and platform
        Physics2D.IgnoreCollision(platformCollider, playerCollider, true);

        // Add downforce to drop down faster
        m_Rigidbody2D.AddForce(new Vector2(0, -dropDownForce), ForceMode2D.Impulse);

        yield return new WaitForSeconds(dropDownInterval);

        // Re-enable collisions between player and platform
        Physics2D.IgnoreCollision(platformCollider, playerCollider, false);
    }

    /// <summary>
    /// Used to request a layer collision toggle, that will be placed on a list
    /// and will only be toggled when the player is not currently overlapping any of that layer's colliders.
    /// </summary>
    /// <param name="layer">Layer for collisions to be toggled.</param>
    /// <param name="enable">If it should enable or disable.</param>
    public void ToggleCollisions(string layer, bool enable)
    {
        // If a request for the opposite is already in the list,
        // simply remove the previous request
        if (collisionsToToggle.TryGetValue(layer, out bool value))
        {
            if (value != enable)
                collisionsToToggle.Remove(layer);

            return;
        }

        // Only add to list if it isn't already as asked
        if (Physics2D.GetIgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer(layer)) == enable)
        {
            collisionsToToggle.Add(layer, enable);
        }
    }

    /// <summary>
    /// Flips the enemy to the other direction.
    /// </summary>
    public void Flip()
    {
        m_FacingRight = !m_FacingRight;
        transform.Rotate(0f, 180, 0f);
    }
}