using UnityEngine;
using UnityEngine.InputSystem; // tambahkan untuk Input System

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; } 
    public bool IsMoving { get; private set; } // buat ngecek player sedang gerak ato gk
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;
    private float wallJumpCooldown;
    private float originalScaleX;
    private Vector2 moveInput; // input movement dari Input System

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    // Untuk menyimpan status jump dari Input System
    private bool jumpPressed = false;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalScaleX = transform.localScale.x;
    }

    // Method ini akan dipanggil oleh PlayerInput (Send Messages) saat aksi Move dilakukan
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        // Hanya gunakan sumbu X untuk horizontal (karena game side-scroller)
        moveInput = new Vector2(moveInput.x, 0);
    }

    // Method ini akan dipanggil oleh PlayerInput saat aksi Jump dilakukan
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpPressed = true;
        else if (context.canceled)
            jumpPressed = false;
    }

    private void Update()
    {
        // Blokir input jika dialog aktif
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive)
        {
            moveInput = Vector2.zero;
            jumpPressed = false;
        }

        //ini buat yang tes2
        if (DialogueController2.Instance != null && DialogueController2.Instance.IsDialogueActive)
        {
            return;
        }

        // Update IsMoving berdasarkan input movement
        IsMoving = Mathf.Abs(moveInput.x) > 0.01f;

        // Flip sprite berdasarkan arah gerak
        if (moveInput.x > 0.01f)
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        else if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);

        // Update animasi
        anim.SetBool("grounded", isGrounded());
        anim.SetBool("run", moveInput.x != 0);

        wallJumpCooldown += Time.deltaTime;

        if (wallJumpCooldown > 0.2f)
        {
            // Gerakan horizontal menggunakan moveInput.x
            rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);

            // Wall slide
            if (onWall() && !isGrounded())
            {
                rb.gravityScale = 0.3f;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -2f, float.MaxValue));
            }
            else
            {
                rb.gravityScale = 1f;
            }

            // Lompat jika jumpPressed true
            if (jumpPressed)
                Jump();
        }

        // Jika game pause, hentikan gerakan dan animasi
        if (PauseController.IsGamePaused)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("run", false);
            return;
        }
    }

    private void Jump()
    {
        // Reset trigger jump di animator
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

        // Reset jumpPressed agar tidak melompat terus
        jumpPressed = false;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0f, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return !onWall();
        //return horizontalInput == 0 && isGrounded() && !onWall();
    }
}