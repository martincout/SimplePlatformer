using UnityEngine;

public class RespawnEnemy : MonoBehaviour
{
    private void OnEnable()
    {
        EventSystem.RespawnEnemiesHandler += DestroyThis;
    }

    private void OnDisable()
    {
        EventSystem.RespawnEnemiesHandler -= DestroyThis;
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
