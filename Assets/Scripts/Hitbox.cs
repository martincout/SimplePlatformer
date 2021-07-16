using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public string targetTag;
    public float damage;
    public GameObject boss;

    private void Start()
    {
        boss = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            if (collision.GetComponent<IDamageable>() != null)
            {
                collision.GetComponent<IDamageable>().TakeDamage(damage,boss.transform.position);
            }
        }
    }
}
