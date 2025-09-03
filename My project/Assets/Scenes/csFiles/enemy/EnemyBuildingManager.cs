using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBuildingManager : MonoBehaviour
{
    [SerializeField] private GameObject[] buildingPrefabs;
    [SerializeField] private float buildInterval = 10f;
    [SerializeField] private Transform enemyCastleTransform;
    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap obstacleTilemap;
    [SerializeField] private Tilemap stuffTilemap;
    [SerializeField] private TileBase placeholderTile;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= buildInterval)
        {
            TryBuild();
            timer = 0f;
        }
    }

    private void TryBuild()
    {
        if (EnemyGoldManager.Instance.GetGold() < 0 || enemyCastleTransform == null || buildingPrefabs.Length < 4)
            return;

        int randomValue = Random.Range(0, 100);

        GameObject buildingToSpawn = null;

        if (randomValue < 30)
            buildingToSpawn = buildingPrefabs[0]; // Melee
        else if (randomValue < 60)
            buildingToSpawn = buildingPrefabs[1]; // Ranged
        else if (randomValue < 75)
            buildingToSpawn = buildingPrefabs[2]; // Healing
        else
            buildingToSpawn = buildingPrefabs[3]; // GoldMine

        if (buildingToSpawn == null)
        {
            Debug.LogWarning("건물 프리팹이 null입니다. Building Prefabs 배열 확인 요망.");
            return;
        }

        Building buildingScript = buildingToSpawn.GetComponent<Building>();
        if (buildingScript == null)
        {
            Debug.LogWarning($"{buildingToSpawn.name} 프리팹에 Building 컴포넌트 없음");
            return;
        }

        int cost = buildingScript.goldCost;
        if (EnemyGoldManager.Instance.GetGold() < cost)
        {
            Debug.Log("적 골드 부족, 건물 건설 실패");
            return;
        }

        Vector3Int castleCell = baseTilemap.WorldToCell(enemyCastleTransform.position);
        List<Vector3Int> candidates = GetNearbyCandidateCells(castleCell);

        foreach (var cell in Shuffle(candidates))
        {
            if (CanBuildAt(cell))
            {
                Vector3 worldPos = stuffTilemap.GetCellCenterWorld(cell);
                Instantiate(buildingToSpawn, worldPos, Quaternion.identity);
                stuffTilemap.SetTile(cell, placeholderTile);
                EnemyGoldManager.Instance.SpendGold(cost);

                Debug.Log($"[EnemyBuildingManager] 적이 {buildingToSpawn.name} 설치 완료. 위치: {cell}, 비용: {cost}, 남은 골드: {EnemyGoldManager.Instance.GetGold()}");
                return;
            }
        }

        Debug.Log("적이 건설 가능한 셀을 찾지 못했음");
    }



    private List<Vector3Int> GetNearbyCandidateCells(Vector3Int origin)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        int[] dx = { -2, -1, -1, -1, 0, 0, 0, 0, 0, 1, 1, 1, 2 };
        int[] dy = { 0, -1, 0, 1, -2, -1, 0, 1, 2, -1, 0, 1, 0 };

        for (int i = 0; i < dx.Length; i++)
        {
            result.Add(new Vector3Int(origin.x + dx[i], origin.y + dy[i], 0));
        }

        return result;
    }

    private bool CanBuildAt(Vector3Int cell)
    {
        if (baseTilemap.GetTile(cell) == null) return false;
        if (obstacleTilemap.GetTile(cell) != null) return false;
        if (stuffTilemap.GetTile(cell) != null) return false;
        return true;
    }

    private void BuildAt(Vector3Int cell)
    {
        Vector3 worldPos = stuffTilemap.GetCellCenterWorld(cell);

        GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
        Instantiate(prefab, worldPos, Quaternion.identity);

        stuffTilemap.SetTile(cell, placeholderTile);
    }

    private List<T> Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }
}