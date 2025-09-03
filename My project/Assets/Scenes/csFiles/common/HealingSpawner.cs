using UnityEngine;

public class HealingSpawner : Building
{
    public GameObject healingUnitPrefab;
    public string targetCastleTag = "EnemyCastle"; // 기본은 플레이어 유닛 기준

    public float spawnInterval = 5f;
    private float timer = 0f;

    protected override void Activate()
    {
        base.Activate();
    }

    private void Update()
    {
        if (!isBuilt) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnUnit();
        }
    }

    private void SpawnUnit()
    {
        var unit = Instantiate(healingUnitPrefab, transform.position, Quaternion.identity);
        if (unit != null)
        {
            unit.GetComponent<BaseUnit>()?.SetTarget(GameObject.FindGameObjectWithTag(targetCastleTag).transform);
        }
    }
}
