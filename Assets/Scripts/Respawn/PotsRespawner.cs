using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotsRespawner : MonoBehaviour
{
    private List<RespawnEntityData> pots;
    public GameObject potPrefab;

    // Start is called before the first frame update
    void Start()
    {
        pots = new List<RespawnEntityData>();
        foreach (Transform child in transform)
        {
            pots.Add(new RespawnEntityData(child.GetComponent<Destroyable>(),child.position));
        }
    }

    private void OnEnable()
    {
        EventSystems.RespawnHandler += Respawn;
    }

    private void OnDisable()
    {
        
        EventSystems.RespawnHandler -= Respawn;
    }

    private void Respawn(GameObject player)
    {
        foreach (RespawnEntityData ed in pots)
        {
            GameObject instance = Instantiate(potPrefab, ed.position, Quaternion.identity, transform);
            instance.GetComponent<Destroyable>().drop = ed.destroyable.drop;
            instance.GetComponent<Destroyable>().dropChance = ed.destroyable.dropChance;
        }

    }

}
