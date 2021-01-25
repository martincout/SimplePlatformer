using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Enemy
{
    public class GroundedEnemy : Enemy
    {
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRange = 0.2f;
        [SerializeField] private float visionRadius = 0.2f;
        [HideInInspector] public bool checkForHitBox = false;
        [SerializeField] private float rayLength = 0.2f;
        private RaycastHit2D raycastGround;
        public Transform groundDetector;

        protected override void Start() {
            base.Start();
            visionRadius = _enemyData.visionRadius;
            groundDetector = transform.GetChild(3)?.GetComponent<Transform>();
        }

        protected override void Move()
        {
            #region Find Player
            //Vision radius
            Collider2D[] circleVision = Physics2D.OverlapCircleAll(transform.position, visionRadius, 1 << LayerMask.NameToLayer("Player"));
            Collider2D[] boxAttackRadius = Physics2D.OverlapBoxAll(transform.position, _enemyData.attackRadius, 0, 1 << LayerMask.NameToLayer("Player"));

            Vector3 targetPos = transform.position;
            bool foundPlayer = false;
            foreach (Collider2D col in circleVision)
            {
                if (col.CompareTag("Player"))
                {
                    targetPos = col.transform.position;
                    foundPlayer = true;
                    break;
                }
                foundPlayer = false;
            }


            if (!foundPlayer)
            {
                anim.Play(_enemyData.animation.enemyIdle);
                visionRadius = _enemyData.visionRadius;
                return;
            }
            else
            {
                visionRadius += 5;
            }

            RaycastHit2D raycastTarget = Physics2D.Raycast(transform.position, targetPos);
            Debug.DrawLine(transform.position, targetPos);
            #endregion
            #region Direction & Distance
            //Calculate the current distance to the target
            float distance = Vector3.Distance(targetPos, transform.position);
            //Direction: Returns a normalized vector (1,-1)
            Vector3 dir = (targetPos - transform.position).normalized;


            if (CheckGround())
            {
                dirX = dir.x;
            }
            else
            {
                if (!isAttacking) anim.Play(_enemyData.animation.enemyIdle);
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
                                    anim.Play(_enemyData.animation.enemyAttack);
                                    cooldownAttack = _enemyData.attackRate;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dirX != 0) { anim.Play(_enemyData.animation.enemyMovement); }
                        rb2d.velocity = new Vector2(dirX * _enemyData.speed * Time.deltaTime, GetComponent<Rigidbody2D>().velocity.y);
                    }
                }
                #endregion
            }

        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.green;
            Gizmos.DrawRay(groundDetector.position, Vector2.down * rayLength);
        }

        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        protected bool CheckGround()
        {
            //float rayLength = Mathf.Abs(deltaMove.x) + skinWidth * 2;

            //Vector2 rayOrigin = (directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) +
            //    Vector2.up * deltaMove.y;
            Vector2 rayOrigin = groundDetector.position;
            raycastGround = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, 1 << LayerMask.NameToLayer("Ground"));

            if (!raycastGround)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            CheckHitBox();
        }

        protected void CheckHitBox()
        {
            if (checkForHitBox)
            {
                if (!itsDying)
                {
                    Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, 1 << LayerMask.NameToLayer("Player"));

                    foreach (Collider2D col in hits)
                    {
                        if (col.GetComponent<IDamageable>() != null && manyHits >= 1)
                        {
                            col.GetComponent<IDamageable>().TakeDamage(_enemyData.damage, transform.position);
                            manyHits -= 1;
                        }
                    }
                }
            }
        }
    }
}
