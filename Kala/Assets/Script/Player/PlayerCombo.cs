using UnityEngine;

public class PlayerCombo : MonoBehaviour
{
    public float comboWindow = 0.4f;
    public int[] damage = { 10, 15, 20 };
    public LayerMask enemyLayer;
    public Transform attackPoint;

    private Animator anim;
    private int currentAttack = 0;
    private float lastAttackTime;
    private bool isAttacking = false;
    private bool pendingInput = false;  // buffer input selama animasi

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                // Mulai kombo baru
                currentAttack = 1;
                ExecuteAttack();
            }
            else if (currentAttack < 3)
            {
                // Buffer input untuk kombo berikutnya
                pendingInput = true;
            }
        }
    }

    void ExecuteAttack()
    {
        anim.SetTrigger($"Attack{currentAttack}");
        lastAttackTime = Time.time;
        isAttacking = true;
        pendingInput = false;
        Debug.Log($"Attack {currentAttack} triggered");
    }

    // Animation Event – dipanggil di titik kombo bisa dilanjut
    public void ComboOnComboWindow()
    {
        if (pendingInput && currentAttack < 3)
        {
            currentAttack++;
            ExecuteAttack();
        }
    }

    // Animation Event – di akhir animasi
    public void ComboOnAttackEnd()
    {
        isAttacking = false;
        pendingInput = false;
        currentAttack = 0;
    }

    // Animation Event – frame damage
    public void ComboApplyDamage()
    {
        int dmg = damage[currentAttack - 1];
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, 1.2f, enemyLayer);
        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null) health.TakeDamage(dmg, DamageSource.Player);
        }
    }
}