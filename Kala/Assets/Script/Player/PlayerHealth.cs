using UnityEngine;

public class PlayerHealth : Health
{
    private PlayerMovement playerMovement;

    protected override void Awake()
    {
        base.Awake();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.playerCurrentHealth == 0)
                GameManager.instance.playerCurrentHealth = maxHp;
            currentHp = GameManager.instance.playerCurrentHealth;
        }
        else
            currentHp = maxHp;
    }

    public override void TakeDamage(int damage, DamageSource source = DamageSource.Unknown, int enemyLevel = 0)
    {
        base.TakeDamage(damage, source, enemyLevel);
        if (GameManager.instance != null)
            GameManager.instance.playerCurrentHealth = currentHp;

        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null)
            healthBar.SetValue(currentHp);
    }

    protected override void OnDeath()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (GameManager.instance != null)
            GameManager.instance.die();

        // Integrasi DaySystem (salin dari kode lama)
        if (DaySystem.Instance != null)
        {
            // ... (salin sesuai kode asli)
        }
    }

    public override void Revive()
    {
        if (!dead) return;
        dead = false;
        currentHp = maxHp;
        if (GameManager.instance != null)
            GameManager.instance.playerCurrentHealth = currentHp;
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null) healthBar.SetValue(currentHp);
    }

    public override void AddMaxHealth(int amount)
    {
        maxHp += amount;
        currentHp += amount;

        if (GameManager.instance != null)
        {
            GameManager.instance.playerCurrentHealth = currentHp;
            GameManager.instance.playerMaxHealth = maxHp;
        }

        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxValue(maxHp);
            healthBar.SetValue(currentHp);
        }
    }
}