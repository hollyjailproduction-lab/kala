using UnityEngine;
using TMPro;

public class DayUI : MonoBehaviour
{
    private TextMeshProUGUI dayText;

    private void Awake()
    {
        dayText = GetComponent<TextMeshProUGUI>();
        if (dayText == null)
        {
            Debug.LogError("DayUI requires a TextMeshProUGUI component!");
        }
    }

    private void Update()
    {
        if (dayText != null && GameManager.instance != null)
        {
            dayText.text = GameManager.instance.remainingDays.ToString() + " Days";
        }
    }
}
