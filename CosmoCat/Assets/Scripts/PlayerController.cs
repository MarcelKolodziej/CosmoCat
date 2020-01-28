using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float movementInputDirection;

    private int amountOfJumpsLeft;

    public float movementSpeed = 10;
    public float jumpForce = 16;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlidingSpeed;
    public float movementForceInAir;
    public float airDragMultiplayer = 0.9f;
    public float variableJumpHeightMultiplayer = 0.5f;


    public int amountOfJumps = 1;

    private Rigidbody2D rb;
    private Animator anim;

    private bool IsFacingRight = true;
    private bool isWalking; 
    private bool isGround;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;
    
    public Transform wallCheck;
    public Transform groundCheck;

    public LayerMask whatIsGround;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
    }

    private void FixedUpdate() 
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGround);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckSurroundings()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

    }


    private void CheckMovementDirection()
    {
        if (IsFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!IsFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if(rb.velocity.x != 0)
        {
            isWalking = true;
        }
        else 
        {
            isWalking = false;
        }
    }




    private void CheckIfCanJump() {
        if(isGround && rb.velocity.y <= 0)
        {
           amountOfJumpsLeft = amountOfJumps;
        } 
        
        if(amountOfJumpsLeft <= 0)
        {
            canJump = false;
        }
        else 
        {
            canJump = true;
        }
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGround && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }


    private void CheckInput() 
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if(Input.GetButtonUp("Jump"))
        {
          rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplayer);
        }
        
    }

    private void Jump()
    {
        if(canJump)
        {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        amountOfJumpsLeft--;
        }
    }

    private void ApplyMovement()
    {

        if (isGround)
        {
        rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        else if(!isGround && !isWallSliding && movementInputDirection != 0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
            rb.AddForce(forceToAdd);

            if(Mathf.Abs(rb.velocity.x) > movementSpeed)
            {
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
            }
        }
        else if(!isGround && !isWallSliding && movementInputDirection == 0) // Wall Sliding Speed
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplayer, rb.velocity.y); 
        }
        if (isWallSliding)
        {
            if(rb.velocity.y < -wallSlidingSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
            }
        }
    }
        

   private void Flip() 
   {
       if(!isWallSliding)
       {
       IsFacingRight = !IsFacingRight;
       transform.Rotate(0.0f, 180.0f, 0.0f);
       }
   }

  private void  OnDrawGizmos() 
  {
      Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

      Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
  }

 
}
