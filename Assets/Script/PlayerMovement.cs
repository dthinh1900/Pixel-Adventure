using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // ===== COMPONENT =====
    Rigidbody2D rb;
    Animator anim;

    // ===== MOVEMENT =====
    [Header("Movement")]
    public float speed=5f;
    public float jumpForce = 10f;
    private bool isGround = true;
    private bool canDoubleJump;

    // ===== WALL =====
    [Header("Wall")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;

    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 12f;

    private bool isWallSliding;
    private bool isTouchingWall;
    private int wallDirection;

    // ===== DASH =====
    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;

    private bool isDashCooldown;
    private int dashCount = 1;
    private int maxDash = 1;
    private bool isDashing;
    

    // ===== ATTACK =====
    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;
    public int damage = 1;
    int attackIndex = 1;
    float attackCooldown = 0.3f;
    bool canAttack = true;

    // ===== PLAYER =====
    [Header("Player")]
    public int maxHP = 20;
    int currentHP;
    bool canTakeDamage = true;

    // ===== COLLECT =====
    [Header("Collectibles")]
    int gemCount = 0;
    bool hasKey = false;

    // ===== UI =====
    [Header("UI")]
    public Image hpBar;
    public TextMeshProUGUI gemText;
    public GameObject winPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    
    // ===== STATE =====
    bool isPaused = false;
    private float facingDirection = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
        hpBar.fillAmount = (float)currentHP / maxHP;
        gemText.text = "Gem: 0";
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isPaused) return;
        if (isDashing) return;
        CheckWall();       
        WallSlide();
        Move();
        Jump();
        CheckJump();
        anim.SetBool("Grounded", isGround);
        
        if (Input.GetKeyDown(KeyCode.K) && dashCount>0 && !isDashCooldown)
        {
            StartCoroutine(Dash());
            
        }

        if (Input.GetKeyDown(KeyCode.J) && canAttack)
        {
            StartCoroutine(AttackCooldown());
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
        anim.SetFloat("AnimState", Mathf.Abs(rb.linearVelocityX));
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {

            if (isWallSliding)
            {
                anim.SetTrigger("Jump");
                rb.linearVelocity = new Vector2(-wallDirection * wallJumpForceX, wallJumpForceY);
                isWallSliding = false;
                return;
            }

            if (isGround)
            {
                anim.SetTrigger("Jump");
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
                canDoubleJump = true;
            }

            else if (canDoubleJump)
            {
                anim.ResetTrigger("Jump");
                anim.SetTrigger("Jump");
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
                canDoubleJump = false;
            }
        }
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

        if (!isTouchingWall)
        {
            isWallSliding = false;
        }

        anim.SetBool("WallSlide", isWallSliding);

        if (isWallSliding && !isGround && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f);
        }
    }

    IEnumerator Dash()
    {
        isDashCooldown = true;
        isDashing = true;
        anim.SetTrigger("Roll");
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
        anim.SetTrigger("Attack" + attackIndex);

        attackIndex++;

        if (attackIndex > 3)
            attackIndex = 1;
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

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(int dmg)
    {
        if (!canTakeDamage) return;
        anim.SetTrigger("Hurt");
        currentHP -= dmg;
        hpBar.fillAmount = (float)currentHP / maxHP;

        StartCoroutine(DamageCooldown());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(0.5f);
        canTakeDamage = true;
    }

    public void Die()
    {
        anim.SetTrigger("Death");
        OverGame();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Gem"))
        {
            gemCount++;
            gemText.text = "Gem: " + gemCount;
            Destroy(col.gameObject);
        }
        else if (col.CompareTag("Key"))
        {
            hasKey = true;
            Destroy(col.gameObject);
        }
        else if (col.CompareTag("Door"))
        {
            if (hasKey)
            {
                WinGame();
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

    public void PauseGame()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);

        if (isPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }
    
    public void WinGame()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    
    public void OverGame()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

}


