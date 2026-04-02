using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public Image characterImage;
    public GameObject continueButton;
    public GameObject choiceButtonPrefab; //ini pake prefab buat pilihan
    public Transform choiceButtonsParent;
    public System.Action OnDialogEnded;

    [Header("Settings")]
    public float wordSpeed = 0.05f;

    private string[] currentDialogs;
    private Sprite[] currentSprites;
    private int currentIndex;
    private bool isDialogActive;
    private Coroutine typingCoroutine;

    private DialogNode currentNode;
    private bool isChoiceMode;

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
        if (choiceButtonsParent != null)
            choiceButtonsParent.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isDialogActive) return;

        // Dalam mode pilihan, tidak perlu input dari keyboard/mouse
        if (isChoiceMode) return;

        // Mode typing
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

    // ========== DIALOG TREE (NODE) ==========
    public void StartDialog(DialogNode startNode)
    {
        if (isDialogActive)
            EndDialog();

        currentNode = startNode;
        isChoiceMode = false;
        isDialogActive = true;
        dialoguePanel.SetActive(true);
        ShowCurrentNode();
    }

    void ShowCurrentNode()
    {
        if (currentNode == null)
        {
            EndDialog();
            return;
        }

        // Tampilkan teks dan sprite
        dialogueText.text = "";
        if (currentNode.characterSprite != null)
            characterImage.sprite = currentNode.characterSprite;

        // Jika ada pilihan, tampilkan tombol pilihan
        if (currentNode.choices.Count > 0)
        {
            isChoiceMode = true;
            continueButton.SetActive(false);
            if (choiceButtonsParent != null)
            {
                // Bersihkan tombol sebelumnya
                foreach (Transform child in choiceButtonsParent)
                    Destroy(child.gameObject);
                choiceButtonsParent.gameObject.SetActive(true);

                // Instantiate tombol untuk setiap pilihan
                foreach (var choice in currentNode.choices)
                {
                    Button btn = Instantiate(choiceButtonPrefab, choiceButtonsParent).GetComponent<Button>();
                    TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
                    if (btnText != null)
                    {
                        btnText.text = choice.choiceText;
                        Debug.Log($"Tombol dibuat: {choice.choiceText}");
                    }
                    else
                    {
                        Debug.LogError("Tidak ada komponen TextMeshPro di prefab tombol!");
                    }

                    DialogNode next = choice.nextNode;
                    btn.onClick.AddListener(() => OnChoiceSelected(next));
                }
            }
            // Tampilkan teks langsung (tanpa typing) karena mode pilihan
            dialogueText.text = currentNode.dialogueText;
            typingCoroutine = null;
        }
        else
        {
            // Tidak ada pilihan, gunakan mode typing biasa
            isChoiceMode = false;
            if (choiceButtonsParent != null)
                choiceButtonsParent.gameObject.SetActive(false);
            continueButton.SetActive(false);
            StartCoroutine(TypeNodeLine());
        }
    }

    IEnumerator TypeNodeLine()
    {
        dialogueText.text = "";
        foreach (char c in currentNode.dialogueText.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(wordSpeed);
        }
        continueButton.SetActive(true);
        typingCoroutine = null;
    }

    void OnChoiceSelected(DialogNode nextNode)
    {
        if (choiceButtonsParent != null)
            choiceButtonsParent.gameObject.SetActive(false);
        currentNode = nextNode;
        ShowCurrentNode();
    }

    // ========== DIALOG LINEAR (ARRAY) ==========
    public void StartDialog(string[] dialogs, Sprite[] sprites)
    {
        if (isDialogActive)
            EndDialog();

        currentDialogs = dialogs;
        currentSprites = sprites;
        currentIndex = 0;
        isChoiceMode = false;
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

    // ========== FUNGSI UMUM ==========
    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            if (!isChoiceMode)
            {
                if (currentNode != null)
                    dialogueText.text = currentNode.dialogueText;
                else
                    dialogueText.text = currentDialogs[currentIndex];
            }
            continueButton.SetActive(true);
        }
    }

    public void NextLine()
    {
        // Mode linear (array)
        if (currentNode == null)
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
        else
        {
            // Mode node tanpa pilihan
            if (currentNode.nextNode != null)
            {
                currentNode = currentNode.nextNode;
                ShowCurrentNode();
            }
            else
            {
                EndDialog(); // ini yang memicu event
            }
        }
    }

    public void EndDialog()
    {
        Debug.Log("EndDialog dipanggil");
        dialoguePanel.SetActive(false);
        isDialogActive = false;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = null;
        if (choiceButtonsParent != null)
            choiceButtonsParent.gameObject.SetActive(false);
        currentNode = null;
        isChoiceMode = false;

        // Panggil event
        OnDialogEnded?.Invoke();
    }
}