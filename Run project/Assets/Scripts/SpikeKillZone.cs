using UnityEngine;

public class SpikeKillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(other.gameObject);
            StageManager.Instance?.OnPlayerDied();
        }
    }
}
