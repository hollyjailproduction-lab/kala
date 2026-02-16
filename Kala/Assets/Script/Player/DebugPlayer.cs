using UnityEngine;

public class DebugAnimator : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        InvokeRepeating("CheckAnimator", 1f, 1f);
    }

    private void CheckAnimator()
    {
        if (anim == null)
        {
            Debug.LogError("Animator is null!");
            return;
        }
        Debug.Log($"Animator enabled: {anim.enabled}, Controller: {anim.runtimeAnimatorController}, UpdateMode: {anim.updateMode}");
    }
}