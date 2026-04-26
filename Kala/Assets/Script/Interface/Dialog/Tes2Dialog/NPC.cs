using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private DialogueController2 dialogueUI;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private bool waitingForChoice = false;

    private void Start()
    {
        dialogueUI = DialogueController2.Instance;
        if (dialogueUI == null)
        {
            Debug.LogError("DialogueController2.Instance is null! Make sure there is a DialogueController2 in the scene.");
        }
    }

    public bool Caninteract()
    {
        return !isDialogueActive && !waitingForChoice;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if (isDialogueActive)
        {
            if (!waitingForChoice) NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        if (dialogueUI == null || dialogueData == null) return;
        isDialogueActive = true;
        waitingForChoice = false;
        dialogueIndex = 0;

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);

        DisplayCurrentLine();
    }

    void NextLine()
    {
        if (dialogueUI == null) return;
        if (waitingForChoice) return;

        if (isTyping)
        {
            StopAllCoroutines();
            if (dialogueIndex < dialogueData.dialogueLines.Length)
                dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }

        // Cek pilihan
        if (dialogueData.choices != null && dialogueData.choices.Length > 0)
        {
            foreach (DialogueChoice choice in dialogueData.choices)
            {
                if (choice.dialogueIndex == dialogueIndex)
                {
                    waitingForChoice = true;
                    DisplayChoices(choice);
                    return;
                }
            }
        }

        // Cek apakah baris ini adalah akhir dialog
        if (dialogueData.endDialogueLines != null && 
            dialogueIndex < dialogueData.endDialogueLines.Length && 
            dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        // Lanjut ke baris berikutnya
        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");
        string line = dialogueData.dialogueLines[dialogueIndex];
        foreach (char letter in line)
        {
            if (dialogueUI.dialogueText == null)
            {
                Debug.LogError("dialogueUI.dialogueText is null! Cannot type.");
                yield break;
            }
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text + letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }
        isTyping = false;
    }

    void DisplayChoices(DialogueChoice choice)
    {
        dialogueUI.ClearChoices();
        for (int i = 0; i < choice.choices.Length; i++)
        {
            // Pastikan array nextDialogueIndex memiliki panjang yang sesuai
            int nextIndex = (choice.nextDialogueIndex != null && i < choice.nextDialogueIndex.Length) ? choice.nextDialogueIndex[i] : -1;
            if (nextIndex == -1)
            {
                Debug.LogWarning("Next dialogue index not defined for choice " + i);
                continue;
            }
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex));
        }
    }

    void ChooseOption(int nextIndex)
    {
        dialogueIndex = nextIndex;
        waitingForChoice = false;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        waitingForChoice = false;
        if (dialogueUI != null)
        {
            dialogueUI.SetDialogueText("");
            dialogueUI.ShowDialogueUI(false);
            dialogueUI.ClearChoices();
        }
        PauseController.SetPause(false);
    }
}