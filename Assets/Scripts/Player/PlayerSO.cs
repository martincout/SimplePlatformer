using SimplePlatformer.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SimplePlatformer.Assets.Scripts.Player
{
    public class PlayerSO : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("The maximum speed that can perform gradually.")]
        public float MaxSpeed = 10f;
        [Tooltip("The acceleration is how fast will the object reach the maximum speed.")]
        public float Acceleration = 5f;
        [Tooltip("The decelaration is how fast will the object reach a speed of 0.")]
        public float Decelaration = 5f;
        [Tooltip("Multipies the fall after jump")] 
        public float FallMultiplier = 2f;
        [Tooltip("When the player Stops Pressing Jump in mid air, multiplies the fall")] 
        public float LowFallMultiplier = 8f;
        [Tooltip("Jump force")] 
        public float JumpForce = 4f;
        [Tooltip("Hang time in the air (Coyote time)")] 
        public float HangTime = 0.2f;
        [Header("Ground")]
        [Tooltip("Ground raycast height.")]
        public float GroundedHeight = 0.5f;
        [Tooltip("Ground raycast offset.")]
        public float HeightOffset = 0.25f; // we dont want to cast from the players feet (may cast underground sometimes), so we offset it a bit
        [Tooltip("Ground Layer")]
        public LayerMask GroundLayer;
        [Tooltip("Left Raycast Offset")]
        public Vector3 RaycastLeftOffset = new Vector2(0.5f, 0);
        [Tooltip("Right Raycast Offset")]
        public Vector3 RaycastRightOffset = new Vector2(0.5f, 0);
        [Header("Combat")]
        [Tooltip("Attack cooldown for the next attack")] 
        public float AttackRate = 0.3f;
        [Tooltip("The damage")] 
        public float AttackDamage = 10f;
        [Tooltip("Initial friction of the character.")] 
        public float InitialDrag;
        [Tooltip("Friction when attacking.")] 
        public float AttackDrag;
        [Tooltip("Arrow Prefab")] 
        public GameObject PF_Arrow;
        [Header("HitBox")]
        public Vector3 HitboxSize;
        public float Rotation;
        [Tooltip("Time to perform the next combo attack.")]
        public float TimeNextCombo = 0.3f;
        [Header("Particles")]
        public GameObject PF_HurtParticle;
        public GameObject PF_FootstepsParticle;
    }
}
