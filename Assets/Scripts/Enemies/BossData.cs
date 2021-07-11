using SimplePlatformer.ExpandableAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SimplePlatformer.Enemy
{
    [CreateAssetMenu(fileName = " New BossData", menuName = "Data/Boss Data")]
    public class BossData : ScriptableObject
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
        public Vector2 attackRadius = new Vector2(2, 2);
        public float attackRange;
        public float attackRate = 1.2f;
        //Stun
        [Header("Stun")]
        public float stunTime = 0.3f;
        public float thrust = 50f;

        public string damageSound;
        public string swishSound;
        public GameObject projectileGO;
        public float projectileSpeed;
        public float projectileDamage;
        [Expandable]
        public BossAnimationData animation;
    }
}
