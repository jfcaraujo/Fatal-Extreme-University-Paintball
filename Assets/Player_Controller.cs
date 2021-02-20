using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 500f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;

    const float k_GroundedRadius = .2f;
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_FacingRight = false;
    private bool m_Grounded;
    private bool jump = false;
    private float horizontalMove = 0f;
    public float runSpeed = 40f;

    // Start is called before the first frame update
    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump"))
            jump = true;
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

        Move(horizontalMove * Time.fixedDeltaTime);
        jump = false;
    }

    private void Move(float move)
    {
        var velocity = m_Rigidbody2D.velocity;
        Vector3 targetVelocity = new Vector2(move * 10f, velocity.y);
        m_Rigidbody2D.velocity =
            Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        if (move > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (move < 0 && m_FacingRight)
        {
            Flip();
        }

        if (!m_Grounded || !jump) return;
        // Add a vertical force to the player.
        m_Grounded = false;
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        transform.Rotate(0f, 180, 0f);
    }
}