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
    public class BossBehaviour : MonoBehaviour, IDamageable
    {
        [Expandable]
        public BossData _bossData;
        protected HealthSystem healthSystem;
        private Rigidbody2D rb2d;
        private Animator anim;
        public Transform fireballsPosition;
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
       


        /// <summary>
        /// Finite State Machine
        /// </summary>
        public enum State
        {
            NONE,
            START
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
        private int countBasicAttacks = 1;
        public GameObject fireLight;

        private void Start()
        {
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.SetMaxHealth(_bossData.maxHealth);
            rb2d = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            _currentState = State.NONE;
            _currentPhase = Phase.FIRST;
            countBasicAttacks = 1;
        }
        private void Update()
        {
            if (_currentState.Equals(State.NONE)) return;
            Movement();
        }

        public void ChangeState(State state)
        {
            _currentState = state;
        }

        #region Movement Behaviour

        private bool CheckGround()
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
        #endregion
        #region Attack
        private void Attack()
        {
            if (!isAttacking)
            {
                isAttacking = true;
                //Basic Attacks
                if (countBasicAttacks <= 2)
                {
                    StartCoroutine(CooldownAttack(_bossData.attackRate));
                    anim.Play(_bossData.animation.enemyAttack[0]);
                    countBasicAttacks += 1;
                    return;
                }
                //Casting Attack
                if(countBasicAttacks > 2 && countBasicAttacks <= 4)
                {
                    StartCoroutine(CooldownAttack(3));
                    anim.Play(_bossData.animation.enemyAttack[1]);
                    //FIreballs
                    StartCoroutine(CreateProjectile(.4f));
                    StartCoroutine(CreateProjectile(.6f,new Vector2(4,-3)));
                    StartCoroutine(CreateProjectile(.8f, new Vector2(-4,-3)));
                    countBasicAttacks += 1;
                    return;
                }
                //Restart
                if(countBasicAttacks == 5)
                {
                    StartCoroutine(CooldownAttack(0.1f));
                    countBasicAttacks = 1;
                    return;
                }
            }
        }

        private IEnumerator CreateProjectile(float _seconds, Vector2 _Offset)
        {
            //Set the position - With an X Offset
            Vector2 position = new Vector2(fireballsPosition.position.x + _Offset.x, fireballsPosition.position.y + _Offset.y);
            //Wait
            Instantiate(fireLight, position,Quaternion.identity);
            yield return new WaitForSeconds(_seconds);
            //Instantiate
            GameObject instance = Instantiate(_bossData.projectileGO, position, _bossData.projectileGO.transform.rotation);
            //Custom parameters
            instance.GetComponent<Fireball>().speed = _bossData.projectileSpeed;
            instance.GetComponent<Fireball>().damage = _bossData.projectileDamage;

        }

        private IEnumerator CreateProjectile(float _seconds)
        {
            //Set the position - With an X Offset
            Vector2 position = new Vector2(fireballsPosition.position.x, fireballsPosition.position.y);
            //Wait
            Instantiate(fireLight, position, Quaternion.identity);
            yield return new WaitForSeconds(_seconds);
            //Instantiate
            GameObject instance = Instantiate(_bossData.projectileGO, position, _bossData.projectileGO.transform.rotation);
            //Custom parameters
            instance.GetComponent<Fireball>().speed = _bossData.projectileSpeed;
            instance.GetComponent<Fireball>().damage = _bossData.projectileDamage;

        }

        private IEnumerator CooldownAttack(float _seconds)
        {
            yield return new WaitForSeconds(_seconds);
            //Cooldown Attack
            isAttacking = false;
            FlipByTargetDirection(playerPosition.x);
        }
        #endregion
        

        public void DisplayHealthBar()
        {
            LeanTween.alphaCanvas(healthSystem.healthBar.GetComponent<CanvasGroup>(), 1.0f, 1f);
        }

        #region Debug
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
        #endregion
        

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

        public void TakeDamage(float damage, Vector3 attackerPosition)
        {
            healthSystem.DealDamage(damage);
            StartCoroutine(HurtCo());
        }

        private IEnumerator HurtCo()
        {
            anim.Play(_bossData.animation.enemyHurt,1,0);
            yield return new WaitForSeconds(0.3f);
        }

        public void DieInstantly()
        {
            //He nothing
        }
    }//End class
}

