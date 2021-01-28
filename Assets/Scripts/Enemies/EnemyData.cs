using UnityEngine;
using SimplePlatformer.ExpandableAttributes;
namespace SimplePlatformer.Enemy
{
    [CreateAssetMenu(fileName = " New EnemyData", menuName = "Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public Sprite model;
        public GameObject prefab;
        public string enemyName;
        public string description;
        public float maxHealth;
        public float damage = 60f;
        public float speed = 200f;
        //Vision
        [Header("Vision")]
        public float visionRadius = 7f;
        public float visionRadiusUpgrade = 10f;
        public Vector2 attackRadius = new Vector2(2, 2);
        public float attackRange;
        public float attackRate = 1.2f;
        //Stun
        [Header("Stun")]
        public float stunTime = 0.3f;
        public float thrust = 50f;

        public string soundName;

        [Expandable]
        public EnemyAnimationData animation;
    }

}
