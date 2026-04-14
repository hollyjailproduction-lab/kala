using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class Item : MonoBehaviour
{
    [field: SerializeField] public ItemSO ItemData { get; private set; }
    [field: SerializeField] public int Quantity { get; set; } = 1;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float duration = 0.3f;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = ItemData.ItemImage;
    }

    public void DestroyItem()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(AnimateItemPickup());
    }

    private IEnumerator AnimateItemPickup()
    {
        audioSource.Play();
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;
        while (currentTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
