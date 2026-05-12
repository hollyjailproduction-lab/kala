using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    //private Transform currentCheckpoint;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void Respawn()
    {
        if (GameManager.instance != null)
            transform.position = GameManager.instance.GetCheckpointPos();
        if (playerHealth != null)
            playerHealth.Revive();
    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.transform;
            // Perbarui posisi checkpoint di GameManager
            if (GameManager.instance != null)
                GameManager.instance.UpdateCheckPoint(currentCheckpoint.position);
            
            // Nonaktifkan collider checkpoint agar tidak terpicu lagi (opsional)
            collision.GetComponent<Collider2D>().enabled = false;
            
            // Jalankan animasi checkpoint jika ada
            Animator anim = collision.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("appear");
        }
    }
    */
}