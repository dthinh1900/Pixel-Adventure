using UnityEngine;

public class Star : MonoBehaviour
{
    PlayerMovement player;

    Rigidbody2D rb;

    bool grounded;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGround();
    }

    void CheckGround()
    {
        if (grounded)
        {
            return;
        }

        grounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.1f,
            groundLayer
        );

        if (grounded)
        {
            rb.gravityScale = 0f;

            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.AddStar();
            Destroy(gameObject);
        }
    }
}