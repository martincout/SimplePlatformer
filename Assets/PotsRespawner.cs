using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotsRespawner : MonoBehaviour
{
    private List<Vector3> potsPos;
    public GameObject potPrefab;

    // Start is called before the first frame update
    void Start()
    {
        potsPos = new List<Vector3>();
        foreach(Transform child in transform)
        {
            potsPos.Add(child.transform.position);
        }
    }

    private void OnEnable()
    {
        EventSystem.RespawnHandler += Respawn;
    }

    private void OnDisable()
    {
        
        EventSystem.RespawnHandler -= Respawn;
    }

    private void Respawn(GameObject player)
    {
        foreach(Vector3 potPos in potsPos)
        {
            Instantiate(potPrefab, potPos, Quaternion.identity,transform);
        }

    }

}
