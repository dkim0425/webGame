// ✅ RangedSpawner.cs (수정 버전)
using UnityEngine;

public class RangedSpawner : Building
{
    public GameObject rangedUnitPrefab;
    public float spawnInterval = 5f;
    public Transform spawnPoint;
    public string targetCastleTag = "EnemyCastle"; // 기본값은 EnemyCastle

    protected override void Activate()
    {
        base.Activate();
        InvokeRepeating(nameof(SpawnRangedUnit), 0f, spawnInterval);
    }

    private void SpawnRangedUnit()
    {
        if (!isBuilt || rangedUnitPrefab == null || spawnPoint == null) return;

        GameObject unit = Instantiate(rangedUnitPrefab, spawnPoint.position, Quaternion.identity);
        BaseUnit unitScript = unit.GetComponent<BaseUnit>();
        if (unitScript != null)
        {
            GameObject targetCastle = GameObject.FindGameObjectWithTag(targetCastleTag);
            if (targetCastle != null)
                unitScript.SetTarget(targetCastle.transform);
        }
    }
}
