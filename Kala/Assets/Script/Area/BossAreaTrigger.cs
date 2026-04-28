using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAreaTrigger : MonoBehaviour
{
    [SerializeField] private Boss boss; // referensi ke script Boss

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            boss.SetPlayerInsideArea(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            boss.SetPlayerInsideArea(false);
        }
    }
}