﻿using UnityEngine;
using System.Collections;
using Pathfinding;
using SimplePlatformer.ExpandableAttributes;
using SimplePlatformer.Player;
namespace SimplePlatformer.Enemy
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [ExpandableAttribute]
        public EnemyData _enemyData;
        protected HealthSystem healthSystem;
        [Header("Item")]
        public float dropChance = 0.50f;
        public GameObject dropItem;
        //GameObject of the graphics
        protected GameObject GFX;
        [Header("Attack")]
        protected float cooldownAttack = 0f;

        [SerializeField] protected Transform target;


        [Header("Stun")]

        private float startStunTime;
        private float stunTimeCooldown = 0;
        protected bool isStunned = false;
        protected bool isAttacking = false;
        protected bool itsDying = false;
        protected float dirX;
        protected int currentWaypoint = 0;
        protected bool reachedEndOfPath = false;
        protected float nextWaypointDistance = 3f;

        public GameObject particle;
        protected Animator anim;
        protected Rigidbody2D rb2d;
        //How many hits when checking for hit box
        [HideInInspector] public int manyHits = 1;
       
        public bool friendly;
        protected GameObject playerGO;

        /// <summary>
        /// Don't follow the Player if enabled. Updates itself when the player die or this enemy die
        /// </summary>
        protected bool notFollow = false;

        protected virtual void Start()
        {
            playerGO = GameObject.FindGameObjectWithTag("Player");
            GFX = transform.GetChild(0).gameObject;
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            healthSystem = GetComponent<HealthSystem>();
            startStunTime = _enemyData.stunTime;
            cooldownAttack = 0;
            healthSystem.SetMaxHealth(_enemyData.maxHealth);

        }

        private void OnEnable()
        {
            EventSystem.RespawnHandler += UpdatePlayer;
        }
        private void OnDisable()
        {
            EventSystem.RespawnHandler -= UpdatePlayer;
        }

        private void UpdatePlayer(GameObject player)
        {
            target = player.transform;
        }

        protected virtual void FixedUpdate()
        {

            if (!notFollow && !friendly)
            {
                Move();
            }
        }

        protected virtual void Update()
        {
            notFollow = itsDying || LevelManager.instance.isPlayerDead || GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>().GetPlayerItsDying();
            StunTimeReset();
            CooldownAttack();
        }

        protected virtual void Move()
        {
            //NOTHING
        }

        protected void CooldownAttack()
        {
            //Cooldown Attack
            if (cooldownAttack > 0)
            {
                cooldownAttack -= Time.deltaTime;
            }
            else
            {
                isAttacking = false;
            }
        }


        protected void PlayAnimation(string name)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(name)) { return; }
            anim.Play(name);
        }

        private void StunTimeReset()
        {
            if (stunTimeCooldown > 0)
            {
                isStunned = true;
                stunTimeCooldown -= Time.deltaTime;
            }
            else
            {
                isStunned = false;
            }
        }

        #region Attack



        /// <summary>
        /// Damage to the Character
        /// </summary>
        /// <param name="col"></param>
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !notFollow)
            {
                col.GetComponent<IDamageable>().TakeDamage(_enemyData.damage, transform.position);
            }
        }

        #endregion

        #region Take Damage
        public void TakeDamage(float damage, Vector3 attackerPos)
        {
            if (!itsDying)
            {
                healthSystem.DealDamage(damage);
                if (healthSystem.GetHealth() > 0)
                {
                    SoundManager.instance.Play(_enemyData.soundName);
                    anim.Play(_enemyData.animation.enemyHurt, 1, 0);
                    PlayAnimation(_enemyData.animation.enemyIdle);
                    if (!isStunned)
                    {
                        StopCoroutine(KnockCo(attackerPos));
                        StartCoroutine(KnockCo(attackerPos));
                    }
                    Vector3 dir = transform.position - attackerPos;
                    dir = transform.position + dir.normalized;
                    PlayHurtParticle();
                }
                else
                {
                    Die();
                    
                }
            }

        }


        protected void Die()
        {
            PlayHurtParticle();
            SoundManager.instance.Play("Death");
            StopAllCoroutines();
            StartCoroutine(DieCo());
            if (dropItem != null)
            {
                Instantiate(dropItem, new Vector2(transform.position.x, Mathf.Round(transform.position.y + 0.2f)), Quaternion.identity);
            }
        }

        private void PlayHurtParticle()
        {
            if (particle != null)
            {
                GameObject instance = Instantiate(particle, transform.position, Quaternion.identity);
                Destroy(instance, 1f);
            }
        }

        private IEnumerator KnockCo(Vector3 playerPos)
        {
            stunTimeCooldown = startStunTime;
            isAttacking = false;
            Vector2 forceDirection = transform.TransformDirection(transform.position - playerPos);
            if(forceDirection.x > 0)
            {
                forceDirection = new Vector2(1, forceDirection.y);
            }
            else
            {
                forceDirection = new Vector2(-1, forceDirection.y);

            }
            Vector2 force = forceDirection * _enemyData.thrust;
            rb2d.AddForce(force, ForceMode2D.Impulse);
            yield return new WaitForSeconds(_enemyData.stunTime);
            rb2d.velocity = new Vector2();
        }

        private IEnumerator DieCo()
        {
            itsDying = true;
            rb2d.velocity = new Vector2();
            anim.Play(_enemyData.animation.enemyDeath);
            yield return new WaitForSeconds(0.55f);
            rb2d.isKinematic = true;
            GetComponent<Collider2D>().enabled = false;
            enabled = false;

        }

        public void DieInstantly()
        {
            healthSystem.SetHealth(0);
            GameObject instance = Instantiate(particle, transform.position, Quaternion.identity);
            SoundManager.instance.Play("Death");
            StopAllCoroutines();
            StartCoroutine(DieCo());
            Destroy(instance, 1f);

        }

        #endregion

        protected void Flip()
        {
            if (rb2d.velocity.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if(rb2d.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        
        protected void Flip(float _dirX)
        {
            if (_dirX > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if(_dirX < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _enemyData.visionRadius);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, _enemyData.visionRadiusUpgrade);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _enemyData.attackRadius);

        }


    }


}

