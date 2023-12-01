using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class PlayerController_NewInput : MonoBehaviour
{

    private Rigidbody2D playerRB;
    private SpriteRenderer Sprite;
    private Animator animator;
    private GrappleHook Grapple;
    private GameLogic GameLogic;

    [Header("Horizontal movement settings")]
    [SerializeField] private float xSpeed = 1f;
    [SerializeField] private float runSpeedMultiplier = 2f; //shift to run
    [SerializeField] private float walkSpeedMultiplier = 0.5f; //ctrl to walk
    [SerializeField] private float swingSpeedMultiplier = 1.5f; //movement sensitivity while grappling
    [SerializeField] private float airSpeedMultiplier = 0.75f; //movement sensitivity while in the air
    [SerializeField] private float acceleration = 100f; //acceleration rate
    [SerializeField] private float accelerationAir = 50; //acceleration rate in the air
    [SerializeField] private float decelleration = 100f; //decelleration rate
    [SerializeField] private float decellerationAir = 50f; //decelleration rate in the air
    /*
    [SerializeField] private float fallAccel = 1f; //gravity
    [SerializeField] private float grapplefallAccel = 1f; //gravity while grappling
    */
    [Header("Jump settings")]
    [SerializeField] private float jumpForce = 1f; //impulse jump force
    [SerializeField] private float holdjumpForce = 5f; //force to apply while holding jump button -- variable jump height
    [SerializeField] private float fallGravityMultiplier = 1.5f; //gravity
    [SerializeField] private float grappleGravityMultiplier = 0.8f; //gravity while grappling
    [SerializeField] private float CoyoteTimeLength = 0.1f;
    private float timeBetweenJumps = 0.1f;
    /*
    [SerializeField] private float rampupTime = 0.5f; //movement acceleration time
    [SerializeField] private float rampdownTime = 0.5f; //movement decelleration time
    [SerializeField] private float floatyrampFactor = 4f; //acceleration time while in the air
    */

    [Header("Debugging")]
    [SerializeField] private bool IsGrounded = false;
    [SerializeField] private bool Jumping = false;

    private float rampuptimer = 0f;
    private float rampdowntimer = 0f;
    private float timesinceJump = 0;
    private float timesinceGround = 0;
    private int RaycastMask = ~(1 << 2);
    private int groundLayer = 1 << 7;

    private float Vert = 0;
    private float Horiz = 0;
    private bool run = false;
    private bool walk = false;
    private int spacePress = 0;
    private bool wDown = false;
    private bool spaceDown = false;
    private int wPress = 0;
    private int sPress = 0;
    private int aPress = 0;
    private int dPress = 0;
    private float JumpPress = 0;
    private float playerGravityScale;
    private Vector2 CharPos;

    //private RaycastHit2D FloorCheck;
    private Collider2D FloorCheck;


    //[SerializeField] private float  = 1f;


    private float moveVal;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = this.GetComponent<Rigidbody2D>();
        playerGravityScale = playerRB.gravityScale;
        Sprite = this.GetComponent<SpriteRenderer>();
        animator = this.GetComponent<Animator>();
        Grapple = this.GetComponent<GrappleHook>();
        GameLogic = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogic>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        

        if (FloorCheck != null)
        {
            ResetJump();
            IsGrounded = true;
            timesinceGround = 0;
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
        //if (FloorCheck.collider != null)
        if (FloorCheck!= null)
        {
            if (IsGrounded)
            {
                return;
            }
            else
            {
                ResetJump();
                IsGrounded = true;
                timesinceGround = 0;
            }
        }
        else if (FloorCheck == null)
        {
            IsGrounded = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        IsGrounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameLogic.Paused)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                run = true;
            }
            else
            {
                run = false;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                walk = true;
            }
            else
            {
                walk = false;
            }



 

            Horiz = Input.GetAxisRaw("Horizontal");
            Vert = Input.GetAxisRaw("Vertical");
            JumpPress = Input.GetAxisRaw("Jump");

            if (!IsGrounded)
            {
                timesinceGround += Time.deltaTime;
            }
            if(Jumping)
            {
                timesinceJump += Time.deltaTime;
            }
            animator.SetFloat("Speed", Mathf.Abs(playerRB.velocity.x));
        }

    }
    void FixedUpdate()
    {
        if (!GameLogic.Paused)
        {
            CharPos = playerRB.position;

            if (Grapple.isGrappling)
            {
                Vector2 PointinWorld = Grapple.CurrentBody.transform.TransformPoint(Grapple.CurrentPoint); //current grapple point, in world coords
                Vector2 ChartoPoint = (PointinWorld - CharPos).normalized;
                if (ChartoPoint.y < 0)
                {
                    Vert *= -1; //makes it so down button contracts rope when player is above grapple point
                }
            }

            //FloorCheck = Physics2D.Raycast(CharPos, Vector2.down, 3f, groundLayer);
            FloorCheck = Physics2D.OverlapBox(CharPos + 2.5f * Vector2.down, new(1.5f, 0.1f), 0, groundLayer);

            //Move(Horiz, Vert);

            Debug.Log(moveVal);

            MoveWithForces(moveVal);
            /*
            float jumpInput = JumpPress > 0 ? JumpPress : Vert;

            if (timesinceGround <= CoyoteTimeLength && !Jumping) //jumping while grappling is bugged -- will have to fix
            {
                Jump(jumpInput);
            }
            */

            GravityHandler();
        }



    }

    void GravityHandler()
    {

        if (IsGrounded && playerRB.gravityScale != playerGravityScale)
        {
            playerRB.gravityScale = playerGravityScale;
        }
        else if (!IsGrounded && Grapple.isGrappling) //if grappling, mark jumping as false and apply the grapple gravity
        {
            if (Jumping)
            {
                ResetJump();
            }
            playerRB.gravityScale = playerGravityScale * grappleGravityMultiplier;
        }
        else if (playerRB.velocity.y < -0.01) //make gravity stronger while falling
        {
            playerRB.gravityScale = playerGravityScale * fallGravityMultiplier;
        }

        if (Jumping && (Vert > 0 || JumpPress > 0) && playerRB.velocity.y > 0.01)
        {
            Vector2 JumpForce = new(0, holdjumpForce);
            playerRB.AddForce(JumpForce, ForceMode2D.Force); //apply additional upward force to increase jump height when holding jump button
        }
    }

    void MoveWithForces(float inputx)
    { 
        if (Grapple.isGrappling)
        {
            inputx *= swingSpeedMultiplier;
        }
        else if (!IsGrounded)
        {
            inputx *= airSpeedMultiplier;
        }
        else if (run && IsGrounded)
        {
            inputx *= runSpeedMultiplier;
        }
        else if (walk && IsGrounded)
        {
            inputx *= walkSpeedMultiplier;
        }
        

        float targetspeed = inputx * xSpeed;
        float deltaspeed = targetspeed - playerRB.velocity.x;

        float accelRate;
       
        if (targetspeed > 0)
        {
            this.transform.rotation = Quaternion.identity;
        }
        else if(targetspeed < 0)
        {
            if(this.transform.rotation.y == 180f)
            {
                return;
            }
            else
            {
                this.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            
        }    

        if (IsGrounded)
        {
            accelRate = (Mathf.Abs(targetspeed) > 1f) ? acceleration : decelleration; // condition ? if true : else
        }
        else
        {
            if (Mathf.Abs(playerRB.velocity.x) >= Mathf.Abs(targetspeed) && Mathf.Sign(targetspeed) == Mathf.Sign(playerRB.velocity.x))
            {
                accelRate = 0;
            }
            else if(Mathf.Abs(targetspeed) < 1f && Mathf.Abs(playerRB.velocity.x) < 0.01f)
            {
                accelRate = 0;
            }
            else
            {
                accelRate = ((Mathf.Abs(targetspeed) > 1f) ? accelerationAir : decellerationAir);
            }
            
        }

        float movement = deltaspeed * accelRate;

        playerRB.AddForce(movement * Vector2.right);
    }
    void Jump(float inputy)
    {
        if (inputy > 0)
        {
            Vector2 JumpForce = new(0, jumpForce);
            playerRB.AddForce(JumpForce, ForceMode2D.Impulse);
            Jumping = true;
            animator.SetBool("Jumping", Jumping);
        }
    }
    private void ResetJump()
    {
        if(timesinceJump > timeBetweenJumps)
        {
            timesinceJump = 0;
            Jumping = false;
            animator.SetBool("Jumping", Jumping);
        }
    }

    void OnJump()
    {
        if (timesinceGround <= CoyoteTimeLength && !Jumping) //jumping while grappling is bugged -- will have to fix
        {
            Jump(1);
        }
    }

    void OnMove(InputValue value)
    {
        moveVal = value.Get<float>();
    }
}

