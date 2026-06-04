using UnityEngine;

public class SlashProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;

    public float lifeTime = 0.5f;

    public int damage = 5;

    int direction;

    Rigidbody2D rb;

    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(int dir)
    {
        direction = dir;

        transform.localScale =
            new Vector3(dir, 1, 1);

        rb.linearVelocity =
            new Vector2(direction * speed, 0);
    }

    

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Enemy
        Enemy enemy = col.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            Destroy(gameObject);

            return;
        }

        
        if ( col.CompareTag("Ground") || col.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}