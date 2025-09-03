using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public int maxHP = 100;
    protected int currentHP;

    public int goldCost = 100;
    public float buildTime = 1.0f;

    protected bool isBuilt = false;

    // 🔹 체력바 관련 추가
    public GameObject healthBarPrefab;
    private HealthBar healthBarInstance;

    protected virtual void Start()
    {
        currentHP = maxHP;
        Invoke(nameof(Activate), buildTime);

        // 🔹 체력바 생성 및 초기화
        if (healthBarPrefab != null)
        {
            var hb = Instantiate(healthBarPrefab);
            healthBarInstance = hb.GetComponent<HealthBar>();
            healthBarInstance.Init(transform);
        }
    }

    protected virtual void Activate()
    {
        isBuilt = true;
    }

    public virtual void TakeDamage(int damage)
    {
        Debug.Log($"Building {name} taking damage: {damage}, currentHP: {currentHP}");
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Debug.Log($"Building {name} destroyed!");
            Die();
        }
    }


    protected virtual void Die()
    {
        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        Destroy(gameObject);
    }

    // 체력 접근용 프로퍼티 (HealthBar용)
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
}
