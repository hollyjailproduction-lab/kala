using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;

    [Header("Enemy Requirements")]
    [Tooltip("Daftar enemy yang harus dikalahkan untuk membuka teleport. Kosongkan jika tidak ada syarat.")]
    [SerializeField] private List<GameObject> requiredEnemies = new();

    [Header("Visual")]
    [Tooltip("Particle System yang aktif saat teleport sudah bisa digunakan")]
    [SerializeField] private ParticleSystem activeParticle;
    [Tooltip("Particle System yang tampil saat teleport masih terkunci")]
    [SerializeField] private ParticleSystem lockedParticle;

    private bool isUnlocked;

    private void Start()
    {
        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager.instance is null!");
            return;
        }

        if (transitionTo == GameManager.instance.transitionedFromScene)
        {
            if (PlayerMovement.Instance == null)
            {
                Debug.LogError("PlayerMovement.Instance is null!");
                return;
            }
            if (startPoint == null)
            {
                Debug.LogError("startPoint is null!");
                return;
            }

            PlayerMovement.Instance.transform.position = startPoint.position;

            if (PlayerMovement.Instance.TryGetComponent(out Animator anim))
            {
                anim.Rebind();
                anim.Update(0f);
            }

            if (PlayerMovement.Instance.TryGetComponent(out SpriteRenderer sr) && !sr.enabled)
                sr.enabled = true;

            Vector3 pos = PlayerMovement.Instance.transform.position;
            pos.z = 0;
            PlayerMovement.Instance.transform.position = pos;
        }

        // State awal: langsung unlock jika tidak ada syarat
        RefreshUnlockState(forceRefresh: true);
    }

    private void Update()
    {
        if (!isUnlocked)
            RefreshUnlockState();
    }

    private void RefreshUnlockState(bool forceRefresh = false)
    {
        bool allDefeated = AllEnemiesDefeated();

        if (allDefeated == isUnlocked && !forceRefresh) return;

        isUnlocked = allDefeated;
        UpdateVisuals();
    }

    private bool AllEnemiesDefeated()
    {
        if (requiredEnemies.Count == 0) return true;

        foreach (GameObject enemy in requiredEnemies)
        {
            if (enemy == null) continue; // sudah di-destroy

            Health health = enemy.GetComponentInChildren<Health>();
            if (health != null && health.currentHp > 0)
                return false;
        }

        return true;
    }

    private void UpdateVisuals()
    {
       
        if (activeParticle != null)
        {
            if (isUnlocked)
            {
                if (!activeParticle.isPlaying)
                {
                    activeParticle.Play();
                    activeParticle.Emit(80); // burst sekali saat unlock
                }
            }
            else
                activeParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (lockedParticle != null)
        {
            if (!isUnlocked)
            {
                if (!lockedParticle.isPlaying)
                    lockedParticle.Play();
            }
            else
                lockedParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!isUnlocked) return;

        if (DaySystem.Instance != null)
            DaySystem.Instance.OnMapTransition();

        GameManager.instance.transitionedFromScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(transitionTo);
    }
}
