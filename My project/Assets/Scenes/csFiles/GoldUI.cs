using UnityEngine;
using UnityEngine.UI;

public class GoldUI : MonoBehaviour
{
    public Text goldText;

    void Update()
    {
        if (goldText != null && GoldManager.Instance != null)
        {
            goldText.text = $"Gold: {GoldManager.Instance.GetGold()}";
        }
    }
}
