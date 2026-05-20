using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FrouraV2 : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [Tooltip("Posisi offset Froura relatif terhadap player")]
    [SerializeField] private Vector2 baseOffset = new(-1f, 1f);
    [Tooltip("Kecepatan maksimum saat mengejar player")]
    [SerializeField] private float maxSpeed = 12f;
    [Tooltip("Seberapa halus pergerakan mengikuti player (lebih kecil = lebih responsif)")]
    [SerializeField] [Range(0.01f, 1f)] private float smoothTime = 0.15f;

    [Header("Float Animation")]
    [Tooltip("Aktifkan animasi melayang naik-turun")]
    [SerializeField] private bool enableFloating = true;
    [Tooltip("Seberapa jauh Froura bergerak naik-turun")]
    [SerializeField] [Range(0f, 1f)] private float floatAmplitude = 0.2f;
    [Tooltip("Kecepatan animasi naik-turun")]
    [SerializeField] [Range(0f, 10f)] private float floatFrequency = 2f;

    [Header("Sprite Flip")]
    [Tooltip("Otomatis balik sprite sesuai arah pergerakan")]
    [SerializeField] private bool autoFlip = true;
    [Tooltip("Balik arah flip jika sprite default menghadap kanan")]
    [SerializeField] private bool invertFlip = false;

    [Header("Auto Heal")]
    [Tooltip("Aktifkan auto heal player jika HP berkurang")]
    [SerializeField] private bool enableAutoHeal = true;
    [Tooltip("Jumlah HP yang dipulihkan setiap interval")]
    [SerializeField] private int healAmount = 5;
    [Tooltip("Interval heal dalam detik")]
    [SerializeField] [Min(0.1f)] private float healInterval = 2f;

    [Header("Heal Particles")]
    [SerializeField] private ParticleSystem healParticle;
    [Tooltip("Jumlah partikel")]
    [SerializeField] [Range(1, 30)] private int emitCount = 8;
    [Tooltip("Kecepatan partikel")]
    [SerializeField] [Range(1f, 20f)] private float particleAttractSpeed = 6f;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 smoothVelocity = Vector2.zero;
    private PlayerHealth playerHealth;
    private float healTimer;
    private ParticleSystem.Particle[] particleBuffer = new ParticleSystem.Particle[64];

    private void Awake()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Froura menembus semua object — seluruh collider dijadikan trigger
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.isTrigger = true;

        TryGetComponent(out spriteRenderer);
    }

    private void Start()
    {
        if (target != null)
            playerHealth = target.GetComponent<PlayerHealth>();

        if (healParticle != null)
        {
            var main = healParticle.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            // Jangan paksa loop = false — sistem harus tetap playing agar Emit() selalu jalan
            healParticle.Play();
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        float floatOffsetY = enableFloating
            ? Mathf.Sin(Time.time * floatFrequency) * floatAmplitude
            : 0f;

        Vector2 desiredPosition = (Vector2)target.position + baseOffset + new Vector2(0f, floatOffsetY);
        Vector2 newPosition = Vector2.SmoothDamp(rb.position, desiredPosition, ref smoothVelocity, smoothTime, maxSpeed);

        rb.MovePosition(newPosition);
    }

    private void Update()
    {
        if (autoFlip && spriteRenderer != null)
        {
            if (smoothVelocity.x > 0.05f)
                spriteRenderer.flipX = invertFlip;
            else if (smoothVelocity.x < -0.05f)
                spriteRenderer.flipX = !invertFlip;
        }

        HandleAutoHeal();
        AttractHealParticles();
    }

    private void HandleAutoHeal()
    {
        if (!enableAutoHeal || playerHealth == null) return;

        bool isDead = playerHealth.currentHp <= 0;
        bool isFull = playerHealth.currentHp >= playerHealth.maxHp;

        if (isDead || isFull)
        {
            healTimer = 0f;
            return;
        }

        healTimer += Time.deltaTime;
        if (healTimer < healInterval) return;

        healTimer = 0f;
        playerHealth.AddHealth(healAmount);

        if (GameManager.instance != null)
            GameManager.instance.playerCurrentHealth = playerHealth.currentHp;

        if (healParticle != null)
        {
            healParticle.transform.position = transform.position;
            if (!healParticle.isPlaying)
                healParticle.Play();
            healParticle.Emit(emitCount);
        }
    }

    private void AttractHealParticles()
    {
        if (healParticle == null || target == null || healParticle.particleCount == 0) return;

        int count = healParticle.particleCount;
        if (particleBuffer.Length < count)
            particleBuffer = new ParticleSystem.Particle[count * 2];

        int num = healParticle.GetParticles(particleBuffer);
        Vector3 targetPos = target.position;

        for (int i = 0; i < num; i++)
        {
            Vector3 dir = (targetPos - particleBuffer[i].position).normalized;
            particleBuffer[i].velocity = dir * particleAttractSpeed;
        }

        healParticle.SetParticles(particleBuffer, num);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos || target == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(target.position, 0.2f);
        Gizmos.DrawLine(transform.position, target.position);

        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Vector3 offsetPos = (Vector2)target.position + baseOffset;
        Gizmos.DrawSphere(offsetPos, 0.15f);
    }
}
