using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class VideoAspectFitter : MonoBehaviour
{
    private RawImage rawImage;
    private RectTransform rectTransform;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        if (rawImage.texture != null)
            Fit();
    }

    void Update()
    {
        // Jika texture video berubah (misal video mulai diputar), panggil Fit()
        if (rawImage.texture != null && rawImage.texture.width > 0)
            Fit();
    }

    private void Fit()
    {
        if (rawImage.texture == null) return;

        float videoAspect = (float)rawImage.texture.width / rawImage.texture.height;
        float parentWidth = rectTransform.parent.GetComponent<RectTransform>().rect.width;
        float parentHeight = rectTransform.parent.GetComponent<RectTransform>().rect.height;
        float parentAspect = parentWidth / parentHeight;

        float newWidth, newHeight;
        if (videoAspect > parentAspect)
        {
            // Video lebih lebar (landscape) → lebar penuh, tinggi proporsional
            newWidth = parentWidth;
            newHeight = parentWidth / videoAspect;
        }
        else
        {
            // Video lebih tinggi (portrait) → tinggi penuh, lebar proporsional
            newHeight = parentHeight;
            newWidth = parentHeight * videoAspect;
        }

        rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        // Opsional: atur posisi agar tengah
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}