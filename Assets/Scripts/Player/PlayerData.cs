using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimplePlatformer
{
    [CreateAssetMenu(fileName = "New Player Data", menuName = "Player/Player Data")]
    public class PlayerData : ScriptableObject
    {
        public float MaxHealth = 200;
        //Action Maps
        public string actionMapPlayerControls = "PlayerControlls";
        public string actionMapMenuControls = "UI";
        //Other
        public float InvincibleTime = 1f;
        public float StunTime = 0.3f;
        public HealthBar HealthBar;
        [Tooltip("Multipies the fall after jump"), SerializeField] private float FallMultiplier = 2f;
        [Tooltip("When the player Stops Pressing Jump in mid air, multiplies the fall"), SerializeField] private float LowFallMultiplier = 8f;
        [SerializeField] private float JumpForce = 4f;
        [Tooltip("Hang time in the air. (Coyote Time)"), SerializeField] private float HangTime = 0.2f;
        [Tooltip("Hang time counter, decreasing value"), SerializeField] private float HangTimeCounter = 0f;
        //Check Ground
        public float GroundedHeight = 0.5f;
        public float HeightOffset = 0.25f; // we dont want to cast from the players feet (may cast underground sometimes), so we offset it a bit
        public LayerMask GroundLayer;
        // Offset from the center-bottom of the collider 2d
        public Vector3 RayCastLeftOffset = new Vector2(0.5f, 0);
        public Vector3 RaycastRightOffset = new Vector2(0.5f, 0);
        [Tooltip("The acceleration is how fast will the object reach the maximum speed.")]
        [SerializeField] public float Acceleration = 5f;
        [Tooltip("The decelaration is how fast will the object reach a speed of 0.")]
        [SerializeField] public float Decelaration = 5f;
        [Tooltip("The maximum speed that can perform gradually.")]
        [SerializeField] public float MaxSpeed = 10f;
    }
}
