using UnityEngine;

public class Enemy_Sideways : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int enemyLevel = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().TakeDamage(damage, DamageSource.Enemy, enemyLevel);
        }
    }
}
