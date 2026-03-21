using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Carte : MonoBehaviour
{
    public string[] firsttimeDialogs;
    public Sprite[] firsttimeSprites;

    public string[] repeatDialogs;
    public Sprite[] repeatSprites;

    public bool playerIsClose;
    private bool hasTalked = false;

    void Update()
    {
            if (Input.GetKeyDown(KeyCode.F) && playerIsClose)
        {
            if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive)
                return;

            string[] dialogsToUse;
            Sprite[] spritesToUse;

            if (!hasTalked)
            {
                dialogsToUse = firsttimeDialogs;
                spritesToUse = firsttimeSprites;
                hasTalked = true;
            }
            else
            {
                dialogsToUse = repeatDialogs;
                spritesToUse = repeatSprites;
            }

            DialogManager.Instance.StartDialog(dialogsToUse, spritesToUse);
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