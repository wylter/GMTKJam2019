﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

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
    [SerializeField]
    private float m_walledDeceleration = 0.3f;

    [Space]
    [Header("Collision Settings")]
    [Space]
    [SerializeField]
    private LayerMask m_layermask = 0;
        [SerializeField]
    private SpriteRenderer m_headSprite = null;
    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip m_jumpClip = null;
    [SerializeField]
    private AudioClip m_bounceClip = null;

    [Header("Other")]
    [SerializeField]
    private SpriteRenderer m_sprite = null;
    [SerializeField]
    private Transform m_particleSystemSpawn = null;
    [SerializeField]
    private ParticleSystem m_particleSystem = null;

    private Animator m_playerAnimator;

    public Rigidbody2D m_rb;
    private Vector2 m_input;
    private bool m_jumping; //True if the player is olding the Jump button
    private bool m_lateJumping = false;
    //private float m_accellerationMultiplier = 1; //Current acceleration multiplier dependant if the player is running or not
    //private float m_groundTime = 0; //time we were last grounded


    private bool m_canJump = true;

    private float m_jumpTime = 0f;
    //private bool m_stop = false;

    private bool m_bouncing = false;

    private Collider2D m_collider = null;

    private bool m_isGrounded = false;

    private float m_slopeAngle = 0f;

    private bool walled = false;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_playerAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        //Inizialization

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
        bool grounded = isGrounded(); 
        //bool walled = (wallDir != 0); //True if the player is touching a wall

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

        Vector2 force = new Vector2();
        force.x = ((m_input.x * m_speed) - m_rb.velocity.x) * ((grounded && !walled) ? m_accelleration : m_airaccelleration) * (walled ? m_walledDeceleration : 1);
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

        //Debug.Log("Force: " + force.x + "\nVelocity: " + xVelocity);
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
            SoundController.instance.PlaySingle(m_jumpClip);

           ParticleSystem instance = Instantiate(m_particleSystem, m_particleSystemSpawn.position, m_particleSystemSpawn.rotation);
           if (m_rb.velocity.x < -0.2f)
           {
                var shape = instance.shape;
                shape.rotation = new Vector3(shape.rotation.x - 90f, shape.rotation.y, shape.rotation.z);
           }
            Destroy(instance, 1f);
        }
        if (!m_bouncing && Input.GetButtonUp("Jump"))
        {
            m_jumping = false;
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

            if (Mathf.Abs(angle) > 2f && Mathf.Abs(angle) < 60f)
            {
                m_slopeAngle = angle;
                walled = false;
            }
            else if (Mathf.Abs(angle) > 60f)
            {
                walled = true;
                m_slopeAngle = 0f;
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
            walled = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        m_slopeAngle = 0f;
        walled = false;
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
        m_jumpTime = Time.time;
        m_lateJumping = false;

        //m_playerAnimator.SetTrigger("LoseJump");
        m_playerAnimator.SetBool("Jumping", true);
        m_playerAnimator.SetTrigger("Jump");
        SoundController.instance.PlaySingle(m_bounceClip);

        m_bouncing = true;

    }

    public Vector2 Vector2FromAngle(float a)
     {
         a *= Mathf.Deg2Rad;
         return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
     }

}
