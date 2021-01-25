using UnityEngine;
public interface IDamageable
{
    /// <summary>
    /// Decreases the health of the character and adds a knockback.
    /// </summary>
    /// <param name="damage"></param> Amount of health to be decreased
    /// <param name="attackerPosition"></param> Can be Vector3.zero if has not knockback
    void TakeDamage(float damage, Vector3 attackerPosition);

    void DieInstantly();

}
