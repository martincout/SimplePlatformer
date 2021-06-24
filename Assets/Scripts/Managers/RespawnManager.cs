using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private List<CampFire> respawns;
    public static GameObject currentRespawn; 

    public void Awake()
    {
        respawns = new List<CampFire>();
        foreach(Transform child in transform)
        {
            respawns.Add(child.GetComponent<CampFire>());
        }
        currentRespawn = respawns[0].gameObject; //First respawn
    }
}
