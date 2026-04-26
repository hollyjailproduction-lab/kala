using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private GameManager gameManager; // tambahkan deklarasi

    private void Awake() // void, bukan GameObject
    {
        gameManager= GameObject.FindGameObjectWithTag("Player").GetComponent<GameManager>();
    }

    private void OntrigerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameManager.UpdateCheckPoint(transform.position);
        }
    }
}
