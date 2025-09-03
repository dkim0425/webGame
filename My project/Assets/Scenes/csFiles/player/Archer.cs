using UnityEngine;

public class Archer : BaseUnit
{
    public GameObject projectilePrefab;
    public float fireCooldown = 1.5f;
    private float fireTimer;
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (currentTarget == null) return;

        Vector2 dir = currentTarget.position - transform.position;
        float distance = dir.magnitude;

        bool isMoving = false;

        // 유닛인 경우: 사거리 내에 있으면 정지
        if (currentTarget.CompareTag("EnemyUnit") || currentTarget.CompareTag("PlayerUnit"))
        {
            isMoving = distance > attackRange;
        }
        else
        {
            // 건물/성은 사거리 무시하고 끝까지 가야 함
            isMoving = true;
        }

        animator?.SetBool("isMoving", isMoving);

        // ↔ 좌우 반전
        if (Mathf.Abs(dir.x) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = dir.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }


    protected override void FixedUpdate()
    {
        SearchClosestEnemy("EnemyUnit");

        if (currentTarget == null) return;

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (currentTarget.CompareTag("EnemyUnit"))
        {
            if (distance <= attackRange)
            {
                fireTimer += Time.fixedDeltaTime;
                if (fireTimer >= fireCooldown)
                {
                    FireProjectile();
                    fireTimer = 0f;
                    animator?.SetTrigger("Attack");
                }

                rb.velocity = Vector2.zero;
                return;
            }
        }

        // 건물이나 성일 경우 → 사거리 조건 무시하고 항상 이동
        MoveTowardsTarget();
    }




    void FireProjectile()
    {
        Vector2 dir = (currentTarget.position - transform.position).normalized;
        var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
        proj.SetDirection(dir);
        proj.targetTag = "EnemyUnit";
        proj.damage = attackDamage;
    }
}
