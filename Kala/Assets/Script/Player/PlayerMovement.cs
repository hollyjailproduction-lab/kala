using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsSliding { get; private set; }

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;
    private float wallJumpCooldown;
    private float originalScaleX;
    private Vector2 moveInput;

    private bool jumpPressed = false;
    private bool slidePressed = false;
    private float slideTimer = 0f;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private float slideSpeedMultiplier = 1.8f;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalScaleX = transform.localScale.x;
        originalColliderSize = capsuleCollider.size;
        originalColliderOffset = capsuleCollider.offset;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        moveInput = new Vector2(moveInput.x, 0);
        Debug.Log($"Move input: {moveInput}");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpPressed = true;
        else if (context.canceled)
            jumpPressed = false;
            
        Debug.Log($"Jump input: {jumpPressed}");
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        if (context.performed)
            slidePressed = true;
        else if (context.canceled)
            slidePressed = false;

        Debug.Log($"Slide input: {slidePressed}");
    }

    private void Update()
    {
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive)
        {
            moveInput = Vector2.zero;
            jumpPressed = false;
            slidePressed = false;
        }

        if (DialogueController2.Instance != null && DialogueController2.Instance.IsDialogueActive)
            return;

        if (PauseController.IsGamePaused)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("run", false);
            return;
        }

        IsMoving = Mathf.Abs(moveInput.x) > 0.01f;

        if (moveInput.x > 0.01f)
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        else if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);

        anim.SetBool("grounded", isGrounded());
        anim.SetBool("run", moveInput.x != 0 && !IsSliding);

        // Slide update
        if (IsSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
                EndSlide();
        }
        else if (slidePressed && isGrounded())
        {
            StartSlide();
        }

        wallJumpCooldown += Time.deltaTime;

        if (wallJumpCooldown > 0.2f)
        {
            if (!IsSliding)
                rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);

            if (onWall() && !isGrounded())
            {
                rb.gravityScale = 0.3f;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -2f, float.MaxValue));
            }
            else
            {
                rb.gravityScale = 1f;
            }

            if (jumpPressed && !IsSliding)
                Jump();
        }
    }

    private void StartSlide()
    {
        IsSliding = true;
        slideTimer = slideDuration;
        slidePressed = false;
        rb.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * speed * slideSpeedMultiplier, rb.velocity.y);
        capsuleCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
        capsuleCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - originalColliderSize.y * 0.25f);
        anim.SetTrigger("slide");
    }

    private void EndSlide()
    {
        IsSliding = false;
        capsuleCollider.size = originalColliderSize;
        capsuleCollider.offset = originalColliderOffset;
    }

    private void Jump()
    {
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
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            anim.SetBool("grounded", false);
            anim.SetTrigger("jump");
            wallJumpCooldown = 0;
        }

        jumpPressed = false;
    }

    private bool isGrounded()
    {
        if (rb.velocity.y > 0.1f) return false;
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
        return !onWall() && !IsSliding;
    }

    public void IdleState()
    {
        rb.velocity = Vector2.zero;
        anim.SetTrigger("idle");
    }
}
