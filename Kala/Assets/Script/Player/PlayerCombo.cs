using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerCombo : MonoBehaviour
{
    public float comboWindow = 0.4f;
    public int[] damage = { 10, 15, 20 };
    public LayerMask enemyLayer;
    public Transform attackPoint;

    private static readonly int[] AttackHashes =
    {
        Animator.StringToHash("Attack1"),
        Animator.StringToHash("Attack2"),
        Animator.StringToHash("Attack3"),
    };

    private Animator anim;
    private PlayerMovement playerMovement;
    private int currentAttack = 0;
    private float lastAttackTime;
    private bool isAttacking = false;
    private bool pendingInput = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        TryGetComponent(out playerMovement);
    }

    // gamepad buttonWest / mouse klik kiri
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerMovement != null && !playerMovement.canAttack()) return;

        // Reset state
        if (isAttacking && Time.time - lastAttackTime > comboWindow)
        {
            isAttacking = false;
            currentAttack = 0;
            pendingInput = false;
        }

        if (!isAttacking)
        {
            currentAttack = 1;
            ExecuteAttack();
        }
        else if (currentAttack < 3)
        {
            pendingInput = true;
        }
    }

    void ExecuteAttack()
    {
        anim.SetTrigger(AttackHashes[currentAttack - 1]);
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
        if (currentAttack < 1 || currentAttack > damage.Length) return;
        int dmg = damage[currentAttack - 1];
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, 1.2f, enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Health health))
                health.TakeDamage(dmg, DamageSource.Player);
        }
    }

    public void CancelAttack()
    {
        isAttacking = false;
        pendingInput = false;
        currentAttack = 0;

        if (anim != null)
        {
            foreach (int hash in AttackHashes)
                anim.ResetTrigger(hash);
        }

        Debug.Log("Attack cancelled due to damage");
    }
}
