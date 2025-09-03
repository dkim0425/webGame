public class GoldMine : Building
{
    public int goldPerTick = 5;
    public float tickInterval = 3f;
    public bool isEnemy = false; // ✅ 소속 구분

    protected override void Activate()
    {
        base.Activate();
        InvokeRepeating(nameof(GenerateGold), 0f, tickInterval);
    }

    private void GenerateGold()
    {
        if (!isBuilt) return;

        if (isEnemy)
        {
            EnemyGoldManager.Instance.AddGold(goldPerTick);
        }
        else
        {
            GoldManager.Instance.AddGold(goldPerTick);
        }
    }
}
