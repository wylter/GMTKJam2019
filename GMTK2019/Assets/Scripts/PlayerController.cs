using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [System.Serializable]
    public class GroundState
    {

        private Transform playerTransform;
        private float width;
        private float height;
        private float length;
        private float realHeight;
        private float realWidth;


        private LayerMask wallMask;

        //GroundState constructor.  Sets offsets for raycasting.
        public GroundState(Transform playerRef, float offset, LayerMask wallMask)
        {

            this.wallMask = wallMask;

            playerTransform = playerRef.transform;
            Vector2 playerExtents = playerTransform.GetComponent<Collider2D>().bounds.extents;

            realWidth = playerExtents.x;
            realHeight = playerExtents.y;
            width = realWidth + offset;
            height = realHeight + offset * 2;
            length = 0.05f;
        }

        public void setWallMask(LayerMask wallMask)
        {
            this.wallMask = wallMask;
        }

        //Returns whether or not player is touching ground.
        public bool isGround()
        {
            bool bottom1 = Physics2D.Raycast(new Vector2(playerTransform.position.x, playerTransform.position.y - height), Vector2.down, length, wallMask);
            bool bottom2 = Physics2D.Raycast(new Vector2(playerTransform.position.x + realWidth, playerTransform.position.y - height), Vector2.down, length, wallMask);
            bool bottom3 = Physics2D.Raycast(new Vector2(playerTransform.position.x - realWidth, playerTransform.position.y - height), Vector2.down, length, wallMask);
            if (bottom1 || bottom2 || bottom3)
                return true;
            else
                return false;
        }

        //Returns direction of wall.
        public int wallDirection()
        {
            bool left1 = Physics2D.Raycast(new Vector2(playerTransform.position.x - width, playerTransform.position.y), Vector2.left, length, wallMask);
            bool left2 = Physics2D.Raycast(new Vector2(playerTransform.position.x - width, playerTransform.position.y + realHeight), Vector2.left, length, wallMask);
            bool left3 = Physics2D.Raycast(new Vector2(playerTransform.position.x - width, playerTransform.position.y - realHeight), Vector2.left, length, wallMask);
            bool right1 = Physics2D.Raycast(new Vector2(playerTransform.position.x + width, playerTransform.position.y), Vector2.right, length, wallMask);
            bool right2 = Physics2D.Raycast(new Vector2(playerTransform.position.x + width, playerTransform.position.y + realHeight), Vector2.right, length, wallMask);
            bool right3 = Physics2D.Raycast(new Vector2(playerTransform.position.x + width, playerTransform.position.y - realHeight), Vector2.right, length, wallMask);

            if (left1 || left2 || left3)
                return -1;
            else if (right1 || right2 || right3)
                return 1;
            else
                return 0;
        }
    }

    [Header("Movement Settings")]
    [SerializeField]
    private float m_speed = 14f;   //Walking Speed of Player
    [SerializeField]
    private float m_accelleration = 6f;    //accellerationeration of the player on the ground
    [SerializeField]
    private float m_airaccelleration = 3f; //accellerationeration of the player on the air
    [SerializeField]
    private float m_jump = 14f;    //Jump Speed
    [SerializeField]
    private float m_fallingMultiplayer = 2; //Gravity Multiplayer for gravity
//     [SerializeField]
//     private float m_runningMultiplier = 2; //How much the run effect the accellerationeration
//     [SerializeField]
//     private float m_wallJumpMultiplier = 0.75f;
//     [SerializeField]
//     private float m_coyoteTime = 0.1f;
    [SerializeField]
    private float m_jumpDuration = 0.1f;
    [SerializeField]
    private float m_minJumpDuration = 0.1f;
    [SerializeField]
    private float m_lateJumpDuration = 0.1f;
    [SerializeField]
    private float m_jumpingForce = 5f;
    [SerializeField]
    private float m_lateJumpForce = 5f;
    [SerializeField]
    private float m_gravitySpeedTrashhold = -1f;
    [SerializeField]
    private float m_friction = 2f;

    [Space]
    [Header("Collision Settings")]
    [SerializeField]
    private float m_offset = 0.1f; //Offset per cercare un muro in GroundState
    [Space]
    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip m_jumpClip;
    [SerializeField]
    private LayerMask m_layermask = 0;

    private Animator m_playerAnimator;
    private SpriteRenderer m_sprite;
    [SerializeField]
    private SpriteRenderer m_headSprite = null;
    private GroundState m_groundState;
    public Rigidbody2D m_rb;
    private Vector2 m_input;
    private bool m_jumping; //True if the player is olding the Jump button
    private bool m_lateJumping = false;
    private float m_accellerationMultiplier = 1; //Current acceleration multiplier dependant if the player is running or not
    private float m_groundTime = 0; //time we were last grounded

    private bool m_canJump = true;

    private float m_jumpTime = 0f;
    //private bool m_stop = false;

    private bool m_bouncing = false;

    private Collider2D m_collider = null;

    private bool m_isGrounded = false;

    private float m_slopeAngle = 0f;


    void Start()
    {
        //Inizialization
        m_groundState = new GroundState(transform, m_offset, m_layermask);
        m_rb = GetComponent<Rigidbody2D>();
        m_playerAnimator = GetComponent<Animator>();
        m_sprite = GetComponent<SpriteRenderer>();

        Input.ResetInputAxes();

        m_canJump = true;

        m_collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            HandleInput();
        }

        bool isMoving = m_rb.velocity.x < -0.2f || m_rb.velocity.x > 0.2f;

        if (isMoving)
        {
            m_sprite.flipX = m_rb.velocity.x < 0f;
            m_headSprite.flipX = m_rb.velocity.x < 0f;
        }

        m_playerAnimator.SetBool("Running", isMoving);

        if (m_jumpTime + m_minJumpDuration > Time.time)
        {
            m_playerAnimator.SetBool("Jumping", true);
            m_isGrounded = false;
        }
        else
        {
            m_playerAnimator.SetBool("Jumping", !isGrounded());
        }

        

    }

    void FixedUpdate()
    {
        int wallDir = m_groundState.wallDirection(); //Direction of the wall: 1 is right and -1 is left
        bool grounded = isGrounded(); 
        bool walled = (wallDir != 0); //True if the player is touching a wall

        Vector2 force = new Vector2();
        force.x = ((m_input.x * m_speed) - m_rb.velocity.x) * ((grounded && !walled) ? m_accelleration : m_airaccelleration); //* (walled ? 1 : m_accellerationMultiplier);
        force.y = 0f;
        if (m_rb.velocity.y < m_gravitySpeedTrashhold)
        {
            if (!walled)
            {
                force.y = Physics2D.gravity.y * (m_fallingMultiplayer - 1);
            }
        }
        else if (m_jumping)
        {
            force.y = m_jumpingForce;
        }
        else if (m_lateJumping)
        {
            force.y = m_lateJumpForce;
        }
        else if (Mathf.Abs(m_slopeAngle) > 0.5f)
        {
            float angle = m_slopeAngle;
            if (force.x < 0f)
            {
                angle = m_slopeAngle + 180f;
            }


            force = Vector2FromAngle(angle).normalized * force.magnitude * 4;

            force.y += -Physics2D.gravity.y;

            Debug.DrawLine(transform.position, transform.position + new Vector3(force.x, force.y, 0f), Color.red);
            Debug.Log("Slope");
            
        }


        m_rb.AddForce(force); //Move player. If the player is falling, add more fall to the falling (depends on the falling Multiplaier)

        //Stop player if input.x is 0 (and grounded)
        //Jump if the player is pressing Jump (And grounded or walled)
        //If the player is pressing the jump key in air, it keeps jumping high. If the player stops holding the jump key, his air velocity gets reduced.
        float xVelocity = (m_input.x == 0 && grounded) ? m_rb.velocity.x/m_friction : m_rb.velocity.x;
        float yVelocity = (m_input.y == 1) ? m_jump : (m_jumping || m_rb.velocity.y < 0) ? m_rb.velocity.y : m_rb.velocity.y / 2;
        m_rb.velocity = new Vector2(xVelocity, yVelocity);

//         if (walled && !grounded && input.y == 1)
//             rb.velocity = new Vector2(-wallDir * speed * wallJumpMultiplier, rb.velocity.y); //Add force negative to wall direction (with speed reduction)

        //Handels the Sound of Jumping
//         if (input.y == 1 && (walled || grounded))
//         {
//             SoundManager.soundManager.PlaySingle(jumpClip);
//         }

        m_input.y = 0;

        Debug.Log("Force: " + force.x + "\nVelocity: " + xVelocity);
    }

    //Input Reading
    void HandleInput()
    {
        float previousX = m_input.x;
        m_input.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && m_canJump)
        {
            m_input.y = 1;
            m_jumping = true;
            m_canJump = false;
            m_jumpTime = Time.time;
            m_lateJumping = false;

            m_playerAnimator.SetTrigger("LoseJump");
            m_playerAnimator.SetBool("Jumping", true);
            m_playerAnimator.SetTrigger("Jump");
        }
        if (!m_bouncing && Input.GetButtonUp("Jump"))
        {
            m_jumping = false;
        }
        if (m_jumping && m_jumpTime + m_jumpDuration < Time.time)
        {
            m_jumping = false;
            m_lateJumping = true;
        }
        if (m_jumpTime + m_jumpDuration + m_lateJumpDuration < Time.time)
        {
            m_lateJumping = false;
            m_bouncing = false;
        }

        //accellerationMultiplier = (Input.GetAxis("Run") > 0) ? runningMultiplier : 1;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[1];

        bool isInsideLayerMask = m_layermask == (m_layermask | (1 << collision.collider.gameObject.layer));
        if (isInsideLayerMask && collision.GetContacts(contacts) > 0)
        {
            var contactNormal = contacts[0].normal;

            float angle = Vector2.SignedAngle(Vector2.up, contactNormal);
            Debug.Log(angle);

            m_isGrounded = Mathf.Abs(angle) > -1f && Mathf.Abs(angle) < 60f;

            if (Mathf.Abs(angle) > 10f && Mathf.Abs(angle) < 60f)
            {
                m_slopeAngle = angle;
            }
            else
            {
                m_slopeAngle = 0f;
            }
        }
        else
        {
            m_isGrounded = false;
            m_slopeAngle = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        m_slopeAngle = 0f;
    }

    bool isGrounded()
    {
        return m_isGrounded;

//         if (m_groundState.isGround())
//         { //True if the player is touching the ground)
//             m_groundTime = Time.time;
//             return true;
//         }
// //         else if (Time.time < m_groundTime + m_coyoteTime)
// //         { //If the player touched the ground recently, he can still jump #coyoteTime
// //             return true;
// //         }
//         else
//         {
//             return false;
//         }

    }

    public void RecoverJump()
    {
        m_canJump = true;
        m_playerAnimator.SetTrigger("GainJump");
    }

    public void Bounce()
    {
        m_input.y = 1;
        m_jumping = true;
        m_canJump = false;
        m_jumpTime = Time.time;
        m_lateJumping = false;

        //m_playerAnimator.SetTrigger("LoseJump");
        m_playerAnimator.SetBool("Jumping", true);
        m_playerAnimator.SetTrigger("Jump");

        m_bouncing = true;

    }

    public Vector2 Vector2FromAngle(float a)
     {
         a *= Mathf.Deg2Rad;
         return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
     }

}
