using UnityEngine;
using UnityEngine;
using System.Collections;
using Pathfinding;
using SimplePlatformer.ExpandableAttributes;
using SimplePlatformer.Player;
namespace SimplePlatformer.Enemy
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [Expandable]
        public EnemyData _enemyData;
        protected HealthSystem healthSystem;
        /// <summary>
        /// Items Drop
        /// </summary>
        [Header("Item")]
        public float dropChance = 0.50f;
        public GameObject dropItem;

        [Header("Attack")]
        protected float cooldownAttack = 0f;

        [SerializeField] protected GameObject target;
        private float startStunTime;
        private float stunTimeCooldown = 0;
        protected bool isStunned = false;
        protected bool isAttacking = false;

        //How many hits when checking for hit box
        protected float currentVisionRadius;

        [Header("Stun")]
        protected float dirX;
        protected int currentWaypoint = 0;
        protected bool reachedEndOfPath = false;
        protected float nextWaypointDistance = 3f;

        //Components
        public GameObject particle;
        public Animator anim;
        protected Rigidbody2D rb2d;
        protected GameObject playerGO;
        public Collider2D bodyHitCollider;
        protected GameObject GFX;
        protected RaycastHit2D hitPlayer;
        protected float distanceToTarget;

        /// <summary>
        /// Three main State Behaviours.
        /// </summary>
        protected enum State
        {
            NONE,
            PATROLLING,
            CHASING,
            FRIENDLY,
            DEATH
        }
        protected State currentState;

        /// <summary>
        /// Custom Behaviours of the Enemy
        /// </summary>
        public bool friendly;
        protected bool notFollow;
        protected bool sawPlayer;
        protected bool isPatrolling;
        public bool patrollingEnabled = true;
        public bool headingRight;
        public bool knockbackDontInterruptAttack = false;


        protected virtual void Start()
        {
            playerGO = GameObject.FindGameObjectWithTag("Player");
            target = playerGO;
            GFX = transform.GetChild(0).gameObject;
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            healthSystem = GetComponent<HealthSystem>();
            startStunTime = _enemyData.stunTime;
            cooldownAttack = 0;
            healthSystem.SetMaxHealth(_enemyData.maxHealth);
            currentVisionRadius = _enemyData.visionRadius;
            SetInitialState();
        }

        /// <summary>
        /// Set the Initial state acording to custom behaviours, if one of them is true
        /// </summary>
        private void SetInitialState()
        {
            if (friendly)
            {
                currentState = State.FRIENDLY;
            }
            else
            {
                if (patrollingEnabled)
                {
                    currentState = State.PATROLLING;
                }
                else
                {
                    currentState = State.NONE;
                }

            }
        }

        protected bool FollowPlayer()
        {
            if (notFollow)
            {
                return false;
            };
            int mask1 = 1 << LayerMask.NameToLayer("Player");
            int mask2 = 1 << LayerMask.NameToLayer("Ground");
            if (target != null)
            {
                //working
                hitPlayer = Physics2D.Raycast(transform.position, target.transform.position - transform.position, Mathf.Infinity, mask1 | mask2);
                if (hitPlayer)
                {
                    distanceToTarget = Vector2.Distance(transform.position, playerGO.transform.position);
                    //If it is the player and if it is in the range of vision
                    if (hitPlayer.collider.gameObject.CompareTag("Player") && distanceToTarget < currentVisionRadius)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        private void OnEnable()
        {
            EventSystems.RespawnHandler += UpdatePlayer;
        }
        private void OnDisable()
        {
            EventSystems.RespawnHandler -= UpdatePlayer;
        }

        private void UpdatePlayer(GameObject player)
        {
            target = player;
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
            notFollow = currentState.Equals(State.DEATH) || GameManager.GetInstance().playerDeath || GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GetPlayerItsDying();
            StunTimeReset();
            CooldownAttack();

        }

        protected virtual void Move()
        {
            //NOTHING
        }

        protected void RunCooldownAttackTimer()
        {
            cooldownAttack = _enemyData.attackRate;
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
            if (col.CompareTag("Player") && !currentState.Equals(State.DEATH) && !GameManager.GetInstance().playerDeath)
            {
                col.GetComponent<IDamageable>().TakeDamage(_enemyData.damage, transform.position);
            }
        }

        #endregion

        #region Take Damage
        public void TakeDamage(float damage, Vector3 attackerPos)
        {
            if (!currentState.Equals(State.DEATH))
            {
                healthSystem.DealDamage(damage);
                if (healthSystem.GetHealth() > 0)
                {
                    SoundManager.instance.Play(_enemyData.damageSound);
                    anim.Play(_enemyData.animation.enemyHurt, 1, 0);
                    //Usefull stuff
                    if (!knockbackDontInterruptAttack)
                    {
                        PlayAnimation(_enemyData.animation.enemyIdle);
                    }
                    //Knockback
                    if (!isStunned)
                    {
                        isAttacking = false;
                        isStunned = true;
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
            currentState = State.DEATH;
            PlayHurtParticle();
            SoundManager.instance.Play("Death");
            StopAllCoroutines();
            StartCoroutine(DieCo());
            if (dropItem != null)
            {
                if (Random.Range(0f, 1f) <= dropChance)
                {
                    Instantiate(dropItem, new Vector2(transform.position.x, Mathf.Round(transform.position.y + 0.2f)), Quaternion.identity);
                }
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
            cooldownAttack = 0;
            Vector2 forceDirection = transform.TransformDirection(transform.position - playerPos);
            if (forceDirection.x > 0)
            {
                forceDirection = new Vector2(1, rb2d.velocity.y);
            }
            else
            {
                forceDirection = new Vector2(-1, rb2d.velocity.y);

            }
            rb2d.velocity = Vector2.zero;
            Vector2 force = forceDirection * _enemyData.thrust;
            rb2d.AddForce(force, ForceMode2D.Impulse);
            yield return new WaitForSeconds(_enemyData.stunTime);
            rb2d.velocity = Vector2.zero;
        }

        private IEnumerator DieCo()
        {
            rb2d.velocity = Vector2.zero;
            rb2d.simulated = false;
            anim.Play(_enemyData.animation.enemyDeath);
            if(bodyHitCollider!=null) bodyHitCollider.enabled = false;
            yield return new WaitForSeconds(0.55f);
            enabled = false;
            rb2d.velocity = Vector2.zero;

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

        protected void FlipByHeadingDirection(bool _headingRight)
        {
            if (_headingRight)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        protected void FlipByVelocity()
        {
            if (rb2d.velocity.x > 0.02f)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (rb2d.velocity.x < 0.02f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        protected void FlipByTargetDirection(float _dirX)
        {
            if (_dirX > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (_dirX < 0)
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
            Gizmos.color = Color.yellow;
            if (playerGO != null)
            {
                Gizmos.DrawLine(transform.position, playerGO.transform.position);
            }
        }

    }


}

