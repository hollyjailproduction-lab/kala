using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private bool isPlayerInsideArea = true;
    public Transform player;
    public bool isFlipped = false;

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }
    public void SetPlayerInsideArea(bool inside)
    {
        isPlayerInsideArea = inside;
    }

    public bool CanChasePlayer()
    {
        return isPlayerInsideArea;
    }

    public void ResetBossHealth()
    {
        BossHealth bh = GetComponent<BossHealth>();
        if (bh != null) bh.ResetHealth();
    }
}
