using UnityEngine;
using WebSocketSharp;

public class WebSocketChat : MonoBehaviour
{
    WebSocket ws;
    public Transform PlayerPos;

    void Start()
    {
        Debug.Log("Start 실행됨");

        ws = new WebSocket("ws://13.238.197.137:8000/ws/chat");

        // 서버로부터 메시지 수신 시
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("서버로부터 받은 메시지: " + e.Data);
        };

        ws.Connect();

        // 서버로 메시지 전송
        ws.Send("안녕 난 유니티");
    }

    void Update()
    {
        ws.Send("" + PlayerPos.position);
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }
}
