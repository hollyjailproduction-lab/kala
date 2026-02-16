using UnityEngine;
using System.Collections;
public class Health : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp;
    private Animator anim;
    private bool dead;
    private PlayerMovement playerMovement;

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
    
    public void TakeDamage(int damage)
    {
        if (dead) return;

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        
        
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

                //player
                if(GetComponent<PlayerMovement>() != null)
                    GetComponent<PlayerMovement>().enabled = false;

                //enemy
                if(GetComponentInParent<EnemyPatrol>() != null)
                    GetComponentInParent<EnemyPatrol>().enabled = false;

                if(GetComponentInParent<Melee>() != null)
                    GetComponentInParent<Melee>().enabled = false;
                dead = true;

            }

        }
    }
}