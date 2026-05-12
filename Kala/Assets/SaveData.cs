using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public int playerCurrentHealth;
    public int playerMaxHealth;
    public string lastSceneName; // <-- untuk menyimpan scene terakhir
}