using SimplePlatformer.Enemy;
using UnityEngine;
using Pathfinding;
namespace SimplePlatformer.Enemy
{
    public class FlyingEnemy : Enemy
    {

        protected Path path;
        protected Seeker seeker;
        protected float timeBtwShoots;
        public float startTimeBtwShoots = 2f;
        public float retreatDistance = 5f;
        public float stoppingDistance = 4f;
        public GameObject projectile;
        float distanceToPlayer;
        public bool noShooting;

        protected override void Start()
        {
            base.Start();
            timeBtwShoots = startTimeBtwShoots;
            seeker = GetComponent<Seeker>();
            distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
            InvokeRepeating("UpdatePath", 0, .5f);
        }

        public void UpdatePath()
        {
            if (!FollowPlayer() && distanceToPlayer > currentVisionRadius && distanceToPlayer > stoppingDistance)
            {
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
            Flip();
        }

        protected override void Update()
        {
            base.Update();


            if (FollowPlayer()) {
                sawPlayer = true;
            }

            if (target != null && !friendly)
            {

                distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
                //Distance

                if(sawPlayer)
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
                    StopFollowing();
                }
                Shoot();
            }


        }

        private void Shoot()
        {
            if (!itsDying && !noShooting)
            {
                if (timeBtwShoots <= 0 && distanceToPlayer < currentVisionRadius)
                {
                    Instantiate(projectile, transform.position, Quaternion.identity);
                    timeBtwShoots = startTimeBtwShoots;
                }
                else
                {
                    timeBtwShoots -= Time.deltaTime;
                }
            }

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);
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

