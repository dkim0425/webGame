using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float maxDistance = 10f;
    public int damage = 10;
    public string targetTag = "EnemyUnit";

    private Vector2 direction;
    private Vector3 startPosition;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        startPosition = transform.position;

        // 회전: Sprite가 "위쪽"을 기본으로 보고 있다는 전제 하에
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(targetTag))
        {
            var unit = col.GetComponent<BaseUnit>();
            if (unit != null)
            {
                unit.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
