using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float movementInputDirection;

    private Rigidbody2D rb;



    public float movementSpeed = 10;
    public float jumpForce = 16;


    private bool IsFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
    }

    private void FixedUpdate() 
    {
        ApplyMovement();
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
    }

    private void CheckInput() 
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void ApplyMovement()
    {
        rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
    }

   private void Flip() 
   {
       IsFacingRight = !IsFacingRight;
       transform.Rotate(0.0f, 180.0f, 0.0f);
   }

  

}
