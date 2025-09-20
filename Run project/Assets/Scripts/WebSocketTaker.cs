using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;

public class WebSocketTaker : MonoBehaviour
{
    public Transform target;                          // 수신 좌표를 적용할 대상
    [SerializeField] private string serverUrl = "ws://13.238.197.137:8000/ws/chat";
    [SerializeField] private string listenId = "player1"; // 받을 플레이어 id

    private WebSocket ws;
    private readonly Queue<PositionMsgTake> recvQueue = new Queue<PositionMsgTake>();

    void Start()
    {
        if (target == null) target = this.transform;

        ws = new WebSocket(serverUrl);
        ws.OnOpen += (s, e) => Debug.Log("[Taker] Connected");
        ws.OnMessage += (s, e) =>
        {
            Debug.Log("[Taker] Message Connected");
            try
            {
                PositionMsgTake msg = JsonUtility.FromJson<PositionMsgTake>(e.Data);
                if (msg != null && msg.from == listenId)
                {
                    lock (recvQueue) recvQueue.Enqueue(msg);
                }
            }
            catch { /* 수신 형식이 다르면 무시 */ }
        };
        ws.OnError += (s, e) => Debug.LogWarning("[Taker] Error: " + e.Message);
        ws.OnClose += (s, e) => Debug.Log("[Taker] Closed");
        ws.Connect();
    }

    void Update()
    {
        lock (recvQueue)
        {
            while (recvQueue.Count > 0)
            {
                PositionMsgTake m = recvQueue.Dequeue();
                target.position = new Vector3(m.x, m.y, m.z);
            }
        }
    }

    void OnDestroy() { if (ws != null) ws.Close(); }
}
