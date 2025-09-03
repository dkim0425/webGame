using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class TileBuildingPlacer : MonoBehaviour
{
    public Tilemap baseTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap stuffTilemap;

    public TileBase placeholderTile; // �ǹ��� ��ġ�Ǿ��ٴ� ǥ�ÿ� Ÿ��
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

            // ��ֹ� üũ
            if (obstacleTilemap.GetTile(cell) != null)
            {
                Debug.Log("�Ǽ� �Ұ�: ��ֹ� ����");
                return;
            }

            // Stuff�� �̹� �ǹ� ���� �� �Ǽ� ����
            if (stuffTilemap.GetTile(cell) != null)
            {
                Debug.Log("�Ǽ� �Ұ�: �̹� �ǹ� ����");
                return;
            }

            // ���� üũ
            TileBase ground = baseTilemap.GetTile(cell);
            if (!IsBuildable(ground))
            {
                Debug.Log("�Ǽ� �Ұ�: ���� ������");
                return;
            }

            // ��� Ȯ��
            var buildingScript = prefab.GetComponent<Building>();
            if (buildingScript == null || !GoldManager.Instance.SpendGold(buildingScript.goldCost))
            {
                Debug.Log("��� ���� �Ǵ� �߸��� ������");
                return;
            }

            // �ǹ� ��ġ
            Vector3 placePos = baseTilemap.GetCellCenterWorld(cell);
            Instantiate(prefab, placePos, Quaternion.identity);

            // StuffTilemap�� �ǹ� ���� ��ŷ
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
