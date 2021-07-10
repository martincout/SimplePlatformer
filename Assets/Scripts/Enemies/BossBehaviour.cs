using SimplePlatformer.ExpandableAttributes;
using System;
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
        private Rigidbody2D rb2d;
        private Animator anim;
        private GameObject playerGO;

        /// <summary>
        /// Movement
        /// </summary>
        

        private enum State
        {
            IDLE,
            CHASING,
            ATTACKING,
            CASTING
        }
        private State _currentState;

        private enum Phase
        {
            FIRST,
            SECOND
        }
        private Phase _currentPhase;

        /// <summary>
        /// Custom Behaviours of the Enemy
        /// </summary>

        private void Start()
        {
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.SetMaxHealth(_bossData.maxHealth);
            rb2d = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            _currentState = State.IDLE;
            _currentPhase = Phase.FIRST;
        }

        private void Update()
        {
            Movement();
        }

        private void Movement()
        {
            CheckPlayer();
            Move();
        }

        private void Move()
        {
            
        }

        private void CheckPlayer()
        {
            Collider2D[] boxAttackRadius = Physics2D.OverlapBoxAll(transform.position, _bossData.attackRadius, 0, 1 << LayerMask.NameToLayer("Player"));
            Vector3 dir = (playerGO.transform.position - transform.position).normalized;

            //Check if there is a player in the attack radius
            if (boxAttackRadius.Length > 0)
            {
                //Attack
                foreach (Collider2D col in boxAttackRadius)
                {
                    if (col.CompareTag("Player"))
                    {
                        Attack();
                    }
                }
            }
        }

        private void Attack()
        {

        }
    }
}

