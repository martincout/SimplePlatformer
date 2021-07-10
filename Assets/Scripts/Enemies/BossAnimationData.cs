using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SimplePlatformer.Enemy
{
    [CreateAssetMenu(fileName = " New BossAnimationData", menuName = "Data/Boss Animation Data")]
    public class BossAnimationData : ScriptableObject
    {
        public string enemyIdle;
        public string enemyMovement;
        public string[] enemyAttack;
        public string enemyHurt;
        public string enemyDeath;
    }
}
