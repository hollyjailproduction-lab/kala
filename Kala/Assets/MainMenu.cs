using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlaySampleScene()
    {
        // Reset flag tutorial untuk New Game
        PlayerPrefs.DeleteKey("TutorialShown");
        PlayerPrefs.Save();

        SceneManager.LoadScene("CutSceneNewGame");
    }

    public void ContinueGame()
    {
        SaveController saveController = FindObjectOfType<SaveController>();
        if (saveController != null && saveController.HasSaveFile())
        {
            saveController.ContinueGame();
        }
        else
        {
            Debug.Log("No save file found. Starting new game.");
            PlaySampleScene(); // atau load scene default
        }
    }

    public void ExitSampleScene()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}