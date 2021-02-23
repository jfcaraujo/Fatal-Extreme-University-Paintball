using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 500f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private Transform m_Center;
    [SerializeField] private Transform m_FirePoint;

    public Animator animator;
    public Camera cam;

    const float k_GroundedRadius = .2f;
    private Rigidbody2D m_Rigidbody2D;
    public Transform gun;
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_FacingRight = false;
    private bool m_Grounded;
    private bool jump = false;
    private float horizontalMove = 0f;
    public float runSpeed = 40f;

    private Vector2 mousePos;

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

        animator.SetFloat("HorizontalMove", Mathf.Abs(horizontalMove));

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
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

        Move(horizontalMove * Time.fixedDeltaTime);
        Aim();
        jump = false;
    }

    private void Aim()
    {
        Vector2 center = new Vector2(m_Center.position.x, m_Center.position.y);

        Vector2 lookDir = mousePos - center;
        float lookAngle = -(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 180f);

        Vector2 gunDir = new Vector2(m_FirePoint.position.x, m_FirePoint.position.y) - center;
        float gunAngle = -(Mathf.Atan2(gunDir.y, gunDir.x) * Mathf.Rad2Deg - 180f);

        gun.RotateAround(m_Center.position, Vector3.back, lookAngle - gunAngle);

        if ((lookAngle < 85 || lookAngle > 275) && m_FacingRight)
        {
            Flip();
        }
        else if (lookAngle > 95 && lookAngle < 265 && !m_FacingRight)
        {
            Flip();
        }
    }

    private void Move(float move)
    {
        var velocity = m_Rigidbody2D.velocity;
        Vector3 targetVelocity = new Vector2(move * 10f, velocity.y);
        m_Rigidbody2D.velocity =
            Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

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