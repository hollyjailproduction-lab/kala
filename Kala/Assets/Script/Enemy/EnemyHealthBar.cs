using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private Vector3 initLocalScale;

    private void Start()
    {
        initLocalScale = transform.localScale;
    }

    private void LateUpdate()
    {
        // Counteract parent X flip — health bar sentiasa hadap depan
        float worldScaleX = transform.parent.lossyScale.x;
        
        worldScaleX = (GetComponent<BossHealth>() != null) ? transform.lossyScale.x: worldScaleX; 

        transform.localScale = new Vector3(
            worldScaleX < 0 ? -initLocalScale.x : initLocalScale.x,
            initLocalScale.y,
            initLocalScale.z
        );
    }

    public void Init(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void SetHealth(int current)
    {
        slider.value = current;
    }
}
