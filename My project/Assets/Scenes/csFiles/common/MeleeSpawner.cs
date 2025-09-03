// ✅ MeleeSpawner.cs (수정 버전)
using UnityEngine;

public class MeleeSpawner : Building
{
    public GameObject meleeUnitPrefab;
    public float spawnInterval = 5f;
    public Transform spawnPoint;
    public string targetCastleTag = "EnemyCastle"; // 기본값은 EnemyCastle

    protected override void Activate()
    {
        base.Activate();
        InvokeRepeating(nameof(SpawnMeleeUnit), 0f, spawnInterval);
    }

    private void SpawnMeleeUnit()
    {
        if (!isBuilt || meleeUnitPrefab == null || spawnPoint == null) return;

        GameObject unit = Instantiate(meleeUnitPrefab, spawnPoint.position, Quaternion.identity);
        BaseUnit unitScript = unit.GetComponent<BaseUnit>();
        if (unitScript != null)
        {
            GameObject targetCastle = GameObject.FindGameObjectWithTag(targetCastleTag);
            if (targetCastle != null)
                unitScript.SetTarget(targetCastle.transform);
        }
    }
}