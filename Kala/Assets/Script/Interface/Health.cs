using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp;
    private Animator anim;
    private bool dead;
    private PlayerMovement playerMovement;

    // Untuk menyimpan info kematian
    private DamageSource lastDamageSource = DamageSource.Unknown;
    private int lastEnemyLevel = 0;

    private void Start()
    {
        if (gameObject.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                // Jika pertama kali (HP di GameManager masih 0), set ke max
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

    // Method TakeDamage dengan parameter lengkap (default untuk kompatibilitas)
    public void TakeDamage(int damage, DamageSource source = DamageSource.Unknown, int enemyLevel = 0)
    {
        if (dead) return;

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        // Catat sumber damage untuk kematian
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

                // Player death handling
                if (GetComponent<PlayerMovement>() != null)
                {
                    GetComponent<PlayerMovement>().enabled = false;

                    // Integrasi dengan DaySystem (jika ada)
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

                // Enemy death handling (jika musuh mati)
                if (GetComponentInParent<EnemyPatrol>() != null)
                    GetComponentInParent<EnemyPatrol>().enabled = false;

                if (GetComponentInParent<Melee>() != null)
                    GetComponentInParent<Melee>().enabled = false;

                dead = true;
            }
        }
    }

    public void AddHealth(int amount)
    {
        if (dead) return;

        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if (gameObject.CompareTag("Player") && GameManager.instance != null)
            GameManager.instance.playerCurrentHealth = currentHp;

        if (gameObject.CompareTag("Player"))
        {
            HealthBar healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
                healthBar.SetValue(currentHp);
        }
    }
}