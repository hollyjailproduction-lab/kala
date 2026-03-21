using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Carte : MonoBehaviour
{
    public string[] dialogs;
    public Sprite[] characterSprites; // opsional
    public bool playerIsClose;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerIsClose)
        {
            DialogManager.Instance.StartDialog(dialogs, characterSprites);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerIsClose = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerIsClose = false;
    }
}