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
        #region Variables
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
        /// Basic States
        /// </summary>
        public enum State
        {
            WAITING,
            START,
            DEAD
        }
        private State _currentState;

        /// <summary>
        /// Boss Stages
        /// </summary>
        private enum Stage
        {
            STAGE_1,
            STAGE_2,
            STAGE_3
        }
        private Stage _currentStage;

        /// <summary>
        /// Attack Types: basic, cast, etc..
        /// </summary>
        private enum AttackType
        {
            BASIC_ATTACK,
            CAST_FIREBALLS,
            CAST_HANDS
        }

        private AttackType _currentAttack;

        /// <summary>
        /// Custom Behaviours
        /// </summary>
        private bool isAttacking;
        //Counts the amount of attacks did it for a certain Type of Attack
        private int countAttacks = 0;
        private int maxAttacks = 1;
        public GameObject fireLight;
        #endregion

        private void Start()
        {
            playerGO = GameManager.GetInstance().player.gameObject;
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.SetMaxHealth(_bossData.maxHealth);
            rb2d = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            _currentState = State.WAITING;
            _currentStage = Stage.STAGE_1;
        }
        private void Update()
        {
            if (_currentState.Equals(State.WAITING) || _currentState.Equals(State.DEAD) || GameManager.GetInstance().playerDeath) return;
            CheckPlayer();
        }

        private void FixedUpdate()
        {
            if (_currentState.Equals(State.WAITING) || _currentState.Equals(State.DEAD) || GameManager.GetInstance().playerDeath) return;
            Movement();
        }



        #region Movement Behaviour

        private bool CheckGround()
        {
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

        private void Movement()
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
            if (!isAttacking)
            {
                //Player IN RANGE
                if (boxAttackRadius.Length > 0)
                {
                    Attack();
                }
            }

        }
        #endregion
        #region Attack
        /// <summary>
        /// Handles the Attack of the enemy.
        /// - Attack Animation
        /// - Counter of Attacks
        /// - Next Attack to perform
        /// </summary>
        private void Attack()
        {
            isAttacking = true;
            //Basic Attack
            if (_currentAttack.Equals(AttackType.BASIC_ATTACK))
            {
                maxAttacks = 2;
                BasicAttack();
                //Counter
                countAttacks += 1;
                NextAttack();
                return;
            }
            //Casting Fireballs Attack
            if (_currentAttack.Equals(AttackType.CAST_FIREBALLS))
            {
                maxAttacks = 2;
                FireballsAttack();
                //Counter
                countAttacks += 1;
                NextAttack();
                return;
            }
            //Mage Hand Attack
            if (_currentAttack.Equals(AttackType.CAST_HANDS))
            {
                maxAttacks = 1;
                MageHandAttack();
                //Counter
                countAttacks += 1;
                NextAttack();
                return;
            }
        }

        private void FireballsAttack()
        {
            StartCoroutine(CooldownAttack(3));
            anim.Play(_bossData.animation.enemyAttack[1]);
            //FIreballs
            StartCoroutine(CreateProjectile(.4f));
            StartCoroutine(CreateProjectile(.6f, new Vector2(4, -3)));
            StartCoroutine(CreateProjectile(.8f, new Vector2(-4, -3)));

        }

        private void MageHandAttack()
        {
            StartCoroutine(CooldownAttack(3));
            anim.Play(_bossData.animation.enemyAttack[1]);

        }

        private void BasicAttack()
        {
            //Attack Behaviour
            StartCoroutine(CooldownAttack(_bossData.attackRate));
            anim.Play(_bossData.animation.enemyAttack[0]);

        }

        private IEnumerator CreateProjectile(float _seconds, Vector2 _Offset)
        {
            //Set the position - With an X Offset
            Vector2 position = new Vector2(fireballsPosition.position.x + _Offset.x, fireballsPosition.position.y + _Offset.y);
            //Wait
            Instantiate(fireLight, position, Quaternion.identity);
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
            isAttacking = false;
            FlipByTargetDirection(playerPosition.x);
        }
        #endregion
        #region Stages & Next Attacks

        public void ChangeState(State state)
        {
            _currentState = state;
        }

        private void StartNextStage()
        {
            switch (_currentStage)
            {
                case Stage.STAGE_1:
                    _currentStage = Stage.STAGE_2;
                    break;
                case Stage.STAGE_2:
                    _currentStage = Stage.STAGE_3;
                    break;
            }
        }

        /// <summary>
        /// Change the current type of attack, only if has performed the amount of attacks
        /// set previously
        /// - Handles Stage Check
        /// </summary>
        /// <param name="at"></param>
        private void NextAttack()
        {
            if (countAttacks >= maxAttacks)
            {
                countAttacks = 0;
                maxAttacks = 1;

                switch (_currentAttack)
                {
                    case AttackType.BASIC_ATTACK:
                        _currentAttack = AttackType.CAST_FIREBALLS;
                        break;
                    case AttackType.CAST_FIREBALLS:
                        #region Attack Type Following the Stage
                        if (_currentStage.Equals(Stage.STAGE_1))
                        {
                            _currentAttack = AttackType.BASIC_ATTACK;
                        }
                        else
                        {
                            _currentAttack = AttackType.CAST_HANDS;
                        }
                        #endregion
                        break;
                }
            }
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
            if (healthSystem.GetHealth() > 0)
            {
                SoundManager.instance.Play("Damage");
                healthSystem.DealDamage(damage);
                StartCoroutine(HurtCo());
            }
            else
            {
                if (!_currentState.Equals(State.DEAD))
                {
                    Die();
                }
            }
        }

        private void Die()
        {
            SoundManager.instance.Play("DeathBoss");
            ChangeState(State.DEAD);
            anim.Play(_bossData.animation.enemyDeath);
            Destroy(gameObject, 1f);
        }

        private IEnumerator HurtCo()
        {
            anim.Play(_bossData.animation.enemyHurt, 1, 0);
            yield return new WaitForSeconds(0.3f);
        }

        public void DieInstantly()
        {
            //He nothing
        }

    }//End class
}

