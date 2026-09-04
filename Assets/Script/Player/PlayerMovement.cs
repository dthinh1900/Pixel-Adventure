using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour
{
    // ===== COMPONENT =====
    Rigidbody2D rb;
    Animator anim;

    // ===== MOVEMENT =====
    [Header("Movement")]
    public float speed=5f;
    public float jumpForce = 10f;
    public float maxFallSpeed = -15f;
    private bool isGround = true;
    private bool canDoubleJump;

    // ===== WALL =====
    [Header("Wall")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;

    public float wallJumpForceX = 1f;
    public float wallJumpForceY = 12f;

    private bool isWallSliding;
    private bool isTouchingWall;
    private int wallDirection;
    private bool lockMove;
    private float facingDirection = 1;
    // ===== DASH =====
    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;

    
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
    // ===== Lives =====
    [Header("Lives")]
    public int maxLives = 3;
    private int currentLives;
    public TMP_Text livesText;

    // ===== COLLECT =====
    [Header("Collectibles")]
    int StarCount = 0;
    bool hasKey = false;

    // ===== UI =====
    [Header("UI")]
    public Image hpBar;
    public TextMeshProUGUI starText;
    float playTime;
    public Image damageFlash;
 
    

    //====CHECKPION=====
    private Vector3 respawnPoint;

    //==== SOUL System =====
    int soul = 0;
    public TextMeshProUGUI soulText;

    [System.Serializable]
    public class SkillUI
    {
        public Image icon;
        public Image cooldownOverlay;

        public TMP_Text stateText;

        public float cooldownTime;
        public int soulCost;

        [HideInInspector] public bool isCooldown;
    }

    [Header("Skills UI")]
    public SkillUI attackSkill;   // J
    public SkillUI dashSkill;     // K
    public SkillUI jumpSkill;     // L
    public SkillUI slashSkill;    // I
    public SkillUI transformSkill; // U
    public SkillUI summonSkill;    // O

    [Header("Slash Skill")]
    public GameObject slashPrefab;
    public Transform slashSpawnPoint;

    [Header("Transform")]
    public RuntimeAnimatorController normalAnim;
    public RuntimeAnimatorController taurusAnim;
    public float transformDuration = 10f;
    bool isTransform;

    [Header("Summon")]
    public GameObject summonPrefab;
    public bool hasSummon;
    public GameObject Boss;
    bool canJumpCooldown = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
        hpBar.fillAmount = (float)currentHP / maxHP;
        soul = PlayerPrefs.GetInt("Soul", 0);
        StarCount = PlayerPrefs.GetInt("Star", 0);
        UpdateUI();
        respawnPoint = transform.position;
        currentLives = maxLives;
        UpdateLivesUI();
    }

    // Update is called once per frame
    void Update()
    {
        playTime += Time.deltaTime;
        UpdateAllSkillUI();
        if (isDashing) return;
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.K) && dashCount > 0 && !dashSkill.isCooldown)
        {
            StartCoroutine(Dash());
            StartCoroutine(SkillCooldown(dashSkill));
        }

        if (Input.GetKeyDown(KeyCode.J) && canAttack && !attackSkill.isCooldown)
        {
            StartCoroutine(AttackCooldown());
            Attack();
            StartCoroutine(SkillCooldown(attackSkill));
        }
        if (Input.GetKeyDown(KeyCode.I) && !isTransform && !slashSkill.isCooldown && soul >= slashSkill.soulCost)
        {
            SlashSkill();
        }
        if (Input.GetKeyDown(KeyCode.U) && !transformSkill.isCooldown && soul >= transformSkill.soulCost && !isTransform)
        {
            StartCoroutine(TransformSkill());
        }
        if (Input.GetKeyDown(KeyCode.O)&& !summonSkill.isCooldown&& soul >= summonSkill.soulCost&& !hasSummon)
        {
            Summon();
        }

    }

    void HandleMovement()
    {
        CheckWall();
        WallSlide();
        Move();
        Jump();
        CheckJump();
        anim.SetBool("Grounded", isGround);
    }

    //=======================MOVE=============================
    public void Move()
    {
        if (lockMove)
        {
            return;
        }
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
            if (canJumpCooldown)
            {
                StartCoroutine(SkillCooldown(jumpSkill));
                StartCoroutine(JumpCooldownLock());
            }
            if (isWallSliding)
            {

                anim.SetTrigger("Jump");
                SoundManager.instance.PlaySound(SoundManager.instance.jumpSFX);
                float jumpX = -wallDirection * Mathf.Abs(wallJumpForceX);
                
                if (facingDirection < 0)
                {
                    jumpX = -jumpX;
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else if (facingDirection > 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }

                rb.linearVelocity = new Vector2(    jumpX,    wallJumpForceY);

                isWallSliding = false;
                lockMove = true;
                return;
            }

            if (isGround)
            {
                anim.SetTrigger("Jump");
                SoundManager.instance.PlaySound(SoundManager.instance.jumpSFX);
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
                canDoubleJump = true;
            }

            else if (canDoubleJump)
            {
                anim.ResetTrigger("Jump");
                anim.SetTrigger("Jump");
                SoundManager.instance.PlaySound(SoundManager.instance.jumpSFX);
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

            if (rb.linearVelocityY < maxFallSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, maxFallSpeed);
            }
        }
    }
    IEnumerator JumpCooldownLock()
    {
        canJumpCooldown = false;
        yield return new WaitForSeconds(jumpSkill.cooldownTime);
        canJumpCooldown = true;
    }
    public void CheckWall()
    {
        bool hitRight = Physics2D.Raycast(
        wallCheck.position,
        Vector2.right,
        wallCheckDistance,
        wallLayer
        );

        bool hitLeft = Physics2D.Raycast(
            wallCheck.position,
            Vector2.left,
            wallCheckDistance,
            wallLayer
        );

        isTouchingWall = hitLeft || hitRight;

        if (hitRight)
        {
            wallDirection = 1;            
        }
            
        else if (hitLeft)
        {
            wallDirection = -1;            
        }
        else
            wallDirection = 0;
    }

    public void WallSlide()
    {
        float move = Input.GetAxis("Horizontal");

        bool pushing = isTouchingWall && (move * facingDirection > 0);

        isWallSliding = pushing && !isGround && rb.linearVelocityY < 0;
        if (isWallSliding)
        {
            lockMove = false;
        }
        if (!isTouchingWall)
        {
            isWallSliding = false;
        }

        anim.SetBool("WallSlide", isWallSliding);

        if (isWallSliding && !isGround && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f);
            canDoubleJump = false;
            
        }
    }

    IEnumerator Dash()
    {
        
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
        
    }

//=========================ATTACK===========================
    public void Attack()
    {
        anim.SetTrigger("Attack" + attackIndex);
        SoundManager.instance.PlaySound(SoundManager.instance.attackSFX);
        attackIndex = attackIndex % 2 + 1;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position,attackRange,enemyLayer);

        foreach (Collider2D enemy in hits)
        {
            enemy.GetComponent<Enemy>()?.TakeDamage(damage);
            enemy.GetComponent<BossAI>()?.TakeDamage(damage);
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
        StartCoroutine(DamageFlashEffect());

        if (currentHP <= 0)
        {
            Respawn();
        }
    }

    IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(0.5f);
        canTakeDamage = true;
    }

    IEnumerator DamageFlashEffect()
    {
        for (int i = 0; i < 2; i++)
        {
            damageFlash.color = new Color(1f, 1f, 1f, 0.5f);

            yield return new WaitForSeconds(0.08f);

            damageFlash.color = new Color(1f, 1f, 1f, 0f);

            yield return new WaitForSeconds(0.08f);
        }
    }

    //========================SKILL============================
    void UpdateAllSkillUI()
    {
        UpdateSkillUI(attackSkill);
        UpdateSkillUI(dashSkill);
        UpdateSkillUI(jumpSkill);
        UpdateSkillUI(slashSkill);
        UpdateSkillUI(transformSkill);
        UpdateSkillUI(summonSkill);
    }
    void UpdateSkillUI(SkillUI skill)
    {
        if (skill.isCooldown)
        {
            skill.icon.color = Color.gray;
            return;
        }

        skill.stateText.text = skill.soulCost.ToString();

        if (soul >= skill.soulCost)
        {
            skill.icon.color = Color.white;
            skill.stateText.color = Color.white;
        }
        else
        {
            skill.icon.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            skill.stateText.color = Color.red;
        }
    }
    void SlashSkill()
    {
        UseSoul(slashSkill.soulCost);

        anim.SetTrigger("Attack3");
        GameObject slash = Instantiate(slashPrefab, slashSpawnPoint.position, Quaternion.identity);
        slash.GetComponent<SlashProjectile>().SetDirection((int)facingDirection);
        StartCoroutine(SkillCooldown(slashSkill));
    }
    IEnumerator TransformSkill()
    {
        isTransform = true;
        UseSoul(transformSkill.soulCost);

        anim.runtimeAnimatorController =taurusAnim;
        
        maxHP *= 2;
        currentHP = maxHP;
        damage *= 2;

        hpBar.fillAmount = (float)currentHP / maxHP;
        StartCoroutine( SkillCooldown(transformSkill));
        yield return new WaitForSeconds(transformDuration);

        anim.runtimeAnimatorController = normalAnim;

        maxHP /= 2;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        damage /= 2;

        hpBar.fillAmount = (float)currentHP / maxHP;
        isTransform = false;
    }
    void Summon()
    {
        UseSoul(summonSkill.soulCost);

        Instantiate( summonPrefab, transform.position, Quaternion.identity);
        hasSummon = true;
        StartCoroutine(SkillCooldown(summonSkill));
    }

    IEnumerator SkillCooldown(SkillUI skill)
    {
        skill.isCooldown = true;

        float timer = skill.cooldownTime;
        skill.cooldownOverlay.fillAmount = 1;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            skill.cooldownOverlay.fillAmount = timer / skill.cooldownTime;

            skill.stateText.text = Mathf.CeilToInt(timer).ToString();
            yield return null;
        }

        skill.cooldownOverlay.fillAmount = 0;
        skill.isCooldown = false;
    }

    //=======================LIVE=============================
    public void Respawn()
    {
        currentLives--;

        if (currentLives <= 0)
        {
            Die();
            return;
        }

        currentHP = maxHP;
        hpBar.fillAmount = (float)currentHP / maxHP;
        transform.position = respawnPoint;
        UpdateLivesUI();
    }
    public void UpdateLivesUI()
    {
        livesText.text = "X" + currentLives;
    }
    public void Die()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.playerDieSFX);
        anim.SetTrigger("Death");
        GameManager.instance.GameOver();
    }
    //=========================Collider2D===========================
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Key"))
        {
            hasKey = true;
            Destroy(col.gameObject);
        }
        else if (col.CompareTag("Door"))
        {
            if (hasKey)
            {
                GameManager.instance.WinGame();
            }
        }
        else if (col.CompareTag("BossSummon"))
        {
            Boss.SetActive(true);
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
            lockMove = false;
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
    
    //=======================CHECKPOINT,HEAL,SOUL,STAR=============================
    public void SetCheckpoint(Vector3 point)
    {
        respawnPoint = point;
    }
    public void Heal(int amount)
    {
        currentHP += amount;

        if (currentHP > maxHP)
            currentHP = maxHP;

        hpBar.fillAmount = (float)currentHP / maxHP;
    }
    public void AddSoul(int amount)
    {
        soul += amount;

        PlayerPrefs.SetInt("Soul", soul);

        soulText.text = "X" + soul;
    }
    public void AddStar()
    {
        StarCount++;

        PlayerPrefs.SetInt("Star", StarCount);

        SoundManager.instance.PlaySound(SoundManager.instance.collectSFX);

        starText.text = "X" + StarCount;
    }

    //============================UI========================
    public void UpdateUI()
    {
        starText.text = "X" + StarCount;
        soulText.text = "X" + soul;
    }
    public int GetSoul()
    {
        return soul;
    }
    public int GetStar()
    {
        return StarCount;
    }
    void UseSoul(int amount)
    {
        soul -= amount;

        if (soul < 0)
            soul = 0;

        PlayerPrefs.SetInt("Soul", soul);

        soulText.text = "X" + soul;
    }
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(playTime / 60);

        float seconds = playTime % 60;

        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }
}


