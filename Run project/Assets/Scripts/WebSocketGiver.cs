using UnityEngine;
using WebSocketSharp;

public class WebSocketGiver : MonoBehaviour
{
    public Transform target;                         // 위치를 보낼 대상
    [SerializeField] private string playerId = "player1";
    [SerializeField] private string serverUrl = "ws://13.238.197.137:8000/ws/chat";
    [SerializeField] private float sendInterval = 0.1f;

    private WebSocket ws;
    private float nextSend;

    void Start()
    {
        if (target == null) target = this.transform;

        ws = new WebSocket(serverUrl);
        ws.OnOpen += (s, e) => Debug.Log("[Giver] Connected");
        ws.OnError += (s, e) => Debug.LogWarning("[Giver] Error: " + e.Message);
        ws.OnClose += (s, e) => Debug.Log("[Giver] Closed");
        ws.Connect();
    }

    void Update()
    {
        if (Time.time < nextSend || ws == null || ws.ReadyState != WebSocketState.Open) return;

        Vector3 p = target.position;
        PositionMsgGive msg = new PositionMsgGive { id = playerId, x = p.x, y = p.y, z = p.z };
        ws.Send(JsonUtility.ToJson(msg));
        Debug.Log(msg);
        nextSend = Time.time + sendInterval;
    }

    void OnDestroy() { if (ws != null) ws.Close(); }
}
