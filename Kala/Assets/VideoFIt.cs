using UnityEngine;
using UnityEngine.UI;

public class CutsceneVideoFit : MonoBehaviour
{
    [Header("Ukuran asli video")]
    public float videoWidth = 480f;
    public float videoHeight = 480f;

    [Header("Maksimal ukuran di layar (0-1 dari screen)")]
    [Range(0.1f, 1f)]
    public float maxScreenPercent = 0.85f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        FitVideo();
    }

    void FitVideo()
    {
        float screenW = Screen.width;
        float screenH = Screen.height;

        float maxW = screenW * maxScreenPercent;
        float maxH = screenH * maxScreenPercent;

        // Hitung scale agar muat di layar
        float scaleByWidth  = maxW / videoWidth;
        float scaleByHeight = maxH / videoHeight;
        float scale = Mathf.Min(scaleByWidth, scaleByHeight);

        rectTransform.sizeDelta = new Vector2(videoWidth * scale, videoHeight * scale);
        rectTransform.anchoredPosition = Vector2.zero; // tengah
    }
}