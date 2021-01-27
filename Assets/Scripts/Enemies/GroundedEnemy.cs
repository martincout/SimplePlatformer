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
        public bool headingRight;
        private RaycastHit2D raycastGround;
        private RaycastHit2D raycastWall;
        public Transform groundDetector;
        public bool patrolDisabled;
        bool foundPlayer;
        private Vector3 raycastDir;

        /// <summary>
        /// Position of the BoxCollider in the world space. Used to check wall collisions
        /// </summary>
        Vector2 BoxColliderCenter;

        protected override void Start()
        {
            base.Start();
            visionRadius = _enemyData.visionRadius;
            groundDetector = transform.GetChild(3)?.GetComponent<Transform>();
            CheckHeadingDirection();
        }

        /// <summary>
        /// Movement Behaviour. Gets updated every frame
        /// Handles the following and patrolling Behaviour of the enemy.
        /// </summary>
        protected override void Move()
        {
            #region Direction & Distance
            //Calculate the current distance to the target
            float distance = Vector3.Distance(playerGO.transform.position, transform.position);
            //Direction: Returns a normalized vector (1,-1)
            Vector3 dir = (playerGO.transform.position - transform.position).normalized;

            #endregion

            #region Find Player
            //Vision radius
            Collider2D[] circleVision = Physics2D.OverlapCircleAll(transform.position, visionRadius, 1 << LayerMask.NameToLayer("Player"));
            Collider2D[] boxAttackRadius = Physics2D.OverlapBoxAll(transform.position, _enemyData.attackRadius, 0, 1 << LayerMask.NameToLayer("Player"));

            Vector3 targetPos = playerGO.transform.position;
            foundPlayer = false;

            foreach (Collider2D col in circleVision)
            {
                if (col.CompareTag("Player"))
                {
                    foundPlayer = true;
                    break;
                }
                foundPlayer = false;
            }

            #endregion

            #region Check Ground 
            if (!CheckGround() || CheckWall())
            {
                CheckHeadingDirection();
                if (!isAttacking) anim.Play(_enemyData.animation.enemyIdle);
                //Updates the direction of the player. If it is patrolling doesn't apply the direction
                dirX = 0;
                
            }
            else
            {
                //Updates the direction of the player. If it is patrolling doesn't apply the direction
                dirX = dir.x;
            }
            #endregion

            #region Patrol

            if (!foundPlayer)
            {
                Flip();
                Patrolling();
                return;
            }
            else
            {
                visionRadius += 5;
            }

            #endregion

            //Flip the sprite. Working. Don't ask why
            Flip(dirX);

            //Raycast to player
            Debug.DrawLine(transform.position, targetPos);

            if (!isStunned && !isAttacking)
            {
                #region Chase and Attack

                //Raycast target checking the vision radius
                if (distance < visionRadius)
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

        private void CheckHeadingDirection()
        {
            if (!foundPlayer)
            {
                if (headingRight)
                {
                    headingRight = false;
                    Flip();
                    raycastDir = Vector3.left;
                }
                else
                {
                    headingRight = true;
                    Flip();
                    raycastDir = Vector3.right;
                }
            }
        }

        private void Patrolling()
        {
            if (!patrolDisabled)
            {
                //400f
                float patrollingVel = 0f;
                PlayAnimation(_enemyData.animation.enemyMovement);
                if (visionRadius != _enemyData.visionRadius) visionRadius = _enemyData.visionRadius;
                //Change direction
                if (headingRight)
                {
                    rb2d.velocity = new Vector2(1 * _enemyData.speed * Time.deltaTime - patrollingVel, GetComponent<Rigidbody2D>().velocity.y);
                }
                else
                {
                    rb2d.velocity = new Vector2(-1 * _enemyData.speed * Time.deltaTime + patrollingVel, GetComponent<Rigidbody2D>().velocity.y);
                }
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
            Gizmos.DrawRay(BoxColliderCenter, raycastDir);
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

        private bool CheckWall()
        {
            raycastWall = Physics2D.Raycast(BoxColliderCenter, raycastDir, 2, 1 << LayerMask.NameToLayer("Ground"));
            if (raycastWall)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            CheckHitBox();
        }

        protected override void Update()
        {
            base.Update();
            //Updates center of the box collider
            BoxColliderCenter = new Vector2(GetComponent<BoxCollider2D>().bounds.center.x, GetComponent<BoxCollider2D>().bounds.center.y - 0.2f);
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
