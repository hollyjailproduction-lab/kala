using UnityEngine;

public class PlayerComboHybrid : MonoBehaviour
{
    [Header("Attack Durations (fallback)")]
    public float attack1Duration = 0.4f;
    public float attack2Duration = 0.4f;
    public float attack3Duration = 0.6f;
    public float damageNormalizedTime = 0.7f; // persentase durasi untuk damage jika tanpa event

    [Header("Combo")]
    public float comboWindowTime = 0.3f;
    public int[] damagePerAttack = { 10, 15, 20 };
    public LayerMask enemyLayer;
    public Transform attackPoint;

    private Animator anim;
    private int currentAttack = 0;
    private float lastAttackTime;
    private float lastComboInputTime;
    private bool isAttacking = false;
    private float attackStartTime;
    private float currentDuration;
    private bool damageDone = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        if (attackPoint == null) attackPoint = transform;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking && Time.time >= lastAttackTime + comboWindowTime)
            {
                // Mulai atau lanjut combo
                if (currentAttack > 0 && Time.time <= lastComboInputTime + comboWindowTime)
                    currentAttack++;
                else
                    currentAttack = 1;

                if (currentAttack > 3) currentAttack = 3;

                StartAttack();
            }
        }

        if (isAttacking)
        {
            float elapsed = Time.time - attackStartTime;
            if (!damageDone && elapsed >= currentDuration * damageNormalizedTime)
            {
                ApplyDamage();
                damageDone = true;
            }
            if (elapsed >= currentDuration)
                EndAttack();
        }

        if (currentAttack > 0 && !isAttacking && Time.time > lastComboInputTime + comboWindowTime)
            currentAttack = 0;
    }

    void StartAttack()
    {
        isAttacking = true;
        damageDone = false;
        attackStartTime = Time.time;
        lastAttackTime = Time.time;
        lastComboInputTime = Time.time;

        switch (currentAttack)
        {
            case 1: currentDuration = attack1Duration; break;
            case 2: currentDuration = attack2Duration; break;
            case 3: currentDuration = attack3Duration; break;
            default: currentDuration = 0.5f; break;
        }

        anim.SetTrigger($"Attack{currentAttack}");
    }

    void ApplyDamage()
    {
        int dmg = damagePerAttack[currentAttack - 1];
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, 1.2f, enemyLayer);
        foreach (var hit in hits)
        {
            Health h = hit.GetComponent<Health>();
            if (h != null) h.TakeDamage(dmg, DamageSource.Player);
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        if (currentAttack >= 3) currentAttack = 0;
    }

    // Method untuk dipanggil dari Animation Events (jika tersedia)
    public void OnDamageEvent() => ApplyDamage();
    public void OnAttackEnd() => EndAttack();
}