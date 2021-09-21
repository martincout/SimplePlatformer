using UnityEngine;
using SimplePlatformer.Player;
using System.Collections.Generic;
using System;

[System.Serializable]
public class GameData
{
    public float health;
    public float[] position;
    public bool hasBow;
    public int score;
    public List<bool> campFires;
    public List<bool> cellDoors;
    public List<bool> levelKeys;
    public List<bool> chests;
    //BLUE;RED;YELLOW;GRAY
    public List<int> keys;

    public GameData(PlayerController player, int score, List<CampFire> campFires, List<CellDoor> cellDoors, 
        Dictionary<KeyColor,int> keys, List<GameObject> levelKeys, List<Chest> chests)
    {
        this.campFires = new List<bool>();
        this.cellDoors = new List<bool>();
        this.keys = new List<int>();
        this.levelKeys = new List<bool>();
        this.chests = new List<bool>();
        health = player.healthSystem.GetHealth();
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
        hasBow = player.playerCombatBehaviour.hasBow;
        this.score = score;
        //Clear
        this.campFires.Clear();
        this.cellDoors.Clear();
        this.levelKeys.Clear();
        this.chests.Clear();
        //Save Campfires
        foreach(CampFire c in campFires)
        {
            if (c.interacted)
            {
                this.campFires.Add(true);
            }
            else
            {
                this.campFires.Add(false);
            }
        }
        //Save Celldoors

        foreach (CellDoor c in cellDoors)
        {
            this.cellDoors.Add(c.gameObject.activeSelf);
        }

        //Save keys
        int value = 0;
        //Foreach through the Enum of Key Colors and store it into a List<int> with the amount of keys of each color
        foreach (KeyColor kc in (KeyColor[])Enum.GetValues(typeof(KeyColor)))
        {
            keys.TryGetValue(kc, out value);
            this.keys.Add(value);
        }

        //Save the keys that are in the level (Gameobjects)
        foreach(GameObject child in levelKeys)
        {
            this.levelKeys.Add(child.activeSelf);
        }
        //Foreach chests, is Open or not
        foreach(Chest c in chests)
        {
            this.chests.Add(c.interacted);
        }
    }
}
