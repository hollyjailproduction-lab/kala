
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; } 
    public bool IsMoving { get; private set; } // buat ngecek player sedang gerak ato gk
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    private float originalScaleX;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private void Awake()
    {
        // reference
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalScaleX = transform.localScale.x;


        
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        IsMoving = Mathf.Abs(horizontalInput) > 0.01f;

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3 (-originalScaleX, transform.localScale.y, transform.localScale.z);

        anim.SetBool("grounded", isGrounded());
        anim.SetBool("run", horizontalInput != 0);

        wallJumpCooldown += Time.deltaTime;

        if (wallJumpCooldown > 0.2f)
        {
        
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

            if (onWall() && !isGrounded())
            {
                rb.gravityScale = 0.3f;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -2f, float.MaxValue));
            }
            else
            {
                rb.gravityScale = 1f;
            }

            if (Input.GetKeyDown(KeyCode.W))  
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

            float wallJumpDir = -Mathf.Sign(transform.localScale.x);
            rb.velocity = new Vector2(wallJumpDir * speed * 1.5f, jumpForce * 0.8f);

            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            
            anim.SetTrigger("jump");
            wallJumpCooldown = 0;
        }
    }
        

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

        private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0f, new Vector2(transform.localScale.x,0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return !onWall();
        //return horizontalInput == 0 && isGrounded() && !onWall();
    }
}
