using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRun : StateMachineBehaviour
{
    public float speed = 1f;
    public float closeAttackRange = 2f;   // jarak untuk serangan dekat (Attack1/Attack2 random)
    public float longAttackRange = 5f;    // jarak maksimum serangan jauh (Attack2)
    private Vector2 startPosition;

    private System.Random rand = new System.Random();

    Transform player;
    Rigidbody2D rb;
    Boss boss;
    private bool hasStartPos = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
        
        if (!hasStartPos && boss != null)
        {
            startPosition = boss.transform.position;
            hasStartPos = true;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || player == null) return;
        
        boss.LookAtPlayer();

        // Jika player tidak dalam area, kembali ke posisi awal
        if (!boss.CanChasePlayer())
        {
            Vector2 returnPos = Vector2.MoveTowards(rb.position, startPosition, speed * Time.fixedDeltaTime);
            rb.MovePosition(returnPos);
            if (Vector2.Distance(rb.position, startPosition) <= 0.1f)
            {
                animator.SetBool("isIdle", true);
                boss.ResetBossHealth();
            }
            return;
        }

        // Gerak mengejar player (kecuali saat serangan jarak jauh? biarkan tetap bergerak)
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        animator.SetBool("isIdle", false);

        float distance = Vector2.Distance(player.position, rb.position);
        
        // Logika serangan berdasarkan jarak
        if (distance <= longAttackRange)
        {
            if (distance <= closeAttackRange)
            {
                // Serangan dekat: random antara Attack1 dan Attack2
                int r = rand.Next(0, 2);
                if (r == 0)
                    animator.SetTrigger("Attack1");
                else
                    animator.SetTrigger("Attack2");
            }
            else
            {
                // Serangan jarak menengah/jauh: selalu Attack2
                animator.SetTrigger("Attack2");
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
    }
}