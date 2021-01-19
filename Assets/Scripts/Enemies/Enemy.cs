using UnityEngine;
using System.Collections;

namespace SimplePlatformer.Enemy
{
    public class Enemy : MonoBehaviour, IDamagable
    {
        [Header("Stats")]
        [SerializeField] private float maxHealth;
        [SerializeField] private float speed = 5f;
        [SerializeField] private float damage = 60f;
        private HealthSystem healthSystem;


        [Header("Attack")]
        [SerializeField] private float thrust = 50f;
        [SerializeField] private float visionRadius;
        [SerializeField] private float attackRadius;
        [SerializeField] private float attackRate = 1.2f;
        private float cooldownAttack = 0f;
        [SerializeField] private float attackRange = 0.2f;
        [SerializeField] private Transform attackPoint;
        [Header("Stun")]
        [SerializeField] private float stunTime = 0.3f;
        [SerializeField] private float noStunTime = 10f;
        private float startStunTime;
        private bool isStunned = false;
        private bool isAttacking = false;
        private bool itsDying = false;
        private float dirX;

        [SerializeField] private float rayLength = 0.2f;
        private Transform groundDetector;

        public GameObject particle;
        private Animator anim;
        private Rigidbody2D rb2d;

        public void Start()
        {
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            healthSystem = GetComponent<HealthSystem>();
            startStunTime = stunTime;
            groundDetector = transform.GetChild(2).GetComponent<Transform>();
        }

        private void FixedUpdate()
        {
            if (!itsDying && !LevelManager.instance.isPlayerDead)
            {
                Move();
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
        private void Update()
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
            if (stunTime < 0)
            {
                stunTime -= Time.deltaTime;
                if (stunTime < -noStunTime)
                {
                    stunTime = startStunTime;
                }
            }
        }

        #region Attack

        protected virtual void Move()
        {
            #region Find Player
            //Vision radius
            Collider2D[] circleVision = Physics2D.OverlapCircleAll(transform.position, visionRadius);

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
                PlayAnimation("enemyIdle");
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
                if (!isAttacking) PlayAnimation("enemyIdle");
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
                //Chase and Attack
                if (raycastTarget.distance < distance)
                {
                    if (distance < attackRadius)
                    {
                        if (!isAttacking)
                        {
                            isAttacking = true;
                            anim.Play("enemyAttack");
                            cooldownAttack = attackRate;
                        }
                    }
                    else if(!isAttacking)
                    {
                        if (dirX != 0) { anim.Play("enemyWalk"); }
                        rb2d.velocity = new Vector2(dirX * speed * (50 * Time.deltaTime), GetComponent<Rigidbody2D>().velocity.y);
                    }

                }
                #endregion
            }

        }

        /// <summary>
        /// Animation Event: Do the Damage
        /// </summary>
        private void Hit()
        {
            if (!itsDying)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, 1 << LayerMask.NameToLayer("Player"));

                foreach (Collider2D col in hits)
                {
                    if (col.GetComponent<IDamagable>() != null)
                    {
                        col.GetComponent<IDamagable>().TakeDamage(damage, transform.position);
                    }
                }
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
                col.GetComponent<IDamagable>().TakeDamage(damage, transform.position);
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
            if (stunTime > 0)
            {
                isStunned = true;
            }
            isAttacking = false;
            Vector2 forceDirection = transform.position - playerPos;
            Vector2 force = forceDirection.normalized * thrust;
            rb2d.mass = 1;
            rb2d.AddForce(force, ForceMode2D.Impulse);
            rb2d.mass = 999;
            yield return new WaitForSeconds(stunTime);
            isStunned = false;
            if (stunTime > 0)
            {
                stunTime -= 0.1f;
            }
            rb2d.velocity = new Vector2();
        }

        private IEnumerator DieCo()
        {
            itsDying = true;
            Physics2D.IgnoreLayerCollision(10, 10);
            rb2d.velocity = new Vector2();
            anim.Play("enemyDie");
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
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, visionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
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
