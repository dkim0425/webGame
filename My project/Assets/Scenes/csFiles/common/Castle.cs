using UnityEngine;
using UnityEngine.Tilemaps;

public class Castle : MonoBehaviour
{
    public int maxHP = 1000;
    private int currentHP;

    public GameObject healthBarPrefab;
    private HealthBar healthBarInstance;

    public GameObject projectilePrefab;
    public float attackRange = 5f;
    public float attackInterval = 2f;
    public int attackDamage = 30;

    private float attackTimer = 0f;
    private string enemyTag;

    public Tilemap stuffTilemap;               // ⬅ StuffTilemap 참조
    public TileBase placeholderTile;           // ⬅ 건물 자리 마킹용 타일

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    private void Start()
    {
        currentHP = maxHP;

        // ✅ 위치 셀 중심으로 정렬
        if (stuffTilemap != null)
        {
            Vector3Int cell = stuffTilemap.WorldToCell(transform.position);
            transform.position = stuffTilemap.GetCellCenterWorld(cell);

            // ✅ 중복 방지용 타일 마킹
            if (placeholderTile != null)
                stuffTilemap.SetTile(cell, placeholderTile);
        }

        // ✅ 체력바 생성
        if (healthBarPrefab != null)
        {
            var hb = Instantiate(healthBarPrefab);
            healthBarInstance = hb.GetComponent<HealthBar>();
            healthBarInstance.Init(transform);
        }

        // 공격용 태그 설정
        enemyTag = CompareTag("PlayerCastle") ? "EnemyUnit" : "PlayerUnit";
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            Transform target = FindClosestEnemyInRange();
            if (target != null)
            {
                ShootAt(target);
                attackTimer = 0f;
            }
        }
    }

    private Transform FindClosestEnemyInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float closest = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closest)
                {
                    closest = dist;
                    closestEnemy = hit.transform;
                }
            }
        }

        return closestEnemy;
    }

    private void ShootAt(Transform target)
    {
        Vector3 dir = (target.position - transform.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        CastleProjectile p = proj.GetComponent<CastleProjectile>();
        if (p != null)
        {
            p.Init(dir, attackDamage, enemyTag);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        Destroy(gameObject);
    }
}
