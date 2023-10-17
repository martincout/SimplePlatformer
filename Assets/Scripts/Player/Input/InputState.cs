using UnityEngine;

namespace SimplePlatformer.Assets.Scripts.Player.Input
{
    public class InputState
    {
        public Vector2 MovementDirection { get; set; }
        public bool isJumping { get; set; }
        public bool isAttacking { get; set; }
    }
}
