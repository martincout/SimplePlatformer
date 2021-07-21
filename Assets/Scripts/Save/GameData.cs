using UnityEngine;
using SimplePlatformer.Player;

[System.Serializable]
public class GameData
{
    public float health;
    public float[] position;
    public bool hasBow;
    public int score;

    public GameData(PlayerController player, int score)
    {
        health = player.healthSystem.GetHealth();
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
        hasBow = player.playerCombatBehaviour.hasBow;
        this.score = score;
    }
}
