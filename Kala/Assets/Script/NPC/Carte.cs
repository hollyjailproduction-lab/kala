using UnityEngine;
using System.Collections.Generic;

public class Carte : MonoBehaviour
{
    // Daftar urutan dialog (isi di Inspector)
    public List<DialogEntry> dialogSequence;

    // Dialog berulang setelah pernah bicara (opsional)
    public string[] repeatDialogs;
    public Sprite[] repeatSprites;

    private int currentStep = 0;
    private bool playerIsClose;
    private bool hasTalked = false;

    [System.Serializable]
    public struct DialogEntry
    {
        public bool isArray;          // true = linear (array), false = node
        public string[] dialogArray;   // isi jika isArray = true
        public Sprite[] spriteArray;   // isi jika isArray = true
        public DialogNode dialogNode;  // isi jika isArray = false
    }

    private void OnEnable()
    {
        StartCoroutine(RegisterEventWhenReady());
    }

    private System.Collections.IEnumerator RegisterEventWhenReady()
    {
        // Tunggu hingga DialogManager.Instance tersedia
        while (DialogManager.Instance == null)
        {
            yield return null; // tunggu satu frame
        }
        DialogManager.Instance.OnDialogEnded += OnDialogEnded;
        Debug.Log("Event OnDialogEnded berhasil didaftarkan");
    }

    private void OnDisable()
    {
        if (DialogManager.Instance != null)
            DialogManager.Instance.OnDialogEnded -= OnDialogEnded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerIsClose)
        {
            if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive)
                return;

            if (!hasTalked)
            {
                // Mulai dari step 0
                currentStep = 0;
                hasTalked = true;
                PlayCurrentDialog();
            }
            else
            {
                // Sudah pernah bicara: gunakan dialog ulang
                DialogManager.Instance.StartDialog(repeatDialogs, repeatSprites);
            }
        }
    }

    // Memainkan dialog berdasarkan step saat ini
    void PlayCurrentDialog()
    {
        if (currentStep >= dialogSequence.Count)
        {
            // Selesai semua step, tidak perlu aksi
            return;
        }

        DialogEntry entry = dialogSequence[currentStep];
        if (entry.isArray)
        {
            DialogManager.Instance.StartDialog(entry.dialogArray, entry.spriteArray);
        }
        else
        {
            if (entry.dialogNode != null)
                DialogManager.Instance.StartDialog(entry.dialogNode);
            else
                Debug.LogWarning("DialogNode kosong pada step " + currentStep);
        }
    }

    // Dipanggil setiap kali dialog (linear/node) berakhir
    void OnDialogEnded()
    {
        Debug.Log("OnDialogEnded di Carte, hasTalked=" + hasTalked + ", currentStep=" + currentStep);
        if (!hasTalked) return;
        if (currentStep >= 0 && currentStep < dialogSequence.Count - 1)
        {
            currentStep++;
            Debug.Log("Lanjut ke step " + currentStep);
            PlayCurrentDialog();
        }
        else
        {
            Debug.Log("Sequence selesai");
            currentStep = -1; // Tandai sequence sudah selesai
        }
    }

    // Trigger detection
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