using UnityEngine;

public class DoDamage : MonoBehaviour
{
    public float damage;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<IDamageable>() != null)
        {
            collision.GetComponent<IDamageable>().TakeDamage(damage, Vector3.zero);
        }
    }
}
