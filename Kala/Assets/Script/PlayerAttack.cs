using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] spells;
    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        int spellIndex = FindSpell();
        GameObject spell = spells[spellIndex];
        
        spell.SetActive(true);
        
        spell.transform.position = firePoint.position;
        spell.transform.rotation = Quaternion.identity;
        
        // Debug direction
        float dir = Mathf.Sign(transform.localScale.x);
        Debug.Log($"Player facing: {transform.localScale.x}, Direction: {dir}");
        
        spell.GetComponent<Projectile>().SetDirection(dir);
    }

    private int FindSpell()
    {
        for (int i = 0; i < spells.Length; i++)
        {
            if (!spells[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
