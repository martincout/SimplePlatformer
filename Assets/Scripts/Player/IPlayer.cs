using UnityEngine;

namespace SimplePlatformer.Assets.Scripts.Player
{
    public interface IPlayer 
    { 
        Vector2 WorldPosition { get; }
    }
}