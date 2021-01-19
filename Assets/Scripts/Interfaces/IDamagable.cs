
using UnityEngine;


public interface IDamagable
{
    /// <summary>
    /// The entity takes Damage
    /// </summary>
    /// <param name="damage"></param> The amount of damage dealt
    /// <param name="attackerPos"></param> The position to add knockback in the opossite direction
    void TakeDamage(float damage, Vector3 attackerPos);
    /// <summary>
    /// The entity takes Damage
    /// </summary>
    /// <param name="damage"></param> The amount of damage dealt
    void TakeDamage(float damage);

    void DieInstantly();

}
