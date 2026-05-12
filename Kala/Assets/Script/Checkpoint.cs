using UnityEngine;

public class Checkpoint : MonoBehaviour, IInteractable
{
    private static Checkpoint lastActivatedCheckpoint; // checkpoint terakhir global

    private bool isActivated = false;
    private Animator anim;
    private Collider2D col;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public bool Caninteract()
    {
        return !isActivated;
    }

    public void Interact()
    {
        if (isActivated) return;

        // Nonaktifkan checkpoint sebelumnya (jika ada)
        if (lastActivatedCheckpoint != null && lastActivatedCheckpoint != this)
            lastActivatedCheckpoint.ResetCheckpoint();

        // Aktifkan checkpoint ini
        isActivated = true;
        if (anim != null) anim.SetTrigger("appear");
        if (col != null) col.enabled = false;
        if (GameManager.instance != null)
            GameManager.instance.UpdateCheckPoint(transform.position);

         SaveController saveController = FindObjectOfType<SaveController>();
        if (saveController != null)
            saveController.SaveGame();

        lastActivatedCheckpoint = this;
    }

    // Method untuk mengaktifkan kembali checkpoint (saat load scene) tanpa interaksi
    public void ActivateFromSave()
    {
        if (isActivated) return;
        isActivated = true;
        if (anim != null) anim.SetTrigger("appear");
        if (col != null) col.enabled = false;
        lastActivatedCheckpoint = this;
    }

    // Mereset checkpoint ke kondisi tidak aktif
    public void ResetCheckpoint()
    {
        isActivated = false;
        if (col != null) col.enabled = true;
        // Jika ada animasi reset, panggil trigger "reset" (Optional)
        if (anim != null) anim.SetTrigger("reset");
    }
}