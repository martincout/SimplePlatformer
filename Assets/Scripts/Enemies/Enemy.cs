using UnityEngine;
using System.Collections;
using Pathfinding;
using SimplePlatformer.ExpandableAttributes;
namespace SimplePlatformer.Enemy
{
    public class Enemy : MonoBehaviour, IDamagable
    {
        [ExpandableAttribute]
        public EnemyData _edata;
        protected HealthSystem healthSystem;
        protected GameObject GFX;
        [Header("Attack")]
        private float cooldownAttack = 0f;
        [SerializeField] private float attackRange = 0.2f;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private bool checkForHitBox = false;
        [Header("Stun")]
        [SerializeField] private float noStunTime = 10f;
        private float thrust = 50f;
        private float startStunTime;
        private bool isStunned = false;
        private bool isAttacking = false;
        private bool itsDying = false;
        private float dirX;
        [Header("PathFinding")]
        [SerializeField] protected Transform target;
        protected Path path;
        protected Seeker seeker;
        protected int currentWaypoint = 0;
        protected bool reachedEndOfPath = false;
        protected float nextWaypointDistance = 3f;
        [SerializeField] private float rayLength = 0.2f;
        private Transform groundDetector;
        public GameObject particle;
        private Animator anim;
        protected Rigidbody2D rb2d;

        public virtual void Start()
        {
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            healthSystem = GetComponent<HealthSystem>();
            seeker = GetComponent<Seeker>();
            startStunTime = _edata.stunTime;
            groundDetector = transform.GetChild(2)?.GetComponent<Transform>();
        }



        protected virtual void FixedUpdate()
        {
            CheckHitBox();
            if (!itsDying && !LevelManager.instance.isPlayerDead)
            {
                Move();
            }
        }

        private void CheckHitBox()
        {
            if (checkForHitBox)
            {
                if (!itsDying)
                {
                    Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, 1 << LayerMask.NameToLayer("Player"));

                    foreach (Collider2D col in hits)
                    {
                        if (col.GetComponent<IDamagable>() != null)
                        {
                            col.GetComponent<IDamagable>().TakeDamage(_edata.damage, transform.position);
                        }
                    }
                }
            }
        }

        private bool CheckGround()
        {
            //float rayLength = Mathf.Abs(deltaMove.x) + skinWidth * 2;

            //Vector2 rayOrigin = (directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) +
            //    Vector2.up * deltaMove.y;
            Vector2 rayOrigin = groundDetector.position;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, 1 << LayerMask.NameToLayer("Ground"));
            if (!hit)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        protected virtual void Update()
        {
            StunTimeReset();
            CooldownAttack();
        }

        private void CooldownAttack()
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


        private void PlayAnimation(string name)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(name)) { return; }
            anim.Play(name);
        }

        private void StunTimeReset()
        {

            //Not stunned
            if (_edata.stunTime < 0)
            {
                _edata.stunTime -= Time.deltaTime;
                if (_edata.stunTime < -noStunTime)
                {
                    _edata.stunTime = startStunTime;
                }
            }
        }

        #region Attack

        protected virtual void Move()
        {
            #region Find Player
            //Vision radius
            Collider2D[] circleVision = Physics2D.OverlapCircleAll(transform.position, _edata.visionRadius, 1 << LayerMask.NameToLayer("Player"));
            Collider2D[] boxAttackRadius = Physics2D.OverlapBoxAll(transform.position, _edata.attackRadius, 0, 1 << LayerMask.NameToLayer("Player"));

            Vector3 target = transform.position;
            bool foundPlayer = false;
            foreach (Collider2D col in circleVision)
            {
                if (col.CompareTag("Player"))
                {
                    target = col.transform.position;
                    foundPlayer = true;
                    break;
                }
                foundPlayer = false;
            }


            if (!foundPlayer)
            {
                return;
            }

            RaycastHit2D raycastTarget = Physics2D.Raycast(transform.position, target);
            Debug.DrawLine(transform.position, target);
            #endregion
            #region Direction & Distance
            //Calculate the current distance to the target
            float distance = Vector3.Distance(target, transform.position);
            //Direction: Returns a normalized vector (1,-1)
            Vector3 dir = (target - transform.position).normalized;


            if (CheckGround())
            {
                dirX = dir.x;
            }
            else
            {
                if (!isAttacking) PlayAnimation("skeletonIdle");
                dirX = 0;
            }

            //if (dir.x > 0)
            //{
            //    dirX = Mathf.Ceil(dir.x);
            //}
            //else if (dir.x < 0)
            //{
            //    dirX = Mathf.Floor(dir.x);
            //}

            #endregion

            if (!isStunned && !isAttacking)
            {
                #region Chase and Attack
                //Flip the sprite
                Flip(dir);
                //Raycast target checking the vision radius
                if (raycastTarget.distance < distance)
                {
                    //Check if there is a player in the attack radius
                    if (boxAttackRadius.Length > 0)
                    {
                        foreach (Collider2D col in boxAttackRadius)
                        {
                            if (col.CompareTag("Player"))
                            {
                                if (!isAttacking)
                                {
                                    isAttacking = true;
                                    anim.Play("skeletonAttack");
                                    cooldownAttack = _edata.attackRate;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dirX != 0) { anim.Play("skeletonWalk"); }
                        rb2d.velocity = new Vector2(dirX * _edata.speed * Time.deltaTime, GetComponent<Rigidbody2D>().velocity.y);
                    }
                }
                #endregion
            }

        }


        /// <summary>
        /// Damage to the Character
        /// </summary>
        /// <param name="col"></param>
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !itsDying && !LevelManager.instance.isPlayerDead)
            {
                col.GetComponent<IDamagable>().TakeDamage(_edata.damage, transform.position);
            }
        }

        #endregion

        #region Take Damage
        public void TakeDamage(float damage, Vector3 attackerPos)
        {
            healthSystem.DealDamage(damage);


            if (healthSystem.GetHealth() > 0)
            {
                SoundManager.instance.Play("skeletonHit");
                anim.SetTrigger("hurt");
                if (!isStunned)
                {
                    StopCoroutine(KnockCo(attackerPos));
                    StartCoroutine(KnockCo(attackerPos));
                }
                Vector3 dir = transform.position - attackerPos;
                dir = transform.position + dir.normalized;
                GameObject instance = Instantiate(particle, dir, Quaternion.identity);
                Destroy(instance, 1f);
            }
            else
            {
                SoundManager.instance.Play("Death");
                StopAllCoroutines();
                StartCoroutine(DieCo());
                GameObject instance = Instantiate(particle, transform.position, Quaternion.identity);
                Destroy(instance, 1f);
            }
        }

        public void TakeDamage(float damage)
        {
            healthSystem.DealDamage(damage);
            GameObject instance = Instantiate(particle, transform.position, Quaternion.identity);

            if (healthSystem.GetHealth() > 0)
            {
                SoundManager.instance.Play("skeletonHit");
                anim.SetTrigger("hurt");
                Destroy(instance, 1f);
            }
            else
            {
                SoundManager.instance.Play("Death");
                StopAllCoroutines();
                StartCoroutine(DieCo());
                Destroy(instance, 1f);
            }
        }


        private IEnumerator KnockCo(Vector3 playerPos)
        {
            if (_edata.stunTime > 0)
            {
                isStunned = true;
            }
            isAttacking = false;
            Vector2 forceDirection = transform.position - playerPos;
            Vector2 force = forceDirection.normalized * thrust;
            rb2d.mass = 1;
            rb2d.AddForce(force, ForceMode2D.Impulse);
            rb2d.mass = 999;
            yield return new WaitForSeconds(_edata.stunTime);
            isStunned = false;
            if (_edata.stunTime > 0)
            {
                _edata.stunTime -= 0.1f;
            }
            rb2d.velocity = new Vector2();
        }

        private IEnumerator DieCo()
        {
            itsDying = true;
            Physics2D.IgnoreLayerCollision(10, 10);
            rb2d.velocity = new Vector2();
            anim.Play("skeletonDie");
            yield return new WaitForSeconds(0.55f);
            rb2d.isKinematic = true;
            GetComponent<BoxCollider2D>().enabled = false;
            enabled = false;

        }

        #endregion

        private void Flip(Vector3 dir)
        {
            if (dir.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _edata.visionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _edata.attackRadius);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
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
    }


}

