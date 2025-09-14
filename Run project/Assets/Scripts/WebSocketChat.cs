using UnityEngine;
using WebSocketSharp;

[System.Serializable]
public class HelloMsg
{
    public string type = "hello"; // 메시지 타입
    public string id;             // 클라이언트/플레이어 식별자
}

public class WebSocketChat : MonoBehaviour
{
    private WebSocket ws;

    void Start()
    {
        Debug.Log("Start 실행됨");

        ws = new WebSocket("ws://13.238.197.137:8000/ws/chat");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Connected");
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

    void OnDestroy()
    {
        if (ws != null)
        {
            try { ws.Close(); } catch { /* ignore */ }
            ws = null;
        }
    }
}
