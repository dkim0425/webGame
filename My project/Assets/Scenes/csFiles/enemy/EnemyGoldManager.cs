// ✅ EnemyGoldManager.cs - 광산 수에 따라 골드 수익 자동 조정
using UnityEngine;

public class EnemyGoldManager : MonoBehaviour
{
    public static EnemyGoldManager Instance { get; private set; }

    public int currentGold = 0;
    public int baseGoldPerSecond = 10; // 기본 골드 수익
    public int goldPerMine = 5; // 광산 1개당 추가 수익

    private float timer = 0f;
    private float tickInterval = 3f; // 1초마다 수익 발생

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tickInterval)
        {
            timer = 0f;
            GenerateGold();
        }
    }

    private void GenerateGold()
    {
        currentGold += baseGoldPerSecond; // ✅ 기본 수익만 추가
        Debug.Log($"[EnemyGoldManager] +{baseGoldPerSecond} gold (No mine bonus), Total: {currentGold}");
    }

    private int CountEnemyMines()
    {
        GameObject[] mines = GameObject.FindGameObjectsWithTag("EnemyGoldMine");
        return mines.Length;
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            return true;
        }
        return false;
    }

    public int GetGold()
    {
        return currentGold;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log($"[EnemyGoldManager] Gold: +{amount}, Total: {currentGold}");
    }
}
