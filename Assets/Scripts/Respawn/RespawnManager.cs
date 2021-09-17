using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public List<CampFire> respawns { get; set; }
    public static GameObject currentRespawn;

    public void Awake()
    {
        //Get all the campfire gameobjects
        respawns = new List<CampFire>();
        foreach (Transform child in transform)
        {
            respawns.Add(child.GetComponent<CampFire>());
        }
    }
    /// <summary>
    /// This function is invoked in the GameManager class
    /// </summary>
    public void SetSpawn(bool loading)
    {
        if (!loading)
        {
            //New game
            currentRespawn = respawns[0].gameObject; //First respawn
        }
        else
        {
            int maxCampfires = respawns.Count;
            List<bool> campfiresBool = GlobalControl.Instance.LocalCopyOfData.campFires;
            //Loading
            for (int i = 0; i < maxCampfires; i++)
            {
                //If the campfire is true, then it has been activated or interacted with it
                respawns[i].GetComponent<CampFire>().interacted = campfiresBool[i];
                //Sets the current spawn finding the last campfire lighted
                if (campfiresBool[i] == false && campfiresBool[i - 1] == true)
                {
                    currentRespawn = respawns[i - 1].gameObject;
                }else if(campfiresBool[i] == true)
                {
                    currentRespawn = respawns[i].gameObject;
                }
            }
            
        }

    }
}
