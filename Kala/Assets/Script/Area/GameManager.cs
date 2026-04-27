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
            checkpointPos = playerTransform.position;

            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                // 1. Terapkan maxHp dari GameManager ke player
                playerHealth.maxHp = playerMaxHealth;
                // 2. Atur currentHp (pastikan tidak melebihi max)
                if (playerCurrentHealth > 0)
                    playerHealth.currentHp = Mathf.Min(playerCurrentHealth, playerMaxHealth);
                else
                    playerHealth.currentHp = playerMaxHealth;

                playerHealth.Revive(); // jika mati, hidupkan kembali

                // 3. Sinkronkan kembali ke GameManager (nilai aktual)
                playerCurrentHealth = playerHealth.currentHp;
                playerMaxHealth = playerHealth.maxHp;
            }

            // Aktifkan komponen gerak player (jika mati sebelumnya)
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null && !pm.enabled) pm.enabled = true;

            // Update UI health bar
            HealthBar healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
            {
                healthBar.SetMaxValue(playerMaxHealth);
                healthBar.SetValue(playerCurrentHealth);
            }

            Debug.Log($"Player revived at scene {SceneManager.GetActiveScene().name} with HP: {playerCurrentHealth}/{playerMaxHealth}");
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