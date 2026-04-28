using UnityEngine;

public class Enemy : MonoBehaviour
{
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

    void Start()
    {
        currentHealth = maxHealth;
        startPos = transform.position;
    }
    void Update()
    {
        if (Time.timeScale == 0f) return;
        Patrol();
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
        if (movingRight)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;

            if (transform.position.x >= startPos.x + moveDistance)
            {
                movingRight = false;
                Flip(-1);
            }
        }
        else
        {
            transform.position += Vector3.left * speed * Time.deltaTime;

            if (transform.position.x <= startPos.x - moveDistance)
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<PlayerMovement>()?.TakeDamage(4);
        }
    }
}