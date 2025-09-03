using UnityEngine;
using System.Collections;

public class BaseUnit : MonoBehaviour
{
    public int maxHP = 100;
    protected int currentHP;
    public int attackDamage = 20;
    public float moveSpeed = 2f;
    public float detectRange = 2f;
    public float attackRange = 0.5f;

    protected Rigidbody2D rb;
    protected Transform currentTarget;
    protected Transform castleTarget;

    public GameObject healthBarPrefab;
    private HealthBar healthBarInstance;

    public float separationRadius = 1.5f;
    public float separationWeight = 0.5f;
    public float desiredSpacing = 0.5f;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;

        var castleTag = CompareTag("PlayerUnit") ? "EnemyCastle" : "PlayerCastle";
        castleTarget = GameObject.FindGameObjectWithTag(castleTag).transform;
        currentTarget = castleTarget;

        if (healthBarPrefab != null)
        {
            var hb = Instantiate(healthBarPrefab);
            healthBarInstance = hb.GetComponent<HealthBar>();
            healthBarInstance.Init(transform);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void FixedUpdate()
    {
        if (currentTarget == null) currentTarget = castleTarget;

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (distance > attackRange)
        {
            MoveTowardsTarget();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected void MoveTowardsTarget()
    {
        if (currentTarget == null) currentTarget = castleTarget;

        Vector2 moveDir = (currentTarget.position - transform.position).normalized;
        Vector2 separationDir = CalculateSeparationForce();

        Vector2 finalMove = (moveDir + separationDir * separationWeight).normalized;
        rb.MovePosition(rb.position + finalMove * moveSpeed * Time.fixedDeltaTime);
    }

    protected Vector2 CalculateSeparationForce()
    {
        Vector2 separationForce = Vector2.zero;
        Collider2D[] allies = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (var ally in allies)
        {
            if (ally.gameObject == gameObject) continue;

            if (IsAlly(ally))
            {
                Vector2 away = (Vector2)(transform.position - ally.transform.position);
                float distance = away.magnitude;

                if (distance < desiredSpacing)
                {
                    float strength = (desiredSpacing - distance) / desiredSpacing;
                    separationForce += away.normalized * strength;
                }
            }
        }

        return separationForce;
    }

    protected virtual bool IsAlly(Collider2D col)
    {
        if (CompareTag("PlayerUnit"))
            return col.CompareTag("PlayerUnit") || col.CompareTag("PlayerBuilding") || col.CompareTag("PlayerCastle");
        else
            return col.CompareTag("EnemyUnit") || col.CompareTag("EnemyBuilding") || col.CompareTag("EnemyCastle");
    }


    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashWhite());

        if (currentHP <= 0)
            Die();
    }

    private IEnumerator FlashWhite()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        spriteRenderer.color = originalColor;
    }

    public void TakeHeal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }

    protected virtual void Die()
    {
        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        Destroy(gameObject);
    }

    public virtual void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 성
        var castle = col.GetComponent<Castle>();
        if (castle != null && !IsAlly(col))  // ✅ 아군 구분 추가
        {
            castle.TakeDamage(attackDamage);
            Die();
            return;
        }

        // 건물
        var building = col.GetComponent<Building>();
        if (building != null && !IsAlly(col))  // ✅ 아군 구분 추가
        {
            building.TakeDamage(attackDamage);
            Die();
            return;
        }
    }




    protected void SearchClosestEnemy(string enemyUnitTag)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange);
        float closestDist = Mathf.Infinity;
        Transform closestUnit = null;
        Transform closestStructure = null;
        float closestStructureDist = Mathf.Infinity;

        string enemyBuildingTag = CompareTag("PlayerUnit") ? "EnemyBuilding" : "PlayerBuilding";
        string enemyCastleTag = CompareTag("PlayerUnit") ? "EnemyCastle" : "PlayerCastle";

        foreach (var hit in hits)
        {
            if (hit.CompareTag(enemyUnitTag))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestUnit = hit.transform;
                }
            }

            if (hit.CompareTag(enemyBuildingTag) || hit.CompareTag(enemyCastleTag))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestStructureDist)
                {
                    closestStructureDist = dist;
                    closestStructure = hit.transform;
                }
            }
        }

        if (closestUnit != null)
            currentTarget = closestUnit;
        else if (closestStructure != null)
            currentTarget = closestStructure;
        else
            currentTarget = castleTarget;
    }
}
