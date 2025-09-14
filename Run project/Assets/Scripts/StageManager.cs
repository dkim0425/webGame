using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Hierarchy")]
    [SerializeField] private Transform gridContainer;   // "Grid Container"
    [SerializeField] private GameObject playerPrefab;   // �÷��̾� ������
    [SerializeField] private string nextSceneName = "MainMenuScene";

    [Header("Tiles (�װ� �ȷ�Ʈ���� ���� Ÿ�� ���µ�)")]
    [SerializeField] private TileBase spawnPointRuleTile; // ������ RuleTile
    [SerializeField] private TileBase goalRuleTile;       // ��(Ŭ����) RuleTile

    // ���� ����
    private readonly List<Transform> stages = new();
    private int currentStageIndex = 0;
    private GameObject player;

    // �� Ÿ�� ���� �Ǻ���
    private readonly List<Tilemap> goalTilemaps = new();
    private readonly List<HashSet<Vector3Int>> goalCellsPerMap = new();
    private bool goalLocked = false; // �ߺ� Ʈ���� ����

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (!gridContainer || !playerPrefab || !spawnPointRuleTile || !goalRuleTile)
        {
            Debug.LogError("[StageManager] ���� ����: gridContainer / playerPrefab / spawnPointRuleTile / goalRuleTile Ȯ��");
            enabled = false; return;
        }
        for (int i = 0; i < gridContainer.childCount; i++)
            stages.Add(gridContainer.GetChild(i));
        if (stages.Count == 0) { Debug.LogError("[StageManager] �������� ����"); enabled = false; }
    }

    void Start() => StartStage(0);

    void Update()
    {
        // �÷��̾ �� RuleTile ���� �ö󼹴��� �˻�
        if (player && !goalLocked)
        {
            Vector3 worldPos = player.transform.position;

            // �ʿ��ϸ� "�� ��ġ" �������� �ణ �Ʒ��� ������ ������(��: -0.1f) �� �� ����
            Vector3 probe = worldPos + Vector3.down * 0.1f;

            for (int i = 0; i < goalTilemaps.Count; i++)
            {
                var tm = goalTilemaps[i];
                var cell = tm.WorldToCell(probe);
                if (goalCellsPerMap[i].Contains(cell))
                {
                    goalLocked = true;   // �� ���� ó��
                    OnGoalReached();
                    break;
                }
            }
        }
    }

    public void OnGoalReached() => NextStage();

    public void OnPlayerDied()
    {
        Vector3 spawn = FindSpawnWorldPosition(stages[currentStageIndex]);
        SpawnOrMovePlayer(spawn);
    }

    private void StartStage(int index)
    {
        currentStageIndex = Mathf.Clamp(index, 0, stages.Count - 1);

        // �������� ǥ�� ��ȯ
        for (int i = 0; i < stages.Count; i++)
            stages[i].gameObject.SetActive(i == currentStageIndex);

        // �� Ÿ�� ��� �籸��
        BuildGoalTileCache(stages[currentStageIndex]);

        // ���� ��ġ ��� �� ��ġ
        Vector3 spawn = FindSpawnWorldPosition(stages[currentStageIndex]);
        SpawnOrMovePlayer(spawn);

        goalLocked = false;
    }

    private void NextStage()
    {
        int next = currentStageIndex + 1;
        if (next >= stages.Count)
        {
            if (!string.IsNullOrEmpty(nextSceneName)) SceneManager.LoadScene(nextSceneName);
            else Debug.Log("��� �������� Ŭ����");
            return;
        }
        StartStage(next);
    }

    // ====== Spawn: ���������� ��� Ÿ�ϸʿ��� SpawnPoint RuleTile�� ã�´� ======
    private Vector3 FindSpawnWorldPosition(Transform stageRoot)
    {
        foreach (var tm in stageRoot.GetComponentsInChildren<Tilemap>(true))
        {
            var b = tm.cellBounds;
            for (int y = b.yMax - 1; y >= b.yMin; y--)
            {
                for (int x = b.xMin; x < b.xMax; x++)
                {
                    var p = new Vector3Int(x, y, 0);
                    if (tm.GetTile(p) == spawnPointRuleTile)
                        return tm.GetCellCenterWorld(p);
                }
            }
        }
        Debug.LogWarning($"[StageManager] {stageRoot.name}�� SpawnPoint RuleTile ����. �������� ��Ʈ�� ����");
        return stageRoot.position;
    }

    // ====== Goal: ���������� ��� Ÿ�ϸʿ��� Goal RuleTile ��ǥ�� ĳ�� ======
    private void BuildGoalTileCache(Transform stageRoot)
    {
        goalTilemaps.Clear();
        goalCellsPerMap.Clear();

        foreach (var tm in stageRoot.GetComponentsInChildren<Tilemap>(true))
        {
            var b = tm.cellBounds;
            HashSet<Vector3Int> set = null;

            for (int y = b.yMin; y < b.yMax; y++)
            {
                for (int x = b.xMin; x < b.xMax; x++)
                {
                    var p = new Vector3Int(x, y, 0);
                    if (tm.GetTile(p) == goalRuleTile)
                    {
                        if (set == null) set = new HashSet<Vector3Int>();
                        set.Add(p);
                    }
                }
            }

            if (set != null)
            {
                goalTilemaps.Add(tm);
                goalCellsPerMap.Add(set);
            }
        }
    }

    private void SpawnOrMovePlayer(Vector3 pos)
    {
        if (player == null)
        {
            player = Instantiate(playerPrefab, pos, Quaternion.identity);
            player.tag = "Player";
        }
        else
        {
            player.transform.SetPositionAndRotation(pos, Quaternion.identity);
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb) rb.velocity = Vector2.zero;
        }
    }
}
