using UnityEngine;
using WebSocketSharp;

public class WebSocketChat : MonoBehaviour
{
    WebSocket ws;
    public Transform PlayerPos;
    void Start()
    {
        Debug.Log("Start �����");

        ws = new WebSocket("ws://13.238.197.137:8000/ws/chat");

        // �����κ��� �޽��� ���� ��
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("�����κ��� ���� �޽���: " + e.Data);
        };

        ws.Connect();

        // ������ �޽��� ����
        ws.Send("�ȳ� �� ����Ƽ");
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