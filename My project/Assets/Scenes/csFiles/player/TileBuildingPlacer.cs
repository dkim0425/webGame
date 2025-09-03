using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class TileBuildingPlacer : MonoBehaviour
{
    public Tilemap baseTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap stuffTilemap;

    public TileBase placeholderTile; // 건물이 설치되었다는 표시용 타일
    public TileBase[] buildableTiles;

    void Update()
    {
        if (BuildingSelector.Instance.currentPreview != null)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = baseTilemap.WorldToCell(mouseWorld);
            BuildingSelector.Instance.currentPreview.transform.position = baseTilemap.GetCellCenterWorld(cell);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            var prefab = BuildingSelector.Instance.selectedBuildingPrefab;
            if (prefab == null) return;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = baseTilemap.WorldToCell(mouseWorld);

            // 장애물 체크
            if (obstacleTilemap.GetTile(cell) != null)
            {
                Debug.Log("건설 불가: 장애물 존재");
                return;
            }

            // Stuff에 이미 건물 존재 → 건설 금지
            if (stuffTilemap.GetTile(cell) != null)
            {
                Debug.Log("건설 불가: 이미 건물 있음");
                return;
            }

            // 지형 체크
            TileBase ground = baseTilemap.GetTile(cell);
            if (!IsBuildable(ground))
            {
                Debug.Log("건설 불가: 지형 부적합");
                return;
            }

            // 골드 확인
            var buildingScript = prefab.GetComponent<Building>();
            if (buildingScript == null || !GoldManager.Instance.SpendGold(buildingScript.goldCost))
            {
                Debug.Log("골드 부족 또는 잘못된 프리팹");
                return;
            }

            // 건물 설치
            Vector3 placePos = baseTilemap.GetCellCenterWorld(cell);
            Instantiate(prefab, placePos, Quaternion.identity);

            // StuffTilemap에 건물 존재 마킹
            if (placeholderTile != null)
                stuffTilemap.SetTile(cell, placeholderTile);

            BuildingSelector.Instance.ClearSelection();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BuildingSelector.Instance.ClearSelection();
        }
    }

    bool IsBuildable(TileBase tile)
    {
        foreach (var t in buildableTiles)
            if (t == tile) return true;
        return false;
    }
}
