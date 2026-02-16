using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform barRect;
    [SerializeField] private RectMask2D mask;
    [SerializeField] private TMP_Text healthIndicator; 
    
    private float maxRightMask;
    private float initialRightMask;

    private void Start()
    { 
        //debug
        if (barRect == null || mask == null || healthIndicator == null)
        {
            Debug.LogError("HealthBar: One or more references are missing!");
            return;
        }
        
        maxRightMask = barRect.rect.width - mask.padding.x - mask.padding.z;
        
        
        initialRightMask = mask.padding.z;
    }

    private void Update()
    {
        if (GameManager.instance == null) return;
        
        int current = GameManager.instance.playerCurrentHealth;
        int max = GameManager.instance.playerMaxHealth;
        
        // Update teks
        healthIndicator.text = $"{current}/{max}";
        
        // Update mask (health bar visual)
        float targetWidth = current * maxRightMask / max;
        float newRightMask = maxRightMask + initialRightMask - targetWidth;
        var padding = mask.padding;
        padding.z = Mathf.RoundToInt(newRightMask);
        mask.padding = padding;
    }

    public void SetValue(int newValue)
    {
        if (GameManager.instance == null) return;
        
        int max = GameManager.instance.playerMaxHealth;
        float targetWidth = newValue * maxRightMask / max;
        float newRightMask = maxRightMask + initialRightMask - targetWidth;
        
        var padding = mask.padding;
        padding.z = Mathf.RoundToInt(newRightMask);
        mask.padding = padding;
        
        healthIndicator.text = $"{newValue}/{max}";
    }
}