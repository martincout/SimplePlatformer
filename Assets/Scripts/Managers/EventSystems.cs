using System;
using UnityEngine;
public static class EventSystems
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
    /// <summary>
    /// Updates the Keys obtained
    /// </summary>
    public static Action UpdateKeysHandler;
    public static Action OnPlayerDeath;
}