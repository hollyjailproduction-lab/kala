using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlaySampleScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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