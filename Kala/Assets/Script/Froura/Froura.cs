using UnityEngine;

public class Froura : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    [Header("Movement")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float behindOffset = 1.5f;
    [SerializeField] private float stopDistance = 0.5f;

    [Header("Idle Behavior")]
    [SerializeField] private float idleTimeToWander = 5f;
    [SerializeField] private float wanderRadius = 2f;
    [SerializeField] private float wanderSpeed = 2f;

    [Header("Teleport")]
    [SerializeField] private float teleportDistance = 15f;

    [Header("Obstacle Detection")]
    [SerializeField] private Transform frontCheck;
    [SerializeField] private Vector2 frontCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private float frontCheckDistance = 1f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private float idleTimer;
    private bool isWandering;
    private Vector2 wanderTarget;
    private float wanderTimer;

    private PlayerMovement playerMovement;
    private float lastJumpTime;
    private float lastJumpAttemptY;
    private int jumpFailCount;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                playerMovement = player.GetComponent<PlayerMovement>();
            }
        }
    }

    private void Update()
    {
        if (target == null) return;

        CheckGrounded();

        bool playerMoving = playerMovement != null && playerMovement.IsMoving;

        Vector2 targetPos = GetTargetPosition();

        float totalDistance = Vector2.Distance(transform.position, target.position);
        if (totalDistance > teleportDistance)
        {
            TeleportToTarget(targetPos);
        }

        if (playerMoving)
        {
            idleTimer = 0f;
            isWandering = false;
            MoveTowards(targetPos);
        }
        else
        {
            idleTimer += Time.deltaTime;

            // ini buat mondar mandir
            if (idleTimer >= idleTimeToWander && !isWandering)
            {
                StartWandering();
            }

            if (isWandering)
            {
                Wander();
            }
            else
            {
                MoveTowards(targetPos);
            }
        }

        //animator
        if (anim != null)
        {
            anim.SetBool("run", Mathf.Abs(rb.velocity.x) > 0.1f);
            anim.SetBool("grounded", isGrounded);
        }
    }

    private void FixedUpdate()
    {
        //sek
    }
    private Vector2 GetTargetPosition()
    {
        float playerDir = target.localScale.x > 0 ? 1 : -1; //x=1 kanan x=-1 kiri (ini liat si player ngadep ke kanan atau kiri)
        float offsetX = -playerDir * behindOffset;          
        return new Vector2(target.position.x + offsetX, target.position.y);
    }

    //ini buat gerak ke belakang player {sek banyak masalah}
    private void MoveTowards(Vector2 targetPos)
    {
        float moveX = 0f;
        bool needJump = false;

        float distanceX = Mathf.Abs(transform.position.x - targetPos.x);
        if (distanceX > stopDistance)
        {
            moveX = targetPos.x > transform.position.x ? 1f : -1f;
        }

        // Hanya lompat jika ada jurang di depan (tidak ada tanah) dan Froura sedang bergerak
        bool shouldJumpBecauseGap = moveX != 0 && !IsGroundInFront();

        if (shouldJumpBecauseGap && Time.time - lastJumpTime > 1f)
        {
            needJump = true;
            lastJumpTime = Time.time;
        }

        // Terapkan kecepatan
        Vector2 velocity = rb.velocity;
        velocity.x = moveX * followSpeed;

        if (needJump)
        {
            velocity.y = jumpForce;
        }

        rb.velocity = velocity;
    }

    // Teleport ke posisi target
    private void TeleportToTarget(Vector2 targetPos)
    {
        transform.position = targetPos;
        rb.velocity = Vector2.zero; // Reset kecepatan agar tidak terbawa momentum
        // Opsional: reset timer atau state
        idleTimer = 0f;
        isWandering = false;
    }
    private bool IsGroundInFront()
    {
        if (frontCheck == null) return true; // Jika tidak diassign, anggap aman
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.BoxCast(frontCheck.position, frontCheckSize, 0f, direction, frontCheckDistance, groundLayer);
        return hit.collider != null; // Ada tanah di depan?
    }
    private void CheckGrounded()
    {
        if (groundCheck == null) return;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, groundCheckSize, 0f, groundLayer);
        isGrounded = colliders.Length > 0;
    }

    private void StartWandering()
    {
        isWandering = true;
        Vector2 basePos = GetTargetPosition(); // area di belakang player
        wanderTarget = basePos + Random.insideUnitCircle * wanderRadius;
        wanderTimer = 0f;
    }

    private void Wander()
    {
        float moveX = 0f;
        if (Mathf.Abs(transform.position.x - wanderTarget.x) > 0.2f)
        {
            moveX = wanderTarget.x > transform.position.x ? 1f : -1f;
        }

        // Terapkan kecepatan horizontal (gravitasi tetap bekerja)
        Vector2 velocity = rb.velocity;
        velocity.x = moveX * wanderSpeed;

        // Ganti target jika sudah dekat atau waktu habis
        wanderTimer += Time.deltaTime;
        if (Vector2.Distance(transform.position, wanderTarget) < 0.5f || wanderTimer > 3f)
        {
            Vector2 basePos = GetTargetPosition();
            wanderTarget = basePos + Random.insideUnitCircle * wanderRadius;
            wanderTimer = 0f;
        }

        rb.velocity = velocity;
    }

    //buat nampilin kotak groundcheck ama frontcheck
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        if (frontCheck != null)
        {
            Gizmos.color = Color.blue;
            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Gizmos.DrawWireCube(frontCheck.position + (Vector3)(direction * frontCheckDistance / 2), frontCheckSize);
        }
    }
}