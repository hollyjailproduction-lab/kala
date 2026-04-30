using UnityEngine;

public class BossHealth : EnemyHealth
{
    [Header("Boss Settings")]
    public bool isInvulnerable = false;

    [SerializeField] private int phase2Threshold = 200;
    private bool isPhase2 = false;

    public void ResetHealth()
    {
        // Reset health ke max
        currentHp = maxHp;
        
        // Reset fase enrage
        isPhase2 = false;
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetBool("IsEnraged", false);
        
        Debug.Log($"Boss health reset to {maxHp}");
    }

    public override void TakeDamage(int damage, DamageSource source = DamageSource.Unknown, int enemyLevel = 0)
    {
        if (isInvulnerable) return;
        base.TakeDamage(damage, source, enemyLevel);
        if (!isPhase2 && currentHp <= phase2Threshold)
        {
            isPhase2 = true;
            GetComponent<Animator>().SetBool("IsEnraged", true);
        }
        //Debug.Log($"Boss HP: {currentHp}, threshold: {phase2Threshold}");
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject, 1f);
    }
}