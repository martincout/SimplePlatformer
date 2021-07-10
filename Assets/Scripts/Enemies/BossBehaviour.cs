using SimplePlatformer.ExpandableAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimplePlatformer.Enemy
{
    /// <summary>
    /// Enemy boss behaviour class, simplified the structure of the Enemy Behaviour
    /// </summary>
    public class BossBehaviour : MonoBehaviour
    {
        [Expandable]
        public BossData _bossData;
        protected HealthSystem healthSystem;
        private Rigidbody2D rb2d;
        private Animator anim;
        /// <summary>
        /// Ground Detection
        /// </summary>
        public Transform groundDetector;
        private RaycastHit2D raycastGround;
        [SerializeField] private float rayLength = 0.2f;
        /// <summary>
        /// Player
        /// </summary>
        private Vector2 playerPosition;
        [SerializeField] private GameObject playerGO;


        private float cooldownAttack = 0;

        /// <summary>
        /// Finite State Machine
        /// </summary>
        private enum State
        {
            IDLE,
            CHASING,
            ATTACKING,
            CASTING
        }
        private State _currentState;

        /// <summary>
        /// Boss Phases
        /// </summary>
        private enum Phase
        {
            FIRST,
            SECOND
        }
        private Phase _currentPhase;

        /// <summary>
        /// Custom Behaviours
        /// </summary>
        private bool isAttacking;
        private float countBasicAttacks = 0;

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
            CooldownAttack();
            Movement();
        }

        /// <summary>
        /// Handles all movement behaviour of the boss
        /// </summary>
        private void Movement()
        {
            CheckPlayer();
            Move();

        }

        private void Move()
        {
            if (!isAttacking)
            {
                FlipByTargetDirection(playerPosition.x);
                if (playerPosition.x != 0 && CheckGround())
                {
                    anim.Play(_bossData.animation.enemyMovement);
                    rb2d.velocity = new Vector2(playerPosition.x * _bossData.speed * Time.deltaTime, GetComponent<Rigidbody2D>().velocity.y);
                }
                
                //Idle animation only if we have no ground, because otherwise the enemy will keep walking when it's not suppose to do.
                //Not play idle animation when attacking because it will cancel the attack animation
                if (!CheckGround())
                {
                    anim.Play(_bossData.animation.enemyIdle);
                    rb2d.velocity = Vector2.zero;
                }

            }
            else
            {
                //Stops moving when attacking
                rb2d.velocity = Vector2.zero;
            }
        }

        private void CheckPlayer()
        {
            Collider2D[] boxAttackRadius = Physics2D.OverlapBoxAll(transform.position, _bossData.attackRadius, 0, 1 << LayerMask.NameToLayer("Player"));
            playerPosition = (playerGO.transform.position - transform.position).normalized;

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
            if (!isAttacking)
            {
                StartCoroutine(CooldownAttack());
                isAttacking = true;
                anim.Play(_bossData.animation.enemyAttack[0], -1,0f);
            }
        }

   

        protected bool CheckGround()
        {
            //float rayLength = Mathf.Abs(deltaMove.x) + skinWidth * 2;

            //Vector2 rayOrigin = (directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) +
            //    Vector2.up * deltaMove.y;
            Vector2 rayOrigin = groundDetector.position;
            raycastGround = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, 1 << LayerMask.NameToLayer("Ground"));

            if (raycastGround)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(groundDetector.position, Vector2.down * rayLength);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _bossData.attackRadius);
            Gizmos.color = Color.yellow;
            if (playerGO != null)
            {
                Gizmos.DrawLine(transform.position, playerGO.transform.position);
            }
        }

        private IEnumerator CooldownAttack()
        {
            yield return new WaitForSeconds(_bossData.attackRate);
            //Cooldown Attack
            isAttacking = false;
            FlipByTargetDirection(playerPosition.x);
        }

        /// <summary>
        /// Utils
        /// </summary>
        public void FlipByTargetDirection(float _dirX)
        {
            if (_dirX > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (_dirX < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        public void SwishSound()
        {
            SoundManager.instance.Play(_bossData.swishSound);
        }
    }//End class
}

