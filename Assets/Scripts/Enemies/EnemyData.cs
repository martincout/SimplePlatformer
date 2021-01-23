using UnityEngine;
using SimplePlatformer.ExpandableAttributes;
namespace SimplePlatformer.Enemy
{
    [CreateAssetMenu(fileName = " New EnemyData", menuName = "Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public string enemyName;
        public string description;
        public float maxHealth;
        public Sprite model;
        public GameObject prefab;
        public float speed = 200f;
        public float visionRadius = 7f;
        public Vector2 attackRadius = new Vector2(2, 2);
        public float attackRange;
        public float attackRate = 1.2f;
        public float damage = 60f;
        //Stun
        public float stunTime = 0.3f;
        public float thrust = 50f;

        [Expandable]
        public EnemyAnimationData animation;
    }

}
