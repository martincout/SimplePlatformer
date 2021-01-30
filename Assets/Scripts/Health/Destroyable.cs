using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour, IDamageable
{
    private Animator anim;
    public float health;
    public GameObject drop;
    public float dropChance = 0.5f;
    [SerializeField] private GameObject particle;


    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void DieInstantly()
    {
        //NOTHING
    }

    public void TakeDamage(float damage, Vector3 attackerPos)
    {
        DestroyObject(damage);
    }


    public void DestroyObject(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            anim.Play("potDestroying");
            SoundManager.instance.Play("JarDestroy");
            Instantiate(particle, transform);
            if (drop != null)
            {
                if (Random.Range(0, 1f) <= dropChance)
                {
                    Instantiate(drop, new Vector2(transform.position.x, Mathf.Round(transform.position.y + 0.5f)), Quaternion.identity);
                }

            }
            Destroy(gameObject, 1f);
        }
    }

}
