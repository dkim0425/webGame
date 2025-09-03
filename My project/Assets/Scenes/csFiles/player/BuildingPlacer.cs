using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    public LayerMask placementLayer;

    void Update()
    {
        if (BuildingSelector.Instance.currentPreview != null)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            BuildingSelector.Instance.currentPreview.transform.position = mouseWorldPos;
        }

        if (Input.GetMouseButtonDown(0)) // 좌클릭
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (BuildingSelector.Instance.selectedBuildingPrefab != null)
            {
                Debug.Log("건물 설치 시도!");
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, placementLayer);

                if (hit == null)
                {
                    Building buildingScript = BuildingSelector.Instance.selectedBuildingPrefab.GetComponent<Building>();

                    if (buildingScript != null && GoldManager.Instance.SpendGold(buildingScript.goldCost))
                    {
                        Instantiate(BuildingSelector.Instance.selectedBuildingPrefab, mouseWorldPos, Quaternion.identity);
                        BuildingSelector.Instance.ClearSelection(); // ✅ 설치 완료 후 초기화
                    }
                    else
                    {
                        Debug.Log("골드가 부족하거나 잘못된 프리팹입니다.");
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (BuildingSelector.Instance.selectedBuildingPrefab != null)
            {
                BuildingSelector.Instance.ClearSelection(); // ✅ ESC로 초기화
            }
        }
    }
}
