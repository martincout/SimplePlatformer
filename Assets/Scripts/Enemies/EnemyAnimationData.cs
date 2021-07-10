using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Enemy
{
    [CreateAssetMenu(fileName = " New EnemyAnimationData", menuName = "Data/Enemy Animation Data")]
    public class EnemyAnimationData : ScriptableObject
    {
        public string enemyIdle;
        public string enemyMovement;
        public string enemyAttack;
        public string enemyHurt;
        public string enemyDeath;
    }
}