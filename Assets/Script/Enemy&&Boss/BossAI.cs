using System.Collections;
using UnityEngine;
using Pathfinding;

public class BossAI : MonoBehaviour
{
    [Header("HP UI")]
    public EnemyHPUI hpUI;

    [Header("Wall Distance")]
    public Transform wallCheck;
    public LayerMask wallLayer;
    public float wallDistance = 1f;
    public float wallAdjustSpeed = 5f;

    [Header("Hover")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float hoverHeight = 2f;
    public float hoverAdjustSpeed = 5f;

    [Header("Components")]
    Animator anim;
    AIPath aiPath;
    Transform player;

    [Header("Stats")]
    public int maxHealth = 20;
    int currentHealth;

    [Header("Attack")]
    public int damage = 5;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    [Header("Star Drop")]
    public GameObject starPrefab;

    [Range(0, 100)]
    public int dropChance = 100;

    public int minStar = 5;
    public int maxStar = 10;

    bool isDead;
    bool isHit;
    bool isCoolingDown;

    void Start()
    {
        anim = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
        player =GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        if (hpUI != null)
        {
            hpUI.Setup(transform, maxHealth);
        }
    }

    void Update()
    {
        if (isDead || isHit)return;
        CheckPlayer();
        HoverGround();
        KeepWallDistance();
        Flip();
    }


    void HoverGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, hoverHeight * 2, groundLayer);

        if (hit)
        {
            float distance = hit.distance;

            float diff = hoverHeight - distance;

            transform.position += Vector3.up * diff * hoverAdjustSpeed * Time.deltaTime;
        }
    }

    void KeepWallDistance()
    {
        Vector2 dir =aiPath.desiredVelocity.x >= 0? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, dir, wallDistance, wallLayer);

        if (hit)
        {
            float diff = wallDistance - hit.distance;

            transform.position -=(Vector3)dir * diff * wallAdjustSpeed * Time.deltaTime;
        }
    }

    //====================================================
    public void CheckPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            aiPath.canMove = false;

            if (!isCoolingDown)
            {
                Attack();
            }
        }
        else
        {
            aiPath.canMove = true;
        }
    }

    //====================================================
    public void Attack()
    {
        if (isCoolingDown)return;
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        isCoolingDown = true;
        aiPath.canMove = false;
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.5f);

        if (Vector2.Distance(transform.position,player.position) <= attackRange + 0.5f)
        {
            player.GetComponent<PlayerMovement>()?.TakeDamage(damage);
        }

        yield return new WaitForSeconds(0.3f);
        anim.SetBool("isAttack", false);
        yield return new WaitForSeconds(attackCooldown);
        isCoolingDown = false;
    }

    //====================================================
    public void TakeDamage(int dmg)
    {
        if (isDead)return;
        currentHealth -= dmg;
        if (hpUI != null)
        {
            hpUI.UpdateHP(currentHealth);
        }
        anim.SetTrigger("Hit");
        StartCoroutine(HitStun());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator HitStun()
    {
        isHit = true;
        aiPath.canMove = false;
        yield return new WaitForSeconds(0.3f);
        aiPath.canMove = true;
        isHit = false;
    }

    //====================================================
    public void Die()
    {
        isDead = true;
        if (hpUI != null)
        {
            Destroy(hpUI.gameObject);
        }
        aiPath.canMove = false;
        anim.SetTrigger("Die");
        DropStar();
    }

    void DropStar()
    {
        if (starPrefab == null)return;
        int random =Random.Range(0, 100);
        if (random < dropChance)
        {
            int amount =Random.Range(minStar, maxStar + 1);
            for (int i = 0; i < amount; i++)
            {
                Vector3 randomOffset =new Vector3(Random.Range(-1f, 1f),Random.Range(0f, 1f),0);
                Instantiate(starPrefab,transform.position + randomOffset,Quaternion.identity);
            }
        }
    }

    //====================================================
    public void DestroyBoss()
    {
        Destroy(gameObject);
    }

    //====================================================
    void Flip()
    {
        if (aiPath == null) return;

        float moveX = aiPath.desiredVelocity.x;

        if (moveX > 0.05f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveX < -0.05f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    //====================================================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}