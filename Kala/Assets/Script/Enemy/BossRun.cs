using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRun : StateMachineBehaviour
{
    public float speed = 1f;
    public float attackRange = 3f;
    private Vector2 startPosition;

    Transform player;
    Rigidbody2D rb;
    Boss boss;
    private bool hasStartPos = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
        
        // Simpan posisi awal boss (dari transform boss)
        if (!hasStartPos && boss != null)
        {
            startPosition = boss.transform.position;
            hasStartPos = true;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || player == null) return;
        
        boss.LookAtPlayer();

        // Jika player tidak dalam area, hentikan gerakan dan kembali ke posisi awal
        if (!boss.CanChasePlayer())
        {
            Vector2 returnPos = Vector2.MoveTowards(rb.position, startPosition, speed * Time.fixedDeltaTime);
            rb.MovePosition(returnPos);
            // Jika sudah sampai, animasi idle (opsional)
            if (Vector2.Distance(rb.position, startPosition) <= 0.1f)
                animator.SetBool("isIdle", true);
            return;
        }

        // Chase player
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        animator.SetBool("isIdle", false);

        if (Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            animator.SetTrigger("Attack");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}