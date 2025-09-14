using UnityEngine;
using WebSocketSharp;

[System.Serializable]
public class PositionMsg
{
    public string id;
    public float x;
    public float y;
    public float z;
}

public class WebSocketGiver : MonoBehaviour
{
    private WebSocket ws;

    [Header("Tracked object to send position of")]
    public Transform PlayerPos;

    [Header("Message settings")]
    [SerializeField] private string playerId = "player";
    [SerializeField] private float sendInterval = 1f; // 초 단위 전송 주기

    private float nextSendTime = 0f;

    void Start()
    {
        if (PlayerPos == null)
        {
            Debug.LogError("[WebSocketGiver] PlayerPos가 설정되지 않았습니다.");
            enabled = false;
            return;
        }

        Debug.Log("[WebSocketGiver] Start");
        ws = new WebSocket("ws://13.238.197.137:8000/ws/chat");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("[WebSocketGiver] WebSocket Connected");
        };

        // 수신 콜백은 Start에서 ‘한 번만’ 등록
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("[WebSocketGiver] 서버로부터 받은 메시지: " + e.Data);
            // 필요시: PositionMsg msg = JsonUtility.FromJson<PositionMsg>(e.Data);
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogWarning("[WebSocketGiver] WebSocket Error: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("[WebSocketGiver] WebSocket Closed");
        };

        ws.Connect();
    }

    void Update()
    {
        if (Time.time < nextSendTime) return;

        // 연결 상태 확인 후 전송
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            Vector3 p = PlayerPos.position;

            var posMsg = new PositionMsg
            {
                id = playerId,
                x = p.x,
                y = p.y,
                z = p.z
            };

            string json = JsonUtility.ToJson(posMsg);
            ws.Send(json);
            Debug.Log("[WebSocketGiver] 보낸 위치: " + json);
        }
        else
        {
            Debug.LogWarning("[WebSocketGiver] 연결이 열려있지 않아 전송 스킵");
        }

        nextSendTime = Time.time + sendInterval;
    }

    void OnApplicationQuit()
    {
        Cleanup();
    }

    void OnDestroy()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        if (ws != null)
        {
            try { ws.Close(); } catch { /* ignore */ }
            ws = null;
        }
    }
}
