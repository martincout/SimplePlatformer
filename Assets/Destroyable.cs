using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour, IDamagable
{
    private Animator anim;
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
        anim.Play("potDestroying");
        Instantiate(particle, transform);
        Destroy(gameObject, 1f);
    }

    public void TakeDamage(float damage)
    {
        anim.Play("potDestroying");
        Instantiate(particle, transform);
        Destroy(gameObject, 1f);
    }

}
