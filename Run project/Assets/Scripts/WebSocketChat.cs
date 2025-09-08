

using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string id;
    public float x, y, z;
}

[System.Serializable]
public class ReceivedData
{
    public string from;
    public PlayerData data;
}

public class WebSocketChat : MonoBehaviour
{
    WebSocket ws;
    public Transform PlayerTransform;
    public string playerId = "player1"; // 각 클라이언트 고유 ID
    public GameObject PlayerPrefab;      // 다른 플레이어 오브젝트
    private Dictionary<string, GameObject> otherPlayers = new Dictionary<string, GameObject>();

    void Start()
    {
        ws = new WebSocket("ws://13.238.197.137:8000/ws/chat");


        ws.OnMessage += (sender, e) =>
        {
            ReceivedData received = JsonUtility.FromJson<ReceivedData>(e.Data);
            Debug.Log(received.from + " 로부터 받은 좌표: " +
                      received.data.x + "," +
                      received.data.y + "," +
                      received.data.z);

            // 다른 플레이어 생성/이동
            if (!otherPlayers.ContainsKey(received.from))
            {
                GameObject newPlayer = Instantiate(PlayerPrefab);
                otherPlayers[received.from] = newPlayer;
            }
            otherPlayers[received.from].transform.position =
                new Vector3(received.data.x, received.data.y, received.data.z);
        };

        ws.Connect();
    }

    void Update()
    {
        if (ws.ReadyState == WebSocketState.Open)
        {
            PlayerData myData = new PlayerData()
            {
                id = playerId,
                x = PlayerTransform.position.x,
                y = PlayerTransform.position.y,
                z = PlayerTransform.position.z
            };
            string json = JsonUtility.ToJson(myData);
            ws.Send(json);
        }
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }
}
