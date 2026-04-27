using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController2 : MonoBehaviour
{
    private static DialogueController2 instance;
    public static DialogueController2 Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DialogueController2>();
                if (instance == null)
                {
                    Debug.LogError("DialogueController2 instance not found in scene!");
                }
            }
            return instance;
        }
        private set { instance = value; }
    }

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text NPCName;
    public Image NPCPic;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    public bool IsDialogueActive { get; private set; } = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Optional: DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ShowDialogueUI(bool show)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(show);
        
        IsDialogueActive = show;
        if (!show)
        { 
            ClearChoices();
        }
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        if (NPCName != null) NPCName.SetText(npcName);
        if (NPCPic != null) NPCPic.sprite = portrait;
    }

    public void SetDialogueText(string text)
    {
        if (dialogueText != null)
            dialogueText.SetText(text);
        else
            Debug.LogError("DialogueText is null! Please assign in Inspector.");
    }

    public void ClearChoices()
    {
        if (choiceContainer == null) return;
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public GameObject CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        if (choiceButtonPrefab == null || choiceContainer == null)
        {
            Debug.LogError("ChoiceButtonPrefab or ChoiceContainer not assigned!");
            return null;
        }
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        TMP_Text btnText = choiceButton.GetComponentInChildren<TMP_Text>();
        if (btnText != null) btnText.text = choiceText;
        Button btn = choiceButton.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(onClick);
        return choiceButton;
    }
}