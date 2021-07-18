using SimplePlatformer.Enemy;
using UnityEngine;
using Pathfinding;
namespace SimplePlatformer.Enemy
{
    public class FlyingEnemy : Enemy
    {

        protected Path path;
        protected Seeker seeker;
        protected float timeBtwShoots = 0f;
        public float startTimeBtwShoots = 2f;
        public float retreatDistance = 5f;
        public float stoppingDistance = 4f;
        public GameObject projectile;
        float distanceToPlayer;
        public bool noShooting;

        protected override void Start()
        {
            base.Start();
            seeker = GetComponent<Seeker>();
            distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
            InvokeRepeating("UpdatePath", 0, .5f);
        }

        public void UpdatePath()
        {
            if (!FollowPlayer() && distanceToPlayer > currentVisionRadius && distanceToPlayer > stoppingDistance)
            {
                sawPlayer = false;
                path = null;
                return;
            }
            if (seeker.IsDone() && target != null)
            {
                seeker.StartPath(rb2d.position, target.transform.position, OnPathComplete);
            }


        }

        void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }

        protected override void Move()
        {
            //Nothing
        }

        protected override void FixedUpdate()
        {

            //No path
            if (path == null) return;
            //Check reached end of path
            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = false;
                return;
            }
            else
            {
                reachedEndOfPath = true;
            }
            //Direction and Movement
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb2d.position).normalized;
            Vector2 force = direction * _enemyData.speed * Time.deltaTime;
            rb2d.AddForce(force);
            //Distance
            float distance = Vector2.Distance(rb2d.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
            FlipByVelocity();

        }

        protected override void Update()
        {
            base.Update();
            if (!currentState.Equals(State.DEATH))
            {
                if (FollowPlayer())
                {
                    sawPlayer = true;
                }

                if (timeBtwShoots > 0)
                {
                    timeBtwShoots -= Time.deltaTime;
                }

                Attack();
            }


        }

        private void Attack()
        {
            if (target != null && !friendly)
            {
                //Distance
                distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
                //direction
                Vector3 dir = (playerGO.transform.position - transform.position).normalized;

                //Movement Logic
                if (sawPlayer)
                {
                    currentVisionRadius = _enemyData.visionRadiusUpgrade;
                }
                else
                {
                    StopFollowing();
                }

                if (distanceToPlayer > stoppingDistance)
                {
                    notFollow = false;
                }
                else if (distanceToPlayer < stoppingDistance && distanceToPlayer > retreatDistance)
                {
                    StopFollowing();
                }
                else if (distanceToPlayer < retreatDistance)
                {
                    path = null;
                    notFollow = true;
                    //Retreat speed will be * 5 to add force gradually and not instantly. It works faster when the speed it's greater
                    float retreatSpeed = _enemyData.speed * 5;
                    rb2d.AddForce(new Vector2(-dir.x * retreatSpeed * Time.deltaTime, -dir.y * retreatSpeed * Time.deltaTime), ForceMode2D.Force);
                    //rb2d.velocity = new Vector2(-dir.x * _enemyData.speed * Time.deltaTime, -dir.y * _enemyData.speed * Time.deltaTime);
                }

                //Attack Logic
                Shoot();

            }

        }

        protected void RunCooldownBtwShotsTimer()
        {
            timeBtwShoots = startTimeBtwShoots;
        }

        private void Shoot()
        {
            if (!noShooting && sawPlayer && !isStunned)
            {
                if (!isAttacking && timeBtwShoots <= 0 && cooldownAttack <= 0)
                {
                    PlayAnimation(_enemyData.animation.enemyAttack);
                    RunCooldownAttackTimer();
                    isAttacking = true;
                }

                if (isAttacking && cooldownAttack <= 0)
                {
                    PlayAnimation(_enemyData.animation.enemyMovement);
                    GameObject instance = Instantiate(projectile, transform.position, Quaternion.identity);
                    instance.GetComponent<Projectile>().speed = _enemyData.projectileSpeed;
                    instance.GetComponent<Projectile>().damage = _enemyData.projectileDamage;
                    RunCooldownBtwShotsTimer();
                }

            }

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, retreatDistance);
        }

        private void StopFollowing()
        {
            path = null;
            notFollow = true;
            if (!isStunned)
            {
                rb2d.velocity = Vector2.zero;
            }
        }
    }

}

