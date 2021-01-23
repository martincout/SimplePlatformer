using SimplePlatformer.Enemy;
using UnityEngine;
using Pathfinding;
namespace SimplePlatformer.Enemy
{
    public class FlyingEnemy : Enemy
    {
        [SerializeField] protected Transform target;
        protected Path path;
        protected Seeker seeker;
        protected override void Start()
        {
            base.Start();
            seeker = GetComponent<Seeker>();
            InvokeRepeating("UpdatePath", 0, .5f);
        }

        public void UpdatePath()
        {
            if (seeker.IsDone())
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

        private void Flip()
        {
            if (rb2d.velocity.x >= 0.01f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (rb2d.velocity.x <= -0.01f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);

            }
        }
        protected override void Update()
        {
            //nothing
        }
    }

}

