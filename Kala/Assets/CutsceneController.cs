using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName = "Stage_1-1"; // nama scene gameplay

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play();
        }
        else
        {
            // Jika tidak ada video player, langsung lanjut
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void Update()
    {
        // Tekan Space atau klik untuk skip
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            SkipCutscene();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private void SkipCutscene()
    {
        if (videoPlayer != null)
            videoPlayer.Stop();
        SceneManager.LoadScene(nextSceneName);
    }
}