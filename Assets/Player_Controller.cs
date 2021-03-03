using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 10f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;

    public Animator animator;

    const float k_GroundedRadius = .2f;
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    public bool m_FacingRight = false;
    private bool m_Grounded;
    private bool jump = false;
    private float horizontalMove = 0f;
    public float runSpeed = 8f;

    public bool doubleSpeed = false;

    public bool inputBlocked = false;

    // Start is called before the first frame update
    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
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
        }

        animator.SetFloat("HorizontalMove", Mathf.Abs(horizontalMove));
    }

    private void FixedUpdate()
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

        animator.SetBool("IsJumping", !m_Grounded);

        Move(horizontalMove);
        jump = false;
    }

    private void Move(float move)
    {
        var velocity = m_Rigidbody2D.velocity;

        Vector3 targetVelocity = new Vector2(move, velocity.y);

        m_Rigidbody2D.velocity =
            Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        if (!m_Grounded || !jump) return;
        // Add a vertical force to the player.
        m_Grounded = false;
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
    }

    public void Flip()
    {
        m_FacingRight = !m_FacingRight;
        transform.Rotate(0f, 180, 0f);
    }
}