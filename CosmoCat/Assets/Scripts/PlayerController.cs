using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float movementInputDirection;
    private float jumpTimer;
    private int amountOfJumpsLeft;
    private int facingDirection = 1;

    public float movementSpeed = 10;
    public float jumpForce = 16;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlidingSpeed;
    public float movementForceInAir; 
    public float airDragMultiplayer = 0.9f;
    public float variableJumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpTimerSet = 0.15f;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public int amountOfJumps = 1;

    private Rigidbody2D rb;
    private Animator anim;

    private bool IsFacingRight = true;
    private bool isWalking; 
    private bool isGround;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;
    private bool isAttempingToJump;


    public Transform wallCheck;
    public Transform groundCheck;

    public LayerMask whatIsGround;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
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
        if((isGround && rb.velocity.y <= 0 || isWallSliding))
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
            if(isGround || (amountOfJumpsLeft > 0 && isTouchingWall))
            {
                NormalJump();
            }
            else 
            {
                jumpTimer = jumpTimerSet;
                isAttempingToJump = true;
            }
        }

        if(Input.GetButtonUp("Jump"))
        {
          rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
        
    }

    private void CheckJump()
    {
      if(jumpTimer > 0)
      {
          if(!isGround && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
          {
              WallJump();
          }
      }

    
      
    }

    private void NormalJump()
    {
        if (canJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
    }

    private void WallJump()
    {
        if ((isWallSliding || isTouchingWall) && movementInputDirection != 0 && canJump)
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    private void ApplyMovement()
    {

        if (!isGround && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
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
       facingDirection *= -1; 
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
