using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraDragAndZoom : MonoBehaviour
{
    public float dragSpeed = 5f;

    [Header("Zoom Settings (Pixel Perfect)")]
    public float[] zoomLevels = { 2f, 3f, 4f, 6f };
    private int currentZoomIndex = 1;

    private Camera cam;
    private Vector3 dragOrigin;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = zoomLevels[currentZoomIndex];
    }

    void Update()
    {
        HandleDrag();
        HandleZoom();
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(1)) // 우클릭 시작
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1)) // 우클릭 드래그 중
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position += difference;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f && currentZoomIndex > 0)
        {
            currentZoomIndex--;
            cam.orthographicSize = zoomLevels[currentZoomIndex];
        }
        else if (scroll < 0f && currentZoomIndex < zoomLevels.Length - 1)
        {
            currentZoomIndex++;
            cam.orthographicSize = zoomLevels[currentZoomIndex];
        }
    }
}
