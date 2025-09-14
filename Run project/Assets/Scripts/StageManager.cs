using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Hierarchy")]
    [SerializeField] private Transform gridContainer;   // "Grid Container"
    [SerializeField] private GameObject playerPrefab;   // 플레이어 프리팹
    [SerializeField] private string nextSceneName = "MainMenuScene";

    [Header("Tiles (네가 팔레트에서 쓰는 타일 에셋들)")]
    [SerializeField] private TileBase spawnPointRuleTile; // 스폰용 RuleTile
    [SerializeField] private TileBase goalRuleTile;       // 골(클리어) RuleTile

    // 내부 상태
    private readonly List<Transform> stages = new();
    private int currentStageIndex = 0;
    private GameObject player;

    // 골 타일 빠른 판별용
    private readonly List<Tilemap> goalTilemaps = new();
    private readonly List<HashSet<Vector3Int>> goalCellsPerMap = new();
    private bool goalLocked = false; // 중복 트리거 방지

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (!gridContainer || !playerPrefab || !spawnPointRuleTile || !goalRuleTile)
        {
            Debug.LogError("[StageManager] 참조 누락: gridContainer / playerPrefab / spawnPointRuleTile / goalRuleTile 확인");
            enabled = false; return;
        }
        for (int i = 0; i < gridContainer.childCount; i++)
            stages.Add(gridContainer.GetChild(i));
        if (stages.Count == 0) { Debug.LogError("[StageManager] 스테이지 없음"); enabled = false; }
    }

    void Start() => StartStage(0);

    void Update()
    {
        // 플레이어가 골 RuleTile 위에 올라섰는지 검사
        if (player && !goalLocked)
        {
            Vector3 worldPos = player.transform.position;

            // 필요하면 "발 위치" 기준으로 약간 아래를 보도록 오프셋(예: -0.1f) 줄 수 있음
            Vector3 probe = worldPos + Vector3.down * 0.1f;

            for (int i = 0; i < goalTilemaps.Count; i++)
            {
                var tm = goalTilemaps[i];
                var cell = tm.WorldToCell(probe);
                if (goalCellsPerMap[i].Contains(cell))
                {
                    goalLocked = true;   // 한 번만 처리
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

        // 스테이지 표시 전환
        for (int i = 0; i < stages.Count; i++)
            stages[i].gameObject.SetActive(i == currentStageIndex);

        // 골 타일 목록 재구성
        BuildGoalTileCache(stages[currentStageIndex]);

        // 스폰 위치 계산 후 배치
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
            else Debug.Log("모든 스테이지 클리어");
            return;
        }
        StartStage(next);
    }

    // ====== Spawn: 스테이지의 모든 타일맵에서 SpawnPoint RuleTile을 찾는다 ======
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
        Debug.LogWarning($"[StageManager] {stageRoot.name}에 SpawnPoint RuleTile 없음. 스테이지 루트로 스폰");
        return stageRoot.position;
    }

    // ====== Goal: 스테이지의 모든 타일맵에서 Goal RuleTile 좌표를 캐시 ======
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
