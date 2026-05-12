using UnityEngine;
using UnityEngine.SceneManagement;
using Inventory.UI;

public class InGameMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public InventoryPage inventoryPage;
    public string mainMenuSceneName = "MainMenu";

    private bool isMenuOpen = false;

    private void Start()
    {
        if (inventoryPage == null)
            inventoryPage = FindObjectOfType<InventoryPage>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryPage != null && inventoryPage.gameObject.activeSelf)
            {
                inventoryPage.Hide();
                return;
            }
            if (isMenuOpen)
                ResumeGame();
            else
                OpenMenu();
        }
    }

    private void OpenMenu()
    {
        if (menuPanel == null) return;
        isMenuOpen = true;
        menuPanel.SetActive(true);
        PauseController.SetPause(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (menuPanel == null) return;
        isMenuOpen = false;
        menuPanel.SetActive(false);
        PauseController.SetPause(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GoToMainMenu()
    {
        PauseController.SetPause(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}