using UnityEngine;
using UnityEngine.SceneManagement;
using Inventory.Model;
using Inventory.UI;

public class InGameMenu : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject menuPanel;          // Panel yang berisi tombol Continue & Main Menu

    [Header("Scene Name")]

    [Header("Inventory Reference")]
    public InventoryPage inventoryPage;
    public string mainMenuSceneName = "MainMenu"; // Nama scene main menu Anda

    private bool isMenuOpen = false;

    private void Update()
    {
        // Tekan ESC untuk buka/tutup menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryPage != null && inventoryPage.gameObject.activeSelf)
            {
                inventoryPage.Hide();  // Asumsikan ada method Hide()
                return;  // Jangan proses menu
            }
            if (isMenuOpen)
                ResumeGame();
            else
                OpenMenu();
        }
    }

    private void OpenMenu()
    {
        isMenuOpen = true;
        menuPanel.SetActive(true);
        PauseController.SetPause(true);   // Pause game (menggunakan sistem pause yang sudah ada)
        // (Opsional) Lock cursor agar bisa klik UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Method untuk tombol Continue
    public void ResumeGame()
    {
        Debug.Log("ResumeGame dipanggil!"); // <- tambahkan
        isMenuOpen = false;
        menuPanel.SetActive(false);
        PauseController.SetPause(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GoToMainMenu()
    {
        Debug.Log("GoToMainMenu dipanggil!"); // <- tambahkan
        PauseController.SetPause(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}