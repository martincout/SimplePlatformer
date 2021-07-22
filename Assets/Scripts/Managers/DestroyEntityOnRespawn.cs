using UnityEngine;

public class DestroyEntityOnRespawn : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.RespawnEnemiesHandler += DestroyThis;
    }

    private void OnDisable()
    {
        GameEvents.RespawnEnemiesHandler -= DestroyThis;
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
