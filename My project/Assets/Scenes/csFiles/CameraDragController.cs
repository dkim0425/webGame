// ✅ 올바른 확대/축소 해석 반영: 전체 맵(zoom out)은 고정, zoom in 시 이동 가능
using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    public float dragSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoom = 5f; // 가장 확대된 상태 (유닛 큼)
    public float maxZoom = 15f; // 전체 맵 보기 상태 (유닛 작음)
    public float moveSpeed = 10f;

    public Vector2 minBounds = new Vector2(-20f, -20f);
    public Vector2 maxBounds = new Vector2(20f, 20f);

    private Vector3 dragOrigin;
    private Camera cam;
    private Vector3 defaultPosition;

    void Start()
    {
        cam = Camera.main;
        defaultPosition = cam.transform.position;
    }

    void Update()
    {
        HandleZoom();

        // ✅ 전체 맵 보기 상태일 때는 위치 고정
        if (Mathf.Approximately(cam.orthographicSize, maxZoom))
        {
            cam.transform.position = defaultPosition;
            return;
        }

        // ✅ 세부 확대 상태일 때만 조작 허용
        HandleMouseDrag();
        HandleKeyboardMove();
        ClampCameraPosition();
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 difference = cam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-difference.x * dragSpeed, -difference.y * dragSpeed, 0);

        cam.transform.Translate(move, Space.World);
        dragOrigin = Input.mousePosition;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Debug.Log("Scroll Detected: " + scroll); // ✅ 로그 찍기
            float size = cam.orthographicSize;
            size -= scroll * zoomSpeed;
            size = Mathf.Clamp(size, minZoom, maxZoom);
            cam.orthographicSize = size;
        }
    }


    void HandleKeyboardMove()
    {
        Vector3 move = new Vector3();
        if (Input.GetKey(KeyCode.W)) move.y += 1;
        if (Input.GetKey(KeyCode.S)) move.y -= 1;
        if (Input.GetKey(KeyCode.A)) move.x -= 1;
        if (Input.GetKey(KeyCode.D)) move.x += 1;

        cam.transform.Translate(move.normalized * moveSpeed * Time.deltaTime, Space.World);
    }

    void ClampCameraPosition()
    {
        Vector3 pos = cam.transform.position;

        float vertExtent = cam.orthographicSize;
        float horzExtent = cam.orthographicSize * cam.aspect;

        pos.x = Mathf.Clamp(pos.x, minBounds.x + horzExtent, maxBounds.x - horzExtent);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + vertExtent, maxBounds.y - vertExtent);

        cam.transform.position = pos;
    }
}