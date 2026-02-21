using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] private int damage = 100;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, DamageSource.Trap);
            }
        }
    }
}
