﻿using SimplePlatformer.Enemy;
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
        RaycastHit2D hitPlayer;


        protected override void Start()
        {
            base.Start();
            timeBtwShoots = startTimeBtwShoots;
            seeker = GetComponent<Seeker>();
            distanceToPlayer = Vector2.Distance(transform.position, target.position);
            hitPlayer = Physics2D.Raycast(transform.position, target.position);
            InvokeRepeating("UpdatePath", 0, .5f);
        }

        public void UpdatePath()
        {
            
            if (notFollow || distanceToPlayer > _enemyData.visionRadius ||  friendly || hitPlayer != false && hitPlayer.transform.CompareTag("Player") )
            {
                return;
            }
            if (seeker.IsDone() && target != null)
            {
                seeker.StartPath(rb2d.position, target.position, OnPathComplete);
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
            if (path == null)
                return;
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
            if(target != null)
            {
                hitPlayer = Physics2D.Raycast(transform.position, target.position);

                if (!friendly && !hitPlayer.transform.CompareTag("Player"))
                {
                    //Distance
                    distanceToPlayer = Vector2.Distance(transform.position, target.position);

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
                    if (timeBtwShoots <= 0 && distanceToPlayer < _enemyData.visionRadius)
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
            

           


        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        }

        private void StopFollowing()
        {

            notFollow = true;
            if (!isStunned)
            {

                rb2d.velocity = Vector2.zero;
            }


        }
    }

}

