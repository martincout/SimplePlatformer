using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour, IDamagable
{
    private Animator anim;
    public float health;
    [SerializeField] private GameObject drop;
    [SerializeField] private GameObject particle;


    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void DieInstantly()
    {
        //
    }

    public void TakeDamage(float damage, Vector3 attackerPos)
    {
        DestroyObject(damage);

    }

    public void TakeDamage(float damage)
    {
        DestroyObject(damage);
    }

    public void DestroyObject(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            anim.Play("potDestroying");
            Instantiate(particle, transform);
            if (drop != null)
            {
                Instantiate(drop, new Vector2(transform.position.x, Mathf.Round(transform.position.y + 0.5f)), Quaternion.identity);

            }
            Destroy(gameObject, 1f);
        }
    }

}
