using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private bool isFacingRight;
    private Vector3 direction;
    [SerializeField] private float speed = 50f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask damageableLayer;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private Transform hitTransform;
    [SerializeField] private float damage = 60f;

    public void Setup(bool isFacingRight)
    {
        this.isFacingRight = isFacingRight;

        if (!this.isFacingRight)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            direction = Vector3.left;
        }
        else
        {
            direction = Vector3.right;
        }
    }

    void FixedUpdate()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer | damageableLayer | collisionLayer);
        foreach (Collider2D col in hit)
        {
            if (col.GetComponent<IDamageable>() != null)
            {
                col.GetComponent<IDamageable>().TakeDamage(damage, transform.position);
                Destroy(gameObject);
            }
            if (col.IsTouchingLayers(collisionLayer))
            {
                Destroy(gameObject);
            }
        }
        if (hit.Length <= 0)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(hitTransform.position, radius);
        Gizmos.color = Color.white;
    }
}
