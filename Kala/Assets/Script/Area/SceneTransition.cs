using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;

    private void Start()
{
    if (GameManager.instance == null)
    {
        Debug.LogWarning("GameManager.instance is null!");
        return;
    }

    if (transitionTo == GameManager.instance.transitionedFromScene)
    {
        if (PlayerMovement.Instance == null)
        {
            Debug.LogError("PlayerMovement.Instance is null!");
            return;
        }
        if (startPoint == null)
        {
            Debug.LogError("startPoint is null!");
            return;
        }
        PlayerMovement.Instance.transform.position = startPoint.position;

        Animator anim = PlayerMovement.Instance.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        SpriteRenderer sr = PlayerMovement.Instance.GetComponent<SpriteRenderer>();
        if (sr != null && !sr.enabled)
        {
            sr.enabled = true;
            Debug.Log("SpriteRenderer diaktifkan kembali");
        }

        Vector3 pos = PlayerMovement.Instance.transform.position;
        pos.z = 0;
        PlayerMovement.Instance.transform.position = pos;
    }
}    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            GameManager.instance.transitionedFromScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(transitionTo);
        }
    }
}
