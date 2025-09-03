using UnityEngine;

public class CastleProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float maxLifetime = 5f;
    public float explosionRadius = 1.5f;

    public GameObject visualObject; // ✅ 시각용 자식 오브젝트 (Sprite 또는 Animator)

    private Rigidbody2D rb;
    private Vector2 direction;
    private int damage;
    private string targetTag;

    public void Init(Vector2 dir, int dmg, string tag)
    {
        direction = dir.normalized;
        damage = dmg;
        targetTag = tag;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        // ✅ 물리 이동 시작
        rb.velocity = direction * speed;

        // ✅ 시각적 회전 처리
        if (visualObject != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            visualObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        Destroy(gameObject, maxLifetime);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(targetTag))
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag(targetTag))
            {
                var unit = hit.GetComponent<BaseUnit>();
                if (unit != null)
                {
                    unit.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
