using UnityEngine;

public class DaySystem : MonoBehaviour
{
    public static DaySystem Instance { get; private set; }

    [Header("Day Settings")]
    [SerializeField] private int maxDays = 60;
    [SerializeField] private int daysPerMapTransition = 2;
    [SerializeField] private int maxMapDeaths = 5;

    [Header("Death Penalty by Enemy Level")]
    [SerializeField] private int penaltyLevel1 = 1;
    [SerializeField] private int penaltyLevel2 = 3;
    [SerializeField] private int penaltyLevel3 = 5;
    [SerializeField] private int penaltyLevel4 = 7;
    [SerializeField] private int penaltyLevel5 = 10;
    [SerializeField] private int penaltyLevel6 = 15;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // === REMAINING DAYS ===

    public int RemainingDays
    {
        get
        {
            if (GameManager.instance != null)
                return GameManager.instance.remainingDays;
            return maxDays;
        }
        private set
        {
            if (GameManager.instance != null)
                GameManager.instance.remainingDays = value;
        }
    }

    public int MapDeathCount
    {
        get
        {
            if (GameManager.instance != null)
                return GameManager.instance.mapDeathCount;
            return 0;
        }
        private set
        {
            if (GameManager.instance != null)
                GameManager.instance.mapDeathCount = value;
        }
    }

    public bool FrouraRevived
    {
        get
        {
            if (GameManager.instance != null)
                return GameManager.instance.frouraRevived;
            return false;
        }
        private set
        {
            if (GameManager.instance != null)
                GameManager.instance.frouraRevived = value;
        }
    }

    // === CORE METHODS ===

    /// <summary>
    /// Dipanggil saat player pindah map. Kurangi hari.
    /// </summary>
    public void OnMapTransition()
    {
        ReduceDays(daysPerMapTransition);
        Debug.Log($"[DaySystem] Pindah map: -{daysPerMapTransition} hari. Sisa: {RemainingDays} hari");
    }

    /// <summary>
    /// Dipanggil saat player mati oleh musuh. Kurangi hari sesuai level.
    /// </summary>
    public void OnPlayerDeathByEnemy(int enemyLevel)
    {
        int penalty = GetPenaltyByLevel(enemyLevel);
        ReduceDays(penalty);
        Debug.Log($"[DaySystem] Mati oleh Nocturn Level {enemyLevel}: -{penalty} hari. Sisa: {RemainingDays} hari");
    }

    /// <summary>
    /// Dipanggil saat player mati karena trap/map (jatuh jurang, jebakan, dll).
    /// </summary>
    public void OnPlayerDeathByTrap()
    {
        MapDeathCount++;
        Debug.Log($"[DaySystem] Mati karena trap! Map death count: {MapDeathCount}/{maxMapDeaths}");

        if (MapDeathCount >= maxMapDeaths)
        {
            Debug.Log("[DaySystem] 5x mati karena trap! GAME OVER!");
            TriggerBadEnding();
        }
    }

    /// <summary>
    /// Kurangi hari. Jika habis, cek apakah Froura bisa revive atau bad ending.
    /// </summary>
    private void ReduceDays(int amount)
    {
        RemainingDays -= amount;

        if (RemainingDays <= 0)
        {
            RemainingDays = 0;

            if (!FrouraRevived)
            {
                // Froura berkorban, tambah 60 hari lagi (sekali saja)
                TriggerFrouraRevive();
            }
            else
            {
                // Sudah pernah di-revive, bad ending
                TriggerBadEnding();
            }
        }
    }

    /// <summary>
    /// Froura berkorban untuk menambah 60 hari. Hanya bisa sekali.
    /// </summary>
    private void TriggerFrouraRevive()
    {
        FrouraRevived = true;
        RemainingDays = maxDays;
        Debug.Log($"[DaySystem] Froura berkorban! Hari direset ke {maxDays}. Ini kesempatan terakhir.");

        // TODO: Tampilkan cutscene/dialog Froura berkorban
    }

    /// <summary>
    /// Bad ending - hari habis atau 5x mati kena trap.
    /// </summary>
    private void TriggerBadEnding()
    {
        Debug.Log("[DaySystem] === BAD ENDING === Game Over! Harus mulai New Game.");

        // TODO: Tampilkan Game Over screen / Bad Ending screen
        // Untuk sekarang, log saja. Nanti bisa panggil GameOverManager.
    }

    /// <summary>
    /// Reset semua data untuk New Game.
    /// </summary>
    public void ResetForNewGame()
    {
        RemainingDays = maxDays;
        MapDeathCount = 0;
        FrouraRevived = false;
        Debug.Log($"[DaySystem] New Game! Hari: {maxDays}, Map Deaths: 0, Froura Revived: false");
    }

    /// <summary>
    /// Dapatkan penalty hari berdasarkan level musuh.
    /// </summary>
    private int GetPenaltyByLevel(int level)
    {
        switch (level)
        {
            case 1: return penaltyLevel1;
            case 2: return penaltyLevel2;
            case 3: return penaltyLevel3;
            case 4: return penaltyLevel4;
            case 5: return penaltyLevel5;
            case 6: return penaltyLevel6;
            default:
                Debug.LogWarning($"[DaySystem] Enemy level {level} tidak dikenali! Pakai penalty level 1.");
                return penaltyLevel1;
        }
    }
}
