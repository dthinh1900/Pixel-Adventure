using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed=5f;
    public float jumpForce = 10f;

    private bool isGround = true;
    private bool canDoubleJump;
    
    public float coyoteTime = 0.2f;
    private float coyoteCounter;

    Rigidbody2D rb;

    [Header("Wall")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;

    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 12f;

    private bool isWallSliding;
    private bool isTouchingWall;
    private int wallDirection;

    public bool checkLeft;
    public bool checkRight;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    private bool isDashCooldown;


    private int dashCount = 1;
    private int maxDash = 1;
    private bool isDashing;
    private float facingDirection = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing) return;
        Move();
        Jump();
        CheckJump();
        CheckWall();
        WallSlide();
        if (isGround)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
        
        if (Input.GetKeyDown(KeyCode.K) && dashCount>0 && !isDashCooldown)
        {
            StartCoroutine(Dash());
            if(!isGround)   dashCount--;
        }


    }

    public void Move()
    {
        float move = Input.GetAxis("Horizontal");

        if (move > 0) facingDirection = 1;
        else if (move < 0) facingDirection = -1;

        float targetSpeed = move * speed;
        float accel = 8f;

        float newX=Mathf.Lerp(rb.linearVelocityX, targetSpeed, accel * Time.deltaTime);
        rb.linearVelocity=new Vector2(newX, rb.linearVelocityY);
    }

    public void CheckJump()
    {
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocityY > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.4f);
        }
        if (rb.linearVelocityY < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * 2f * Time.deltaTime;
        }
    }
    public void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (isWallSliding)
            {
                rb.linearVelocity = new Vector2(-wallDirection * wallJumpForceX, wallJumpForceY);
                isWallSliding = false;
                return;
            }

            if (coyoteCounter>0)
            {
                rb.linearVelocity=new Vector2(rb.linearVelocityX,jumpForce);
                canDoubleJump = true;
                coyoteCounter = 0;
            }                
            else if(canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
                canDoubleJump = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            if (!isGround) 
            {
                dashCount = maxDash;
            }
            isGround = true;
            
        }
    }
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGround = false;
        }
    }

    public void CheckWall()
    {
        checkLeft = Physics2D.Raycast(
        wallCheckLeft.position,
        Vector2.left,
        wallCheckDistance,
        wallLayer
        );

        checkRight = Physics2D.Raycast(
            wallCheckRight.position,
            Vector2.right,
            wallCheckDistance,
            wallLayer
        );

        if (checkLeft) wallDirection = -1;
        else if (checkRight) wallDirection = 1;
        else wallDirection = 0; 

        isTouchingWall = checkLeft || checkRight;
    }

    public void WallSlide()
    {
        float move = Input.GetAxis("Horizontal");

        bool pushingLeft = checkLeft && move < 0;
        bool pushingRight = checkRight && move > 0;

        isWallSliding = (pushingLeft || pushingRight);

        if (isWallSliding && !isGround && rb.linearVelocityY < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, -2f);
        }
    }

    IEnumerator Dash()
    {
        isDashCooldown = true;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(0.2f);
        isDashCooldown = false;
    }

}
