using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    private string saveLocation;

    private void Awake()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth == null) return;

        SaveData saveData = new SaveData();
        saveData.playerPosition = player.transform.position;
        saveData.playerCurrentHealth = playerHealth.currentHp;
        saveData.playerMaxHealth = playerHealth.maxHp;
        saveData.lastSceneName = SceneManager.GetActiveScene().name; // simpan scene aktif

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveLocation, json);
        Debug.Log($"Game saved at {saveLocation} (scene: {saveData.lastSceneName})");
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveLocation);
    }

    public SaveData LoadSaveData()
    {
        if (File.Exists(saveLocation))
        {
            string json = File.ReadAllText(saveLocation);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            return saveData;
        }
        return null;
    }

    public void ApplyLoadedData()
    {
        SaveData saveData = LoadSaveData();
        if (saveData == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = saveData.playerPosition;

            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.currentHp = saveData.playerCurrentHealth;
                playerHealth.maxHp = saveData.playerMaxHealth;

                HealthBar healthBar = FindObjectOfType<HealthBar>();
                if (healthBar != null)
                {
                    healthBar.SetMaxValue(playerHealth.maxHp);
                    healthBar.SetValue(playerHealth.currentHp);
                }
            }
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.playerCurrentHealth = saveData.playerCurrentHealth;
            GameManager.instance.playerMaxHealth = saveData.playerMaxHealth;
            GameManager.instance.UpdateCheckPoint(saveData.playerPosition);
        }
    }

    // Method untuk Continue dari Main Menu
    public void ContinueGame()
    {
        SaveData saveData = LoadSaveData();
        if (saveData != null && !string.IsNullOrEmpty(saveData.lastSceneName))
        {
            // Simpan scene target ke PlayerPrefs agar GameManager tahu saat scene dimuat
            PlayerPrefs.SetString("ContinueScene", saveData.lastSceneName);
            PlayerPrefs.Save();
            SceneManager.LoadScene(saveData.lastSceneName);
        }
        else
        {
            Debug.Log("No save file to continue.");
        }
    }

    // (Opsional) Method untuk new game – hapus file save
    public void NewGame()
    {
        if (File.Exists(saveLocation))
            File.Delete(saveLocation);
        SceneManager.LoadScene("YourFirstSceneName"); // ganti dengan scene awal game
    }
}