using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
public class Carte : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialougeText;
    public string[] dialogs;
    private int index;

    public Image characterImage;    public Sprite[] characterSprites; 

    public GameObject continueButton;
    public float wordSpeed;
    public bool playerIsClose;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && playerIsClose)
        {
            if (dialoguePanel.activeInHierarchy)
            {
                zeroText();
            }
            else
            {
                dialoguePanel.SetActive(true);
                if (characterSprites != null && characterSprites.Length > 0)
                    characterImage.sprite = characterSprites[0];
                StartCoroutine(Type());
            }
        }

        if(dialougeText.text == dialogs[index])
        {
            continueButton.SetActive(true);
        }
    }

    public void zeroText()
    {
        dialougeText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }

    IEnumerator Type()
    {
        foreach (char letter in dialogs[index].ToCharArray())
        {
            dialougeText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        continueButton.SetActive(false);
        
        if (index < dialogs.Length - 1)
        {
            index++;
            dialougeText.text = "";
            if (characterSprites != null && index < characterSprites.Length)
                characterImage.sprite = characterSprites[index];
            StartCoroutine(Type());
        }
        else
        {
            zeroText();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerIsClose = false;
        zeroText();
    }
}
