using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    public int currentGold = 500;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log($"[GoldManager] Gold: +{amount}, Total: {currentGold}");
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            Debug.Log($"[GoldManager] Gold Spent: -{amount}, Remaining: {currentGold}");
            return true;
        }
        Debug.Log("[GoldManager] Not enough gold!");
        return false;
    }

    public int GetGold()
    {
        return currentGold;
    }
}