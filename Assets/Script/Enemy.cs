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
    float currentSpeed;

    [Header("Damage")]
    public int damage = 4;
    

    [Header("Track")]
    bool isCoolingDown = false;
    public float focusRange = 3f;
    bool canFocusPlayer = true;


    public float attackCooldown = 2f;

    [Header("Attack")]
    public float attackRange = 1.5f;

    [Header("Edge Check")]
    public Transform edgeCheck;
    public float edgeDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.2f;
    public LayerMask wallLayer;

    Animator anim;
    Transform player;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = maxHealth;
        startPos = transform.position;

        Flip(movingRight ? 1 : -1);
    }
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= focusRange && canFocusPlayer)
        {
            if (distance <= attackRange &&
                IsPlayerInFront() &&
                HasGroundAhead())
            {
                if (!isCoolingDown)
                {
                    Attack();
                }
                else
                {
                    currentSpeed = 0;
                }
            }
            else
            {
                TrackPlayer();
            }
        }
        else
        {
            Patrol();
        }
        anim.SetFloat("Speed", currentSpeed);
    }




    public void TrackPlayer()
    {
        if (!HasGroundAhead() || HasWallAhead())
        {
            currentSpeed = 0;

            if (canFocusPlayer)
            {
                canFocusPlayer = false;
                StartCoroutine(FocusCooldown());
            }

            return;
        }

        currentSpeed = speed * 1.5f;

        float dirX = player.position.x > transform.position.x ? 1 : -1;

        Vector2 nextPos = rb.position +
                          Vector2.right * dirX * speed * 1.5f * Time.deltaTime;

        rb.MovePosition(nextPos);

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
        float dir = movingRight ? 1 : -1;

        Vector2 nextEdgePos =
            (Vector2)edgeCheck.position +
            Vector2.right * dir * speed * Time.deltaTime;

        bool hasGround = Physics2D.Raycast(
            nextEdgePos,
            Vector2.down,
            edgeDistance,
            groundLayer
        );

        if (!hasGround || HasWallAhead())
        {
            currentSpeed = 0;

            movingRight = !movingRight;

            Flip(movingRight ? 1 : -1);

            return;
        }

        currentSpeed = speed;

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

        StartCoroutine(ChargeAttack());
    }

    IEnumerator ChargeAttack()
    {
        isCoolingDown = true;
        currentSpeed = 0;
        anim.Play("Track");

        Vector2 originalPos = rb.position;

        float dir = transform.localScale.x;

        Vector2 attackPos = originalPos + Vector2.right * dir * 1.5f;

        float timer = 0;

        while (timer < 0.15f)
        {
            rb.MovePosition(Vector2.Lerp(originalPos, attackPos, timer / 0.15f));

            timer += Time.deltaTime;
            yield return null;
        }

        if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.5f)
        {
            player.GetComponent<PlayerMovement>()?.TakeDamage(damage);
        }

        yield return new WaitForSeconds(0.2f);

        timer = 0;

        while (timer < 0.15f)
        {
            rb.MovePosition(Vector2.Lerp(attackPos, originalPos, timer / 0.15f));

            timer += Time.deltaTime;
            yield return null;
        }

        

        yield return new WaitForSeconds(attackCooldown);

        

        isCoolingDown = false;
    }

    

    public bool IsPlayerInFront()
    {
        float dirToPlayer = player.position.x - transform.position.x;

        return (dirToPlayer > 0 && transform.localScale.x > 0) ||
               (dirToPlayer < 0 && transform.localScale.x < 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, focusRange);

        
    }



    bool HasGroundAhead()
    {
        return Physics2D.Raycast(
            edgeCheck.position,
            Vector2.down,
            edgeDistance,
            groundLayer
        );
    }

    IEnumerator FocusCooldown()
    {
        yield return new WaitForSeconds(1f);

        canFocusPlayer = true;
    }

    bool HasWallAhead()
    {
        return Physics2D.OverlapCircle(
            wallCheck.position,
            0.1f,
            wallLayer
        );
    }



}
