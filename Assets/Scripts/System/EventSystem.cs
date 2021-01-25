using System;
using UnityEngine;
public static class EventSystem
{
    public static Action DeathHandler;
    public static Action<GameObject> RespawnHandler; 
    public static Action NewSpawnHandler;
    public static Action RespawnEnemiesHandler;
    public static Action SetDeathFromVoid;
}