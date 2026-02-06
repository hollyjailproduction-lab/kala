using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp;
    
    private void Start()
    {
        currentHp = maxHp;
    }
    
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        
        // Panggil health bar update
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null)
            healthBar.SetValue(currentHp);
    }
}