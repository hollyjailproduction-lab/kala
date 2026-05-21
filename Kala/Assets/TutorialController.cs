using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public GameObject tutorialPanel;
    public Button startButton;
    public string tutorialFlagKey = "TutorialShown";

    void Start()
    {
        // Cek apakah tutorial sudah pernah ditampilkan
        if (PlayerPrefs.GetInt(tutorialFlagKey, 0) == 0)
        {
            // Belum pernah ditampilkan, munculkan panel dan jeda game
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;  // pause game
            startButton.onClick.AddListener(OnTutorialFinished);
        }
        else
        {
            // Sudah pernah, langsung aktifkan gameplay
            tutorialPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void OnTutorialFinished()
    {
        // Set flag bahwa tutorial sudah ditampilkan
        PlayerPrefs.SetInt(tutorialFlagKey, 1);
        PlayerPrefs.Save();
        
        // Sembunyikan panel dan lanjutkan game
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}