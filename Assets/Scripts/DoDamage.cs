using UnityEngine;

public class DoDamage : MonoBehaviour
{
    public float damage;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<IDamagable>() != null)
        {
            collision.GetComponent<IDamagable>().TakeDamage(damage);
        }
    }
}
