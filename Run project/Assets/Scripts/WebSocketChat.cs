using UnityEngine;
using WebSocketSharp;

[System.Serializable]
public class HelloMsg
{
    public string type = "hello"; // 메시지 타입
    public string id;             // 클라이언트/플레이어 식별자
}

[System.Serializable]
public class PositionMsg
{
    public string id;
    public float x;
    public float y;
    public float z;
}

public class WebSocketChat : MonoBehaviour
{
    private WebSocket ws;
    public Transform PlayerPos;

    // 과도한 전송 방지: 10Hz(0.1초) 간격으로 위치 전송
    [SerializeField] private float sendInterval = 0.1f;
    private float nextSendTime = 0f;

    // 플레이어 식별자 (필요시 인스펙터에서 변경)
    [SerializeField] private string playerId = "player";

    void Start()
    {
        Debug.Log("Start 실행됨");

        ws = new WebSocket("ws://13.238.197.137:8000/ws/chat");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Connected");

            // ✅ 최초 인사도 JSON으로 전송
            var hello = new HelloMsg { id = playerId };
            ws.Send(JsonUtility.ToJson(hello));
        };

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("서버로부터 받은 메시지: " + e.Data);
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket Error: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Closed");
        };

        ws.Connect();
    }

    void Update()
    {
        if (Time.time >= nextSendTime)
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
            Debug.Log(json);

            nextSendTime = Time.time + sendInterval;
        }
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            try { ws.Close(); } catch { /* ignore */ }
            ws = null;
        }
    }
}
