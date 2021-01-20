using SimplePlatformer.Enemy;
using UnityEngine;
using Pathfinding;

public class FlyingEnemy : Enemy
{

    public override void Start()
    {
        base.Start();
        InvokeRepeating("UpdatePath", 0, .5f);
    }

    private void UpdatePath()
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
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = false;
            return;
        }
        else
        {
            reachedEndOfPath = true;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb2d.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb2d.AddForce(force);
        float distance = Vector2.Distance(rb2d.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
    protected override void Update()
    {
        //nothing
    }
}
