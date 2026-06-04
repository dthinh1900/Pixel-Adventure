using System.Collections;
using UnityEngine;

public class SummonPet : MonoBehaviour
{
    Transform player;

    public float speed = 5f;
    // Bán kính tìm quái gần nhất
    public float detectRange = 5f;
    public float maxDistanceFromPlayer = 4f;

    public int damage = 1;
    public float attackCooldown = 1f;
    public float lifeTime = 20f;

    Enemy target;
    PlayerMovement playerScript;
    Animator anim;

    bool canAttack = true;
    Vector3 velocity = Vector3.zero;

    [Header("Orbit")]
    // Bán kính, tốc độ bay quanh player
    public float orbitRadius = 2f;
    public float orbitSpeed = 2f;

    [Header("Hover")]
    // Độ cao tối thiểu so với mặt đất.
    public float hoverHeight = 3f;

    public float hoverAdjustSpeed = 3f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallDistance = 1.5f;
    public LayerMask wallLayer;

    float orbitAngle;
    Vector3 lastPos;

    void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        Destroy(gameObject, lifeTime);
        lastPos = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        CheckDistanceFromPlayer();

        HandleTarget();

        HandleMovement();

        if (target == null)
        {
            KeepHoverHeight();
        }

        Flip();
    }
    void CheckDistanceFromPlayer()
    {
        float playerDist = Vector2.Distance( transform.position, player.position);
        if (playerDist > maxDistanceFromPlayer)
        {
            target = null;
        }
    }
    void HandleTarget()
    {
        if (target == null)
        {
            FindEnemy();
        }
    }

    void HandleMovement()
    {
        if (target != null)
        {
            MoveToEnemy();
        }
        else
        {
            FollowPlayer();
        }
    }
    void MoveToEnemy()
    {
        float dist = Vector2.Distance( transform.position, target.transform.position);
        if (dist > 1f)
        {
            Vector2 moveDir = (target.transform.position - transform.position).normalized;

            if (HasWallAhead(moveDir))
            {
                target = null;
                return;
            }

            transform.position += (Vector3)moveDir.normalized * speed * Time.deltaTime;
        }
    }

    void FollowPlayer()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;

        Vector3 orbitOffset = new Vector3(Mathf.Cos(orbitAngle), Mathf.Sin(orbitAngle) * 0.5f, 0) * orbitRadius;

        Vector3 targetPos = player.position + orbitOffset;

        Vector2 moveDir = (targetPos - transform.position).normalized;

        if(HasWallAhead(moveDir))
{
            orbitSpeed *= -1;

            orbitAngle += Mathf.PI;

            return;
        }


        transform.position += (Vector3)moveDir.normalized * speed * Time.deltaTime;
    }

    void FindEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        float closest = detectRange;

        target = null;

        foreach (Enemy enemy in enemies)
        {
            float dist = Vector2.Distance( transform.position, enemy.transform.position);

            if (dist < closest)
            {
                closest = dist;

                target = enemy;
            }
        }
    }

    void Flip()
    {
        float moveX = transform.position.x - lastPos.x;

        if (moveX > 0.01f)
        {
            transform.localScale =
                new Vector3(
                    -Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
        }
        else if (moveX < -0.01f)
        {
            transform.localScale =
                new Vector3(
                    Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
        }

        lastPos = transform.position;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!canAttack) return;

        Enemy enemy = col.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            anim.SetTrigger("Attack");

            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    private void OnDestroy()
    {
        if (playerScript != null)
        {
            playerScript.hasSummon = false;
        }
    }



    bool HasWallAhead(Vector2 moveDir)
    {
        return Physics2D.Raycast(
            wallCheck.position,
            moveDir.normalized,
            wallDistance,
            wallLayer
        );
    }

    void KeepHoverHeight()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            20f,
            groundLayer
        );

        if (hit && hit.distance < hoverHeight)
        {
            float diff = hoverHeight - hit.distance;

            transform.position +=
                Vector3.up *
                diff *
                hoverAdjustSpeed *
                Time.deltaTime;
        }
    }
}