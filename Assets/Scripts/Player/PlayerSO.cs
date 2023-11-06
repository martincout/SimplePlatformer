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
    }
}
