using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] protected GameObject deathEffect;

    protected override void OnDeath()
    {
        // Nonaktifkan komponen patrol & melee
        EnemyPatrol patrol = GetComponentInParent<EnemyPatrol>();
        if (patrol != null) patrol.enabled = false;
        Melee melee = GetComponentInParent<Melee>();
        if (melee != null) melee.enabled = false;

        // Efek mati & reward (opsional)
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Hancurkan setelah animasi
        Destroy(gameObject, 1f);
    }
}