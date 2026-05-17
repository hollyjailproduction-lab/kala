using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] protected GameObject deathEffect;
    [SerializeField] private EnemyHealthBar healthBar;

    protected override void Awake()
    {
        base.Awake();
        currentHp = maxHp;
        if (healthBar != null)
            healthBar.Init(maxHp);
    }

    public override void TakeDamage(int damage, DamageSource source = DamageSource.Unknown, int enemyLevel = 0)
    {
        base.TakeDamage(damage, source, enemyLevel);
        if (healthBar != null)
            healthBar.SetHealth(currentHp);
    }

    protected override void OnDeath()
    {
        EnemyPatrol patrol = GetComponentInParent<EnemyPatrol>();
        if (patrol != null) patrol.enabled = false;
        Melee melee = GetComponentInParent<Melee>();
        if (melee != null) melee.enabled = false;

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject, 1f);
    }
}


