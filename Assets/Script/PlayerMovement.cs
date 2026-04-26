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
    
    
    

    Rigidbody2D rb;

    [Header("Wall")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;

    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 12f;

    private bool isWallSliding;
    private bool isTouchingWall;
    private int wallDirection;


    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    private bool isDashCooldown;


    private int dashCount = 1;
    private int maxDash = 1;
    private bool isDashing;
    private float facingDirection = 1;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;
    public int damage = 1;


    [Header("Player")]
    public int maxHP = 5;
    int currentHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing) return;
        CheckWall();       
        WallSlide();
        Move();
        Jump();
        CheckJump();
        
        if (Input.GetKeyDown(KeyCode.K) && dashCount>0 && !isDashCooldown)
        {
            StartCoroutine(Dash());
            
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack();
        }
    }

    public void Move()
    {
        float move = Input.GetAxis("Horizontal");

        if (move > 0)
        {
            facingDirection = 1;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (move < 0)
        {
            facingDirection = -1;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        float targetSpeed = move * speed;
        float accel = 8f;

        float newX=Mathf.Lerp(rb.linearVelocityX, targetSpeed, accel * Time.deltaTime);
        rb.linearVelocity=new Vector2(newX, rb.linearVelocityY);
    }

    public void CheckJump()
    {
        if (Input.GetKeyUp(KeyCode.L) && rb.linearVelocityY > 0)
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
        if(Input.GetKeyDown(KeyCode.L))
        {
            if (isWallSliding)
            {
                rb.linearVelocity = new Vector2(-wallDirection * wallJumpForceX, wallJumpForceY);
                isWallSliding = false;
                return;
            }

            if (isGround)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
                canDoubleJump = true;
            }

            else if (canDoubleJump)
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
                canDoubleJump = true;
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
        Vector2 dir = new Vector2(facingDirection, 0);

        bool hit = Physics2D.Raycast(
            wallCheck.position,
            dir,
            wallCheckDistance,
            wallLayer
        );

        isTouchingWall = hit;

        if (hit)
            wallDirection = (int)facingDirection;
        else
            wallDirection = 0;
    }

    public void WallSlide()
    {
        float move = Input.GetAxis("Horizontal");

        bool pushing = isTouchingWall && (move * facingDirection > 0);

        isWallSliding = pushing;

        if (isWallSliding && !isGround && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f);
        }
    }

    IEnumerator Dash()
    {
        isDashCooldown = true;
        isDashing = true;
        
        if (!isGround) dashCount--;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(0.2f);
        isDashCooldown = false;
    }



    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
        attackPoint.position,
        attackRange,
        enemyLayer
        );

        foreach (Collider2D enemy in hits)
        {
            enemy.GetComponent<Enemy>()?.TakeDamage(damage);
        }
    }


    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Debug.Log("Player Dead");
    }
}
