using UnityEngine;
using SimplePlatformer.Enemy;
public class RespawnEntityData
{
    public Vector3 position;
    public Enemy enemy;
    public Destroyable destroyable;
    public GameObject dropItem;
    public float dropChance;
    public bool patrollingEnabled;

    public RespawnEntityData(Enemy _enemy, Vector3 _position)
    {
        enemy = _enemy;
        dropItem = enemy.dropItem;
        dropChance = enemy.dropChance;
        position = _position;
        patrollingEnabled = enemy.patrollingEnabled;
    }

    public RespawnEntityData(Destroyable _destroyable, Vector3 _position)
    {
        enemy = null;
        patrollingEnabled = false;
        destroyable = _destroyable;
        dropItem = destroyable.drop;
        dropChance = destroyable.dropChance;
        position = _position;
    }


}
