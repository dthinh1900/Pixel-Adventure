using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed=5f;
    public float jumpForce = 10f;

    private bool isGround = true;
    private bool canDoubleJump;

    public float coyoteTime = 0.5f;
    private float coyoteCounter;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        CheckJump();

        if (isGround)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    public void CheckJump()
    {
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.4f);
        }
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * 2f * Time.deltaTime;
        }
    }

    public void Move()
    {
        float move = Input.GetAxis("Horizontal");
        float targetSpeed = move * speed;
        float accel = 8f;
        float newX=Mathf.Lerp(rb.linearVelocityX,targetSpeed,accel*Time.deltaTime);
        rb.linearVelocity=new Vector3(newX,rb.linearVelocity.y,0);
    }
    public void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(coyoteCounter>0)
            {
                rb.linearVelocity=new Vector3(rb.linearVelocity.x,jumpForce,0);
                canDoubleJump = true;
            }                
            else if(canDoubleJump)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0);
                canDoubleJump = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
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
}
