using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   // untuk Button
using Inventory.UI;

public class InGameMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public InventoryPage inventoryPage;
    public string mainMenuSceneName = "MainMenu";

    private bool isMenuOpen = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isMenuOpen = false;
        FindReferences();
    }

    private void Start()
    {
        FindReferences();
    }

    private void FindReferences()
    {
        if (inventoryPage == null)
            inventoryPage = FindObjectOfType<InventoryPage>();

        if (menuPanel == null)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                Transform panel = canvas.transform.Find("MenuPanel");
                if (panel != null)
                {
                    menuPanel = panel.gameObject;
                    break;
                }
            }
            if (menuPanel == null)
                menuPanel = GameObject.FindGameObjectWithTag("MenuPanel");
            
            if (menuPanel == null)
                Debug.LogError($"MenuPanel tidak ditemukan di scene {SceneManager.GetActiveScene().name}!");
            else
            {
                Debug.Log($"MenuPanel ditemukan di scene {SceneManager.GetActiveScene().name}");
                SetupMenuPanelButtons(); // <-- Pasang listener ulang
            }
        }
        else
        {
            // Jika menuPanel sudah ada, tetap pasang listener ulang (misal jika panel diganti)
            SetupMenuPanelButtons();
        }
    }

    private void SetupMenuPanelButtons()
    {
        if (menuPanel == null) return;

        // Cari semua tombol di dalam menuPanel (termasuk grandchildren)
        Button[] allButtons = menuPanel.GetComponentsInChildren<Button>(true);
        Button continueBtn = null;
        Button mainMenuBtn = null;

        foreach (Button btn in allButtons)
        {
            // Cocokkan berdasarkan nama GameObject
            if (btn.name == "Continue")
                continueBtn = btn;
            else if (btn.name == "MainMenuButton")
                mainMenuBtn = btn;

            // (Opsional) fallback berdasarkan teks jika nama tidak cocok
            if (continueBtn == null)
            {
                var btnText = btn.GetComponentInChildren<UnityEngine.UI.Text>();
                if (btnText != null && btnText.text == "Continue")
                    continueBtn = btn;
            }
            if (mainMenuBtn == null)
            {
                var btnText = btn.GetComponentInChildren<UnityEngine.UI.Text>();
                if (btnText != null && btnText.text == "Main Menu")
                    mainMenuBtn = btn;
            }
        }

        if (continueBtn != null)
        {
            continueBtn.onClick.RemoveAllListeners();
            continueBtn.onClick.AddListener(ResumeGame);
            Debug.Log("Continue button found and connected");
        }
        else
            Debug.LogWarning("Tombol Continue tidak ditemukan di MenuPanel");

        if (mainMenuBtn != null)
        {
            mainMenuBtn.onClick.RemoveAllListeners();
            mainMenuBtn.onClick.AddListener(GoToMainMenu);
            Debug.Log("Main Menu button found and connected");
        }
        else
            Debug.LogWarning("Tombol Main Menu tidak ditemukan di MenuPanel");
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
            if (isMenuOpen) ResumeGame();
            else OpenMenu();
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