using System;
using UnityEngine;
public static class EventSystem
{
    /// <summary>
    /// Handles the player respawn. After it gets Instantiated
    /// </summary>
    public static Action<GameObject> RespawnHandler; 
    /// <summary>
    /// Player got a new Respawn waypoint
    /// </summary>
    public static Action NewSpawnHandler;
    public static Action RespawnEnemiesHandler;
    public static Action SetDeathFromVoid;
    /// <summary>
    /// Updates the Keys obtained
    /// </summary>
    public static Action UpdateKeysHandler;
}