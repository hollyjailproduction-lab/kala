using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public int attackDamage = 10;
    public int enragedAttackDamage = 20;

    public Vector4 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    public GameObject groundAttackPrefab;  // prefab asli (bisa dalam keadaan tidak aktif)
    public int poolSize = 4;               // jumlah objek pool
    private List<GameObject> groundAttackPool;

    // Tambahkan variabel untuk chain ground attack
    public float startDistance = 1f;          // jarak pertama dari boss
    public float stepDistance = 1.5f;         // penambahan jarak tiap ledakan
    public float timeBetweenEruptions = 0.3f;
    public float groundAttackHeight = 0f;     // <-- Tambahkan ini (ketinggian tanah)

    private void Awake()
    {
        // Inisialisasi pool
        groundAttackPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(groundAttackPrefab);
            obj.SetActive(false);
            groundAttackPool.Add(obj);
        }
    }

    public void Attack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            Health playerHealth = colInfo.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);
        }
    }

    public void EnragedAttack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            Health playerHealth = colInfo.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(enragedAttackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Gizmos.DrawWireSphere(pos, attackRange);
    }

    public void ChainGroundAttack()
    {
        Debug.Log("[BossAttack] ChainGroundAttack called");
        if (groundAttackPrefab == null)
        {
            Debug.LogError("[BossAttack] groundAttackPrefab is not assigned!");
            return;
        }
        StartCoroutine(DoChainGroundAttack());
    }

    private GameObject GetPooledGroundAttack()
    {
        foreach (var obj in groundAttackPool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }
        Debug.LogWarning("Pool habis, tidak ada ground attack yang tersedia");
        return null;
    }

    private IEnumerator DoChainGroundAttack()
    {
        Debug.Log("[BossAttack] DoChainGroundAttack started");
        Vector3 bossPos = transform.position;
        
        // Cari player secara langsung (atau bisa ambil dari referensi jika sudah ada)
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) yield break;
        
        // Tentukan arah dari boss ke player
        float direction = Mathf.Sign(player.position.x - bossPos.x);
        
        for (int i = 0; i < 4; i++)
        {
            float distance = startDistance + (i * stepDistance);
            Vector3 spawnPos = new Vector3(bossPos.x + direction * distance, groundAttackHeight, 0);
            spawnPos.z = 0;
            
            GameObject eruption = GetPooledGroundAttack();
            if (eruption != null)
            {
                eruption.transform.position = spawnPos;
                eruption.SetActive(true);
            }
            yield return new WaitForSeconds(timeBetweenEruptions);
        }
    }
}