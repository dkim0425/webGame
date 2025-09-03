using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    public static BuildingSelector Instance { get; private set; }

    public GameObject selectedBuildingPrefab;
    public GameObject currentPreview;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SelectBuilding(GameObject buildingPrefab)
    {
        selectedBuildingPrefab = buildingPrefab;
        Debug.Log($"선택된 건물: {buildingPrefab.name}");

        CreatePreview();
    }

    private void CreatePreview()
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        if (selectedBuildingPrefab != null)
        {
            currentPreview = new GameObject("BuildingPreview");

            SpriteRenderer originalSprite = selectedBuildingPrefab.GetComponent<SpriteRenderer>();
            if (originalSprite != null)
            {
                SpriteRenderer previewSprite = currentPreview.AddComponent<SpriteRenderer>();
                previewSprite.sprite = originalSprite.sprite;
                previewSprite.sortingOrder = originalSprite.sortingOrder;

                currentPreview.transform.localScale = Vector3.one * 0.5f;

                // ✅ 투명도를 더 낮춘다 (약간 비치게)
                var color = previewSprite.color;
                color.a = 0.4f; // 40% 불투명 (맵이 자연스럽게 보임)
                previewSprite.color = color;
            }
        }
    }

    public void ClearSelection()
    {
        selectedBuildingPrefab = null;
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
        Debug.Log("건물 설치 선택 취소");
    }
}
