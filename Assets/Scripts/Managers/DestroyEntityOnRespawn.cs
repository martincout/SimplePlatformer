using UnityEngine;

public class DestroyEntityOnRespawn : MonoBehaviour
{
    private void OnEnable()
    {
        EventSystems.RespawnEnemiesHandler += DestroyThis;
    }

    private void OnDisable()
    {
        EventSystems.RespawnEnemiesHandler -= DestroyThis;
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
