using UnityEngine;
using SimplePlatformer.Player;

[System.Serializable]
public class PlayerData
{
    public float health;
    public float[] position;

    public PlayerData(PlayerBase player)
    {
        health = player.healthSystem.GetHealth();
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
