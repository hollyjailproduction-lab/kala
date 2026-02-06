using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health healthSource; 
    [SerializeField] private RectTransform barRect;
    [SerializeField] private RectMask2D mask;
    [SerializeField] private TMP_Text healthIndicator; 
    
    private float maxRightMask;
    private float initialRightMask;

    private void Start()
    { 
        //debug
        if (healthSource == null || barRect == null || mask == null || healthIndicator == null)
        {
            Debug.LogError("HealthBar: One or more references are missing!");
            return;
        }
        
        maxRightMask = barRect.rect.width - mask.padding.z - mask.padding.x;
        
        healthIndicator.text = $"{healthSource.currentHp}/{healthSource.maxHp}";
        
        initialRightMask = mask.padding.z;
    }

    public void SetValue(int newValue)
    {
        if (healthSource == null) return;
        
        float targetWidth = newValue * maxRightMask / healthSource.maxHp;
        float newRightMask = maxRightMask + initialRightMask - targetWidth;
        
        var padding = mask.padding;
        padding.z = newRightMask;
        mask.padding = padding;
        
        healthIndicator.text = $"{newValue}/{healthSource.maxHp}";
    }
}