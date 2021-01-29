using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Enemy
{
    public class GroundedEnemy : Enemy
    {
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRange = 0.2f;
        [HideInInspector] public bool checkForHitBox = false;
        [SerializeField] private float rayLength = 0.2f;
        
        private RaycastHit2D raycastGround;
        private RaycastHit2D raycastWall;
        public Transform groundDetector;
        private Vector3 raycastDir;


        /// <summary>
        /// Position of the BoxCollider in the world space. Used to check wall collisions
        /// </summary>
        Vector2 BoxColliderCenter;

        protected override void Start()
        {
            base.Start();
            groundDetector = transform.GetChild(3)?.GetComponent<Transform>();
        }

        /// <summary>
        /// Guardar temportalmente el Movimiento y Patrulla del enemigo
        /// </summary>
        private void TempFunc()
        {
            //#region Check Ground 
            //if (!CheckGround() || CheckWall())
            //{
            //    CheckHeadingDirection();
            //    if (!isAttacking) anim.Play(_enemyData.animation.enemyIdle);
            //    //Updates the direction of the player. If it is patrolling doesn't apply the direction
            //    dirX = 0;

            //}
            //else
            //{
            //    //Updates the direction of the player. If it is patrolling doesn't apply the direction
            //    dirX = dir.x;
            //}
            //#endregion

            //#region Patrol

            //if (!playerFound)
            //{
            //    currentVisionRadius = _enemyData.visionRadius;
            //    if (patrollingEnabled)
            //    {
            //        Flip();
            //        Patrolling();
            //    }
            //    else
            //    {
            //        PlayAnimation(_enemyData.animation.enemyIdle);
            //    }
            //    return;
            //}
            //else
            //{
            //    currentVisionRadius = _enemyData.visionRadiusUpgrade;
            //}

            //#endregion

            ////Flip the sprite. Working. Don't ask why
            //Flip(dirX);

            ////Raycast to player
            //Debug.DrawLine(transform.position, playerGO.transform.position);

            //if (!isStunned && !isAttacking)
            //{
            //    #region Chase and Attack
            //    Collider2D[] boxAttackRadius = Physics2D.OverlapBoxAll(transform.position, _enemyData.attackRadius, 0, 1 << LayerMask.NameToLayer("Player"));

            //    //Raycast target checking the vision radius
            //    if (distance < currentVisionRadius)
            //    {
            //        //Check if there is a player in the attack radius
            //        if (boxAttackRadius.Length > 0)
            //        {
            //            foreach (Collider2D col in boxAttackRadius)
            //            {
            //                if (col.CompareTag("Player"))
            //                {
            //                    if (!isAttacking)
            //                    {
            //                        isAttacking = true;
            //                        anim.Play(_enemyData.animation.enemyAttack);
            //                        cooldownAttack = _enemyData.attackRate;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (dirX != 0) { anim.Play(_enemyData.animation.enemyMovement); }
            //            rb2d.velocity = new Vector2(dirX * _enemyData.speed * Time.deltaTime, GetComponent<Rigidbody2D>().velocity.y);
            //        }
            //    }
            //    #endregion
            //}
        }

        /// <summary>
        /// Movement Behaviour. Gets updated every frame
        /// Handles the following and patrolling Behaviour of the enemy.
        /// </summary>
        protected override void Move()
        {

            if (!currentState.Equals(State.NONE) && !currentState.Equals(State.FRIENDLY))
            {
                
                //PATROLLING
                if (currentState.Equals(State.CHASING))
                {
                    Chasing();
                }
                //CHASING
                else
                {
                    Patrolling();
                }
            }
        }

        private void Patrolling()
        {
            PlayAnimation(_enemyData.animation.enemyMovement);
            if (currentVisionRadius != _enemyData.visionRadius) currentVisionRadius = _enemyData.visionRadius;

            //If we have no ground, we change the direction and keep patrolling to the oposite direction
            if (!CheckGround() || CheckWall())
            {
                //Change Direction
                if (headingRight)
                {
                    headingRight = false;
                    FlipByHeadingDirection(headingRight);
                    raycastDir = Vector3.left;
                }
                else
                {
                    headingRight = true;
                    FlipByHeadingDirection(headingRight);
                    raycastDir = Vector3.right;
                }
            }
            else
            {
                FlipByVelocity();
            }

            //Change direction
            if (headingRight)
            {
                //Speed divided by two to make the enemy slower.
                rb2d.velocity = new Vector2(1 * _enemyData.speed / 2 * Time.deltaTime, GetComponent<Rigidbody2D>().velocity.y);
            }
            else
            {
                rb2d.velocity = new Vector2(-1 * _enemyData.speed / 2 * Time.deltaTime, GetComponent<Rigidbody2D>().velocity.y);
            }
        }

        private void Chasing()
        {
            Collider2D[] boxAttackRadius = Physics2D.OverlapBoxAll(transform.position, _enemyData.attackRadius, 0, 1 << LayerMask.NameToLayer("Player"));
            Vector3 dir = (playerGO.transform.position - transform.position).normalized;
            dirX = dir.x;
            FlipByTargetDirection(dirX);

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
                if (dirX != 0 && !isAttacking) { anim.Play(_enemyData.animation.enemyMovement); }
                rb2d.velocity = new Vector2(dirX * _enemyData.speed * Time.deltaTime, GetComponent<Rigidbody2D>().velocity.y);
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
            
            if(playerGO != null) UpdateState();
        }

        /// <summary>
        /// Updates the currentState of the enemy. Check the distance between the target and this enemy. And also set
        /// Custom Behaviours (example: frienldly)
        /// </summary>
        protected void UpdateState()
        {
            if (!currentState.Equals(State.FRIENDLY))
            {
                if (patrollingEnabled)
                {
                    distanceToTarget = Vector2.Distance(transform.position, playerGO.transform.position);
                    //Out of vision
                    if (distanceToTarget > currentVisionRadius)
                    {
                        currentState = State.PATROLLING;
                    }
                    else
                    {
                        currentState = State.CHASING;
                    }
                }

            }
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
