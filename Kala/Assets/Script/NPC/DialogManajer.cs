using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public Image characterImage;
    public GameObject continueButton;

    [Header("Settings")]
    public float wordSpeed = 0.05f;

    private string[] currentDialogs;
    private Sprite[] currentSprites;
    private int currentIndex;
    private bool isDialogActive;
    private Coroutine typingCoroutine;

    public bool IsDialogActive => isDialogActive;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!isDialogActive) return;

        if (typingCoroutine != null)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                SkipTyping();
            }
        }
        else if (continueButton.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                NextLine();
            }
        }
    }
    public void StartDialog(string[] dialogs, Sprite[] sprites)
    {
       if (isDialogActive)
            EndDialog();

        currentDialogs = dialogs;
        currentSprites = sprites;
        currentIndex = 0;
        dialoguePanel.SetActive(true);
        isDialogActive = true;

        if (currentSprites != null && currentSprites.Length > 0)
            characterImage.sprite = currentSprites[0];

        ShowText();
    }

    void ShowText()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        dialogueText.text = "";
        foreach (char c in currentDialogs[currentIndex].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(wordSpeed);
        }
        typingCoroutine = null;
        continueButton.SetActive(true);
    }

    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = currentDialogs[currentIndex]; // langsung tampilkan seluruh teks
            continueButton.SetActive(true);
        }
    }

    public void NextLine()
    {
        if (currentIndex < currentDialogs.Length - 1)
        {
            currentIndex++;
            if (currentSprites != null && currentIndex < currentSprites.Length)
                characterImage.sprite = currentSprites[currentIndex];
            continueButton.SetActive(false);
            ShowText();
        }
        else
        {
            EndDialog();
        }
    }

    public void EndDialog()
    {
        dialoguePanel.SetActive(false);
        isDialogActive = false;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = null;
    }
}