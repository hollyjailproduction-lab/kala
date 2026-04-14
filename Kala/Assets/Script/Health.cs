using UnityEngine;
using System.Collections;
public class Health : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp;
    private Animator anim;
    private bool dead;
    private PlayerMovement playerMovement;

    private DamageSource lastDamageSource = DamageSource.Unknown;
    private int lastEnemyLevel = 0;

    private void Start()
    {
        if (gameObject.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                if (GameManager.instance.playerCurrentHealth == 0)
                    GameManager.instance.playerCurrentHealth = maxHp;

                currentHp = GameManager.instance.playerCurrentHealth;
            }
            else
            {
                currentHp = maxHp;
            }
        }  
        else
        {
            currentHp = maxHp;
        }
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    
    public void TakeDamage(int damage, DamageSource source = DamageSource.Unknown, int enemyLevel = 0)
    {
        if (dead) return;

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        lastDamageSource = source;
        lastEnemyLevel = enemyLevel;
        
        if (gameObject.CompareTag("Player") && GameManager.instance != null)
            GameManager.instance.playerCurrentHealth = currentHp;

        if (gameObject.CompareTag("Player"))
        {
            HealthBar healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
            healthBar.SetValue(currentHp);
        }
        

        if (currentHp > 0)
        {
            anim.SetTrigger("hurt");
        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("die");

                if(GetComponent<PlayerMovement>() != null)
                {
                    GetComponent<PlayerMovement>().enabled = false;

                    if (DaySystem.Instance != null)
                    {
                        if (lastDamageSource == DamageSource.Enemy)
                        {
                            DaySystem.Instance.OnPlayerDeathByEnemy(lastEnemyLevel);
                        }
                        else if (lastDamageSource == DamageSource.Trap)
                        {
                            DaySystem.Instance.OnPlayerDeathByTrap();
                        }
                        else
                        {
                            DaySystem.Instance.OnPlayerDeathByEnemy(1);
                            Debug.LogWarning("[Health] Player mati dari sumber tidak dikenal, pakai penalty level 1.");
                        }
                    }
                }

                if(GetComponentInParent<EnemyPatrol>() != null)
                    GetComponentInParent<EnemyPatrol>().enabled = false;

                if(GetComponentInParent<Melee>() != null)
                    GetComponentInParent<Melee>().enabled = false;
                dead = true;

            }

        }
    }
}
