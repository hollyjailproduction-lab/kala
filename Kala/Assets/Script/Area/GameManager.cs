using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Vector2 checkpointPos;
    string checkpointScene;          // scene tempat checkpoint terakhir
    Rigidbody2D playerRb;
    Transform playerTransform;
    private Vector3 originalPlayerScale;

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
        // Inisialisasi checkpoint default (scene awal, posisi awal player)
        if (string.IsNullOrEmpty(checkpointScene))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                checkpointScene = SceneManager.GetActiveScene().name;
                checkpointPos = player.transform.position;
            }
        }
        RefreshPlayerReference();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshPlayerReference();
    }

    void RefreshPlayerReference()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            playerTransform = player.transform;
            originalPlayerScale = playerTransform.localScale;

            // Jangan mengubah checkpointPos di sini! Biarkan sesuai yang tersimpan.
            // Tapi jika scene yang dimuat adalah scene checkpoint, kita tidak perlu teleport.
            // Namun untuk scene lain, player akan ditempatkan di spawn default.
            // Kita tidak mengubah posisi player di sini agar tidak mengganggu saat loading scene.
            // Saat respawn nanti, posisi akan diatur ke checkpointPos.

            // Sinkronisasi health
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.maxHp = playerMaxHealth;
                if (playerCurrentHealth > 0)
                    playerHealth.currentHp = Mathf.Min(playerCurrentHealth, playerMaxHealth);
                else
                    playerHealth.currentHp = playerMaxHealth;
                playerHealth.Revive();
                playerCurrentHealth = playerHealth.currentHp;
                playerMaxHealth = playerHealth.maxHp;
            }

            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null && !pm.enabled) pm.enabled = true;

            HealthBar healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
            {
                healthBar.SetMaxValue(playerMaxHealth);
                healthBar.SetValue(playerCurrentHealth);
            }

            Debug.Log($"Player ready in scene: {SceneManager.GetActiveScene().name}, HP: {playerCurrentHealth}/{playerMaxHealth}");
        }
    }

    public void UpdateCheckPoint(Vector2 pos)
    {
        checkpointPos = pos;
        checkpointScene = SceneManager.GetActiveScene().name;
        Debug.Log($"Checkpoint updated at {checkpointScene}: {checkpointPos}");
    }

    public void die()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (checkpointScene != currentScene)
        {
            // Mati di scene lain: muat scene checkpoint dan respawn tanpa animasi
            StartCoroutine(LoadSceneAndRespawn(checkpointScene));
        }
        else
        {
            // Mati di scene yang sama: respawn dengan animasi (mengecil, delay)
            StartCoroutine(AnimatedRespawn(0.5f));
        }
    }

    IEnumerator LoadSceneAndRespawn(string sceneName)
    {
        // Muat scene checkpoint
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        // Scene sudah dimuat, refresh referensi player
        RefreshPlayerReference();
        if (playerTransform == null) yield break;

        // Langsung set posisi ke checkpoint tanpa delay
        playerTransform.position = checkpointPos;
        playerTransform.localScale = originalPlayerScale;
        playerRb.velocity = Vector2.zero;
        playerRb.simulated = true;

        // Reset health dan UI
        playerCurrentHealth = playerMaxHealth;
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null) healthBar.SetValue(playerCurrentHealth);

        // Aktifkan komponen player
        PlayerMovement pm = playerTransform.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;

        Health playerHealth = playerTransform.GetComponent<Health>();
        if (playerHealth != null) playerHealth.Revive();
    }

    IEnumerator AnimatedRespawn(float duration)
    {
        // Pastikan referensi player valid
        if (playerRb == null || playerTransform == null)
            RefreshPlayerReference();
        if (playerRb == null || playerTransform == null) yield break;

        // Animasi respawn
        playerRb.velocity = Vector2.zero;
        playerRb.simulated = false;
        playerTransform.localScale = Vector3.zero;
        yield return new WaitForSeconds(duration);
        playerTransform.position = checkpointPos;
        playerTransform.localScale = originalPlayerScale;

        // Reset health
        playerCurrentHealth = playerMaxHealth;
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null) healthBar.SetValue(playerCurrentHealth);

        PlayerMovement pm = playerTransform.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;

        Health playerHealth = playerTransform.GetComponent<Health>();
        if (playerHealth != null) playerHealth.Revive();

        playerRb.simulated = true;
    }

    IEnumerator respawn(float duration)
    {
        // Pastikan referensi player sudah valid
        if (playerRb == null || playerTransform == null)
        {
            RefreshPlayerReference();
            if (playerRb == null || playerTransform == null)
            {
                Debug.LogError("Cannot respawn: Player not found!");
                yield break;
            }
        }

        // Animasi respawn (mengecil, delay, muncul di checkpoint)
        playerRb.velocity = Vector2.zero;
        playerRb.simulated = false;
        playerTransform.localScale = Vector3.zero;
        yield return new WaitForSeconds(duration);
        playerTransform.position = checkpointPos;
        playerTransform.localScale = originalPlayerScale;

        // Reset health
        playerCurrentHealth = playerMaxHealth;
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null) healthBar.SetValue(playerCurrentHealth);

        PlayerMovement pm = playerTransform.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;

        Health playerHealth = playerTransform.GetComponent<Health>();
        if (playerHealth != null) playerHealth.Revive();

        playerRb.simulated = true;
    }
}