using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Castle playerCastle;
    public Castle enemyCastle;

    private void Update()
    {
        if (playerCastle == null)
        {
            Debug.Log("YOU LOSE");
        }
        if (enemyCastle == null)
        {
            Debug.Log("YOU WIN");
        }
    }
}
