using System.Collections;
using UnityEngine;

public class BossBiji : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 0.5f;
    private Coroutine disableCoroutine;

    private void OnEnable()
    {
        if (disableCoroutine != null)
            StopCoroutine(disableCoroutine);
        disableCoroutine = StartCoroutine(DisableAfterLifetime());
    }

    private IEnumerator DisableAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
        disableCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage, DamageSource.Enemy);
        }
    }

    public void DeactivateNow()
    {
        if (disableCoroutine != null) StopCoroutine(disableCoroutine);
        gameObject.SetActive(false);
    }
}