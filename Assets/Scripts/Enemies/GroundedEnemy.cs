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
        Vector2 CapsuleColliderCenter;

        protected override void Start()
        {
            base.Start();
            groundDetector = transform.GetChild(3)?.GetComponent<Transform>();
        }

        //TODO (Change the name of the Move Behaviour)

        /// <summary>
        /// Movement Behaviour. Gets updated every frame
        /// Handles the following and patrolling Behaviour of the enemy.
        /// </summary>
        protected override void Move()
        {
            if (!currentState.Equals(State.NONE) && !currentState.Equals(State.FRIENDLY))
            {
                //CHASING
                if (currentState.Equals(State.CHASING))
                {
                    Chasing();
                }
                //PATROLLING
                else if (currentState.Equals(State.PATROLLING))
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

            //Flip
            if (!isAttacking)
            {
                FlipByTargetDirection(dirX);
            }
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
            else
            {
                UpdateMovement();
            }

        }
        /// <summary>
        /// Update Movement 
        /// Check the Direction X of the target and don't update the movement if it's attacking because otherwise, it'll follow
        /// the player in the attacking animation.
        /// </summary>
        protected void UpdateMovement()
        {
            if (dirX != 0 && !isAttacking && CheckGround())
            {
                anim.Play(_enemyData.animation.enemyMovement);
                rb2d.velocity = new Vector2(dirX * _enemyData.speed * Time.deltaTime, GetComponent<Rigidbody2D>().velocity.y);
            }
            else
            {
                //Idle animation only if we have no ground, because otherwise the enemy will keep walking when it's not suppose to do.
                //Not play idle animation when attacking because it will cancel the attack animation
                if (!CheckGround() && !isAttacking)
                {
                    anim.Play(_enemyData.animation.enemyIdle);

                }
                //Not follow when attacking
                rb2d.velocity = Vector2.zero;
            }
        }

        protected void Attack()
        {
            if (!isAttacking)
            {
                rb2d.velocity = Vector2.zero;
                RunCooldownAttackTimer();
                isAttacking = true;
                anim.Play(_enemyData.animation.enemyAttack);
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
            Gizmos.DrawRay(CapsuleColliderCenter, raycastDir);
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
            //Check direction
            if (headingRight)
            {
                raycastDir = Vector3.right;
            }
            else
            {
                raycastDir = Vector3.left;
            }
            //Raycast
            raycastWall = Physics2D.Raycast(CapsuleColliderCenter, raycastDir, 2, 1 << LayerMask.NameToLayer("Ground"));

            if (raycastWall)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Updates Hitbox
        /// </summary>
        protected override void FixedUpdate()
        {
            if (!currentState.Equals(State.DEATH))
            {
                base.FixedUpdate();
                CheckHitBox();
            }
        }

        /// <summary>
        /// Updates States
        /// </summary>
        protected override void Update()
        {
            if (!currentState.Equals(State.DEATH))
            {
                base.Update();
                //Updates center of the box collider to check for walls
                CapsuleColliderCenter = new Vector2(GetComponent<CapsuleCollider2D>().bounds.center.x, GetComponent<CapsuleCollider2D>().bounds.center.y - 0.2f);
                UpdateState();
            }
        }

        /// <summary>
        /// Updates the currentState of the enemy. Check the distance between the target and this enemy. And also set
        /// Custom Behaviours (example: frienldly)
        /// </summary>
        protected void UpdateState()
        {
            if (playerGO != null)
            {
                if (!currentState.Equals(State.FRIENDLY))
                {
                    if (patrollingEnabled)
                    {
                        //Out of vision
                        if (FollowPlayer())
                        {
                            currentState = State.CHASING;
                        }
                        else
                        {
                            currentState = State.PATROLLING;
                        }
                    }
                    else if (FollowPlayer())
                    {
                        sawPlayer = true;
                    }
                    else
                    {
                        currentState = State.NONE;
                        sawPlayer = false;
                    }

                    if (sawPlayer)
                    {
                        currentState = State.CHASING;
                        currentVisionRadius = _enemyData.visionRadiusUpgrade;
                    }
                }
            }
        }

        protected void CheckHitBox()
        {
            if (checkForHitBox)
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

        public void SwishSound()
        {
            SoundManager.instance.Play(_enemyData.swishSound);
        }
    }
}
