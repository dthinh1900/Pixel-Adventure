using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Stats")]
    public int maxHealth = 3;
    int currentHealth;

    [Header("Gem Drop")]
    public GameObject gemPrefab;
    public bool dropGem;

    [Header("Patrol")]
    public float moveDistance = 2f;
    public float speed = 2f;

    private Vector2 startPos;
    private bool movingRight = true;

    [Header("Damage")]
    public int damage = 4;
    

    [Header("Track")]
    public float trackRange = 4f;
    public float trackSpeed = 4f;
    bool isCoolingDown = false;

    public float attackCooldown = 2f;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask playerLayer;

    Animator anim;
    Transform player;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        startPos = transform.position;
    }
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (distance <= trackRange)
        {
            TrackPlayer();
        }
        else
        {
            Patrol();
        }
    }

    public void TrackPlayer()
    {
        anim.Play("Track");
        
        float dirX = player.position.x > transform.position.x ? 1 : -1;

        Vector2 nextPos = rb.position + Vector2.right * dirX * trackSpeed * Time.deltaTime;

        if (nextPos.x >= startPos.x - moveDistance &&
            nextPos.x <= startPos.x + moveDistance)
        {
            rb.MovePosition(nextPos);
        }

        Flip((int)dirX);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Patrol()
    {
        anim.Play("Walk");
        
        if (movingRight)
        {
            rb.MovePosition(rb.position + Vector2.right * speed * Time.deltaTime);

            if (rb.position.x >= startPos.x + moveDistance)
            {
                movingRight = false;
                Flip(-1);
            }
        }
        else
        {
            rb.MovePosition(rb.position + Vector2.left * speed * Time.deltaTime);            

            if (rb.position.x <= startPos.x - moveDistance)
            {
                movingRight = true;
                Flip(1);
            }
        }
    }

    public void Flip(int dir)
    {
        transform.localScale = new Vector3(dir, 1, 1);
    }


    public void Die()
    {
        if (dropGem && gemPrefab != null)
        {
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public void Attack()
    {
        if (isCoolingDown)
            return;

        anim.Play("Idle");

        Collider2D hitPlayer = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<PlayerMovement>()?.TakeDamage(damage);
        }

        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        isCoolingDown = true;
        anim.Play("Idle");
        
        yield return new WaitForSeconds(attackCooldown);

        isCoolingDown = false;
    }

    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}