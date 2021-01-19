using SimplePlatformer.Enemy;
using UnityEngine;
using Pathfinding;

public class FlyingEnemy : Enemy
{
    private AIPath aiPath;

    private void Awake()
    {
        aiPath = FindObjectOfType<AIPath>().GetComponent<AIPath>();
    }

    protected override void Move()
    {
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f,1f);
        }else if(aiPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f,1f);

        }
    }
}
