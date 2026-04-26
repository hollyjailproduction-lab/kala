using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Vector2 checkpointPos;
    Rigidbody2D playerRb;
    Transform playerTransform;

    public string transitionedFromScene;
    public int playerCurrentHealth;
    public int playerMaxHealth = 100;

    public int remainingDays = 60;
    public int mapDeathCount = 0;
    public bool frouraRevived = false;


    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Inisialisasi pertama kali (jika player sudah ada di scene awal)
        RefreshPlayerReference();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Setiap scene dimuat, cari dan update referensi player
        RefreshPlayerReference();
    }

    void RefreshPlayerReference()    
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            playerTransform = player.transform;

            // Set checkpoint ke posisi player saat ini (sudah diatur oleh SceneTransition)
            checkpointPos = playerTransform.position;

            // Sinkronkan health
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerCurrentHealth = playerHealth.currentHp;
                playerMaxHealth = playerHealth.maxHp;

                // Pastikan player dalam keadaan hidup (jika sebelumnya dead karena data lama)
                playerHealth.Revive();
            }

            // Aktifkan kembali PlayerMovement (jika mati dari scene sebelumnya)
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null && !pm.enabled) pm.enabled = true;

            Debug.Log($"Player reference refreshed in scene: {SceneManager.GetActiveScene().name}");
        }
        
    }

    public void UpdateCheckPoint(Vector2 pos)
    {
        checkpointPos = pos;
    }

    public void die()
    {
        StartCoroutine(respawn(0.5f));
    }

    IEnumerator respawn(float duration)
    {
        if (playerRb == null || playerTransform == null)
        {
            RefreshPlayerReference();
            if (playerRb == null || playerTransform == null)
            {
                Debug.LogError("Cannot respawn: Player not found!");
                yield break;
            }
        }


        playerRb.velocity = new Vector2(0, 0);
        playerRb.simulated = false;
        playerTransform.localScale = Vector3.zero;
        yield return new WaitForSeconds(duration);
        playerTransform.position = checkpointPos;
        playerTransform.localScale = Vector3.one;
        playerCurrentHealth = playerMaxHealth;
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null) healthBar.SetValue(playerCurrentHealth);
        PlayerMovement pm = playerTransform.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;

        Health playerHealth = playerTransform.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.Revive();  // ← method baru untuk reset dead flag & komponen
        }

        playerRb.simulated = true;
    }
}