using UnityEngine;

public class Spearman : BaseUnit
{
    public float attackCooldown = 1f;
    private float attackTimer = 0f;
    private Animator animator;
    private Vector2 lastMoveDirection;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (currentTarget == null) return;

        // 이동 방향 계산
        Vector2 direction = (currentTarget.position - transform.position);
        float distance = direction.magnitude;
        lastMoveDirection = direction.normalized;

        // 🔁 이동 중 여부 판단 (castle이든 적이든 상관없이)
        bool isMoving = distance > attackRange;
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }

        // ↔ 좌우 반전 처리 (오른쪽 기준)
        if (Mathf.Abs(lastMoveDirection.x) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = lastMoveDirection.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    protected override void FixedUpdate()
    {
        SearchClosestEnemy("EnemyUnit");

        float distance = Vector2.Distance(transform.position, currentTarget.position);
        bool inAttackRange = currentTarget.CompareTag("EnemyUnit") && distance <= attackRange;

        if (inAttackRange)
        {
            attackTimer += Time.fixedDeltaTime;
            if (attackTimer >= attackCooldown)
            {
                var targetUnit = currentTarget.GetComponent<BaseUnit>();
                if (targetUnit != null)
                {
                    targetUnit.TakeDamage(attackDamage);
                    animator?.SetTrigger("Attack");
                }
                attackTimer = 0f;
            }

            // 움직이지 않도록 보장
            rb.velocity = Vector2.zero;
        }
        else
        {
            base.FixedUpdate(); // 이동 및 separation 포함
        }
    }

}
