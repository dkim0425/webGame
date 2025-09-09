using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
    }
}
