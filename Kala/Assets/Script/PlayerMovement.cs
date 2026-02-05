
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private void Awake()
    {
        // Get references to components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Flip the player
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        anim.SetBool("grounded", isGrounded());
        anim.SetBool("run", horizontalInput != 0);

        // Update cooldown
        wallJumpCooldown += Time.deltaTime;

        // Jika tidak dalam cooldown wall jump
        if (wallJumpCooldown > 0.2f)
        {
            // Normal movement
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

            // Wall sliding (tidak lock sepenuhnya)
            if (onWall() && !isGrounded())
            {
                rb.gravityScale = 0.3f;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -2f, float.MaxValue));
            }
            else
            {
                rb.gravityScale = 1f;
            }

            if (Input.GetKeyDown(KeyCode.W))  // Gunakan GetKeyDown!
                Jump();
        }
    }

    private void Jump()
    {
        anim.ResetTrigger("jump");

        if (isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetBool("grounded", false);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            // Wall jump dengan kecepatan wajar
            float wallJumpDir = -Mathf.Sign(transform.localScale.x);
            rb.velocity = new Vector2(wallJumpDir * speed * 1.5f, jumpForce * 0.8f);
            
            // Flip menghadap arah lompat
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            
            anim.SetTrigger("jump");
            wallJumpCooldown = 0;
        }
    }
        

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

        private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, new Vector2(transform.localScale.x,0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}
