using UnityEngine;
using System.Collections.Generic;

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

    [Header("Breadcrumb")]
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private float followDelay = 1f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private float idleTimer;
    private bool isWandering;
    private Vector2 wanderTarget;
    private float wanderTimer;
    private float originalScaleX;

    private PlayerMovement playerMovement;
    private float lastJumpTime;

    private Queue<Vector2> positionHistory = new Queue<Vector2>();
    private float recordTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalScaleX = transform.localScale.x;

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

        recordTimer += Time.deltaTime;
        while (recordTimer >= recordInterval)
        {
            recordTimer -= recordInterval;
            positionHistory.Enqueue(target.position);

            int maxFrames = Mathf.RoundToInt(followDelay / recordInterval);
            while (positionHistory.Count > maxFrames)
                positionHistory.Dequeue();
        }

        Vector2 targetPos = positionHistory.Count > 0 ? positionHistory.Peek() : (Vector2)target.position;

        //tp
        float totalDistance = Vector2.Distance(transform.position, target.position);
        if (totalDistance > teleportDistance)
        {
            TeleportToTarget(targetPos);
        }

        bool playerMoving = playerMovement != null && playerMovement.IsMoving;

        if (playerMoving)
        {
            idleTimer = 0f;
            isWandering = false;
            MoveTowards(targetPos);
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTimeToWander && !isWandering)
                StartWandering();

            if (isWandering)
                Wander();
            else
                MoveTowards(targetPos);
        }
        if (anim != null)
        {
            anim.SetBool("run", Mathf.Abs(rb.velocity.x) > 0.1f);
            anim.SetBool("grounded", isGrounded);
        }
    }

    private void FixedUpdate()
    {

    }

    private Vector2 GetTargetPosition()
    {
        float playerDir = target.localScale.x > 0 ? 1 : -1;
        float offsetX = -playerDir * behindOffset;
        return new Vector2(target.position.x + offsetX, target.position.y);
    }

    private void MoveTowards(Vector2 targetPos)
    {
        float moveX = 0f;
        bool needJump = false;

        float distanceX = Mathf.Abs(transform.position.x - targetPos.x);
        if (distanceX > stopDistance)
        {
            moveX = targetPos.x > transform.position.x ? 1f : -1f;
        }

        if (Mathf.Abs(moveX) > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScaleX) * Mathf.Sign(moveX), transform.localScale.y, transform.localScale.z);
        }

        float heightDiff = targetPos.y - transform.position.y;
        if (heightDiff > 0.5f && isGrounded && Time.time - lastJumpTime > 1f)
        {
            needJump = true;
            lastJumpTime = Time.time;
        }

        Vector2 velocity = rb.velocity;
        velocity.x = moveX * followSpeed;

        if (needJump)
        {
            velocity.y = jumpForce;
        }

        rb.velocity = velocity;
    }

    private void TeleportToTarget(Vector2 targetPos)
    {
        transform.position = targetPos;
        rb.velocity = Vector2.zero;
        idleTimer = 0f;
        isWandering = false;
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
        Vector2 basePos = GetTargetPosition();
        wanderTarget = basePos + Random.insideUnitCircle * wanderRadius;
        wanderTimer = 0f;
    }

    //buat si froura keliling
    private void Wander()
    {
        float moveX = 0f;
        if (Mathf.Abs(transform.position.x - wanderTarget.x) > 0.2f)
        {
            moveX = wanderTarget.x > transform.position.x ? 1f : -1f;
        }
        if (Mathf.Abs(moveX) > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScaleX) * Mathf.Sign(moveX), transform.localScale.y, transform.localScale.z);
        }

        Vector2 velocity = rb.velocity;
        velocity.x = moveX * wanderSpeed;
        rb.velocity = velocity;

        wanderTimer += Time.deltaTime;
        if (Vector2.Distance(transform.position, wanderTarget) < 0.5f || wanderTimer > 3f)
        {
            Vector2 basePos = GetTargetPosition();
            wanderTarget = basePos + Random.insideUnitCircle * wanderRadius;
            wanderTimer = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}