using UnityEngine;
using System.Collections;
public class Health : MonoBehaviour
{
    public int maxHp;
    public int currentHp;
    private Animator anim;
    private bool dead;
    private PlayerMovement playerMovement;

    private void Start()
    {
        currentHp = maxHp;
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    
    public void TakeDamage(int damage)
    {
        if (dead) return;

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        
        // Panggil health bar update
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null)
            healthBar.SetValue(currentHp);

        if (currentHp > 0)
        {
            anim.SetTrigger("hurt");
        }
        else
        {
            if (!dead)
            {
                dead = true;

                if (anim != null)
                {
                    anim.SetTrigger("die");
                }

                if (playerMovement != null)
                {
                    playerMovement.enabled = false;
                }

            }

        }
    }
}