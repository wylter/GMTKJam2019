using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Object Used to check the groundstate of the player.
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
    private float speed = 14f;   //Walking Speed of Player
    [SerializeField]
    private float accelleration = 6f;    //accellerationeration of the player on the ground
    [SerializeField]
    private float airaccelleration = 3f; //accellerationeration of the player on the air
    [SerializeField]
    private float jump = 14f;    //Jump Speed
    [SerializeField]
    private float fallingMultiplayer = 2; //Gravity Multiplayer for gravity
    [SerializeField]
    private float runningMultiplier = 2; //How much the run effect the accellerationeration
    [SerializeField]
    private float wallJumpMultiplier = 0.75f;
    [SerializeField]
    private float coyoteTime = 0.1f;

    [Space]
    [Header("Collision Settings")]
    [SerializeField]
    private float offset = 0.1f; //Offset per cercare un muro in GroundState
    [SerializeField]
    private LayerMask neutralWallMask; //Cosa GroundState ritiene essere un muro da cui si può jumpare
    [SerializeField]
    private LayerMask blackWallMask;
    [SerializeField]
    private LayerMask whiteWallMask;
    [Space]
    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip jumpClip;

    private bool colorState = false; //False is white, True is Black //Btw we swapped the meaning of this, might be discrepancies in the comments.

    private Animator playerAnimator;
    private GroundState groundState;
    private Rigidbody2D rb;
    private Vector2 input;
    private bool jumping; //True if the player is olding the Jump button
    private bool changingButtonDown = false; // Variable necessary to make a trigger act like a Button. (Unity didnt allow the left trigger of the joypad to act as a button when chanigng, so this solution was necessary)
    private float accellerationMultiplier = 1; //Current acceleration multiplier dependant if the player is running or not
    private float groundTime = 0; //time we were last grounded

    private bool m_canJump = true;

    void Start()
    {
        //Inizialization
        groundState = new GroundState(transform, offset, neutralWallMask);
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();

        Input.ResetInputAxes();

        m_canJump = true;
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            HandleInput();
        }
    }

    void FixedUpdate()
    {
        int wallDir = groundState.wallDirection(); //Direction of the wall: 1 is right and -1 is left
        bool grounded = isGrounded(); //True if the player is touching the ground taking count of the cayote time
        bool walled = (wallDir != 0); //True if the player is touching a wall

        rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x) * ((grounded && !walled) ? accelleration : airaccelleration) * (walled ? 1 : accellerationMultiplier), (rb.velocity.y < -0.05 && !walled) ? Physics2D.gravity.y * (fallingMultiplayer - 1) : 0)); //Move player. If the player is falling, add more fall to the falling (depends on the falling Multiplaier)

        //Stop player if input.x is 0 (and grounded)
        //Jump if the player is pressing Jump (And grounded or walled)
        //If the player is pressing the jump key in air, it keeps jumping high. If the player stops holding the jump key, his air velocity gets reduced.
        rb.velocity = new Vector2((input.x == 0 && grounded) ? 0 : rb.velocity.x, (input.y == 1 && (grounded || walled)) ? jump : (jumping || rb.velocity.y < 0) ? rb.velocity.y : rb.velocity.y / 2);

//         if (walled && !grounded && input.y == 1)
//             rb.velocity = new Vector2(-wallDir * speed * wallJumpMultiplier, rb.velocity.y); //Add force negative to wall direction (with speed reduction)

        //Handels the Sound of Jumping
//         if (input.y == 1 && (walled || grounded))
//         {
//             SoundManager.soundManager.PlaySingle(jumpClip);
//         }

        input.y = 0;
    }

    //Input Reading
    void HandleInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && m_canJump)
        {
            input.y = 1;
            jumping = true;
            m_canJump = false;
        }
        if (Input.GetButtonUp("Jump"))
        {
            jumping = false;
        }

        //accellerationMultiplier = (Input.GetAxis("Run") > 0) ? runningMultiplier : 1;
    }

    bool isGrounded()
    {
        return true;

        if (groundState.isGround())
        { //True if the player is touching the ground)
            groundTime = Time.time;
            return true;
        }
        else if (Time.time < groundTime + coyoteTime)
        { //If the player touched the ground recently, he can still jump #coyoteTime
            return true;
        }
        else
        {
            return false;
        }

    }

}
