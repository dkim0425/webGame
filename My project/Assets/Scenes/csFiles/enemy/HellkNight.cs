using UnityEngine;

public class SkelAOE : BaseUnit
{
    public float attackCooldown = 2f;
    public float attackRadius = 1.5f;

    private float attackTimer = 0f;
    private Animator animator;
    private Transform target;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (currentTarget == null) return;

        Vector2 dir = currentTarget.position - transform.position;
        bool moving = !isAttacking && dir.magnitude > attackRange;

        animator.SetBool("isMoving", moving);

        if (Mathf.Abs(dir.x) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = dir.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    protected override void FixedUpdate()
    {
        if (isAttacking) return;

        SearchClosestEnemy("PlayerUnit");

        if (currentTarget != null)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.position);
            if (distance <= attackRange)
            {
                attackTimer += Time.fixedDeltaTime;
                if (attackTimer >= attackCooldown)
                {
                    animator.SetTrigger("Attack");
                    isAttacking = true;
                    attackTimer = 0f;
                    rb.velocity = Vector2.zero;
                }
            }
            else
            {
                base.FixedUpdate(); // 이동
            }
        }
        else
        {
            base.FixedUpdate(); // 이동
        }
    }

    // ▶ Attack1 끝 프레임에서 호출
    public void DealAOEDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("PlayerUnit"))
            {
                var unit = hit.GetComponent<BaseUnit>();
                if (unit != null)
                    unit.TakeDamage(attackDamage);
            }
        }
    }

    // ▶ Attack2 끝 프레임에서 호출
    public void EndAttack()
    {
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
