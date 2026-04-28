using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp;
    protected Animator anim;
    protected bool dead;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damage, DamageSource source = DamageSource.Unknown, int enemyLevel = 0)
    {
        if (dead) return;
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if (currentHp > 0)
            anim.SetTrigger("hurt");
        else
            Die();
    }

    protected virtual void Die()
    {
        if (dead) return;
        dead = true;
        anim.SetTrigger("die");
        OnDeath();
    }

    protected abstract void OnDeath(); // setiap turunan punya perilaku mati sendiri

    public virtual void AddHealth(int amount)
    {
        if (dead) return;
        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    }

    public virtual void Revive()
    {
        
    }

    public virtual void AddMaxHealth(int amount)
    {
        
    }
}