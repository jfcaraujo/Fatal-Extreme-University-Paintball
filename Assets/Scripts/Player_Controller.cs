using System.Collections;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 10f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;

    public Animator animator;

    const float k_GroundedRadius = .1f;
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    public bool m_FacingRight = false;
    private bool m_Grounded;
    private bool jump = false;
    private bool dropDown = false;
    private float horizontalMove = 0f;
    public float runSpeed = 8f;
    public float dropDownInterval = 0.25f;
    public float dropDownForce = 1f;

    public bool doubleSpeed = false;

    public bool inputBlocked = false;

    private Collider2D playerCollider;

    // Start is called before the first frame update
    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        playerCollider = gameObject.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (inputBlocked)
        {
            horizontalMove = 0;
        }
        else
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed * (doubleSpeed ? 2 : 1);
            if (Input.GetButtonDown("Jump"))
                jump = true;

            if (Input.GetButtonDown("Drop"))
                dropDown = true;
        }

        animator.SetFloat("HorizontalMove", Mathf.Abs(horizontalMove));
    }

    private void FixedUpdate()
    {
        m_Grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        foreach (Collider2D temp_collider in colliders)
        {
            if (temp_collider.gameObject != gameObject && !temp_collider.isTrigger)
            {
                m_Grounded = true;

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

    private void Move(float move)
    {
        var velocity = m_Rigidbody2D.velocity;

        Vector3 targetVelocity = new Vector2(move, velocity.y);

        m_Rigidbody2D.velocity =
            Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        if (!m_Grounded || !jump || m_Rigidbody2D.velocity.y > 0.05) return;
        // Add a vertical force to the player.
        m_Grounded = false;
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
    }

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

    public void Flip()
    {
        m_FacingRight = !m_FacingRight;
        transform.Rotate(0f, 180, 0f);
    }
}