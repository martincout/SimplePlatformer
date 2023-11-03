using UnityEngine;

namespace SimplePlatformer.Assets.Scripts.Player.Input
{
    public class InputState
    {
        public Vector2 MovementDirection { get; set; }
        public bool IsJumping { get; set; }
        public bool CanJump { get; set; }
        public bool CanAttack { get; set; }
    }
}
