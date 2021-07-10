using SimplePlatformer.ExpandableAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimplePlatformer.Enemy
{
    public class BossBehaviour : MonoBehaviour
    {
        [Expandable]
        public BossData _bossData;
        protected HealthSystem healthSystem;

    }
}

