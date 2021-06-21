using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingCellCage : MonoBehaviour, IDamageable
{
    private HealthSystem hs;

    private bool isStunned;
    private float stunTime = 0.2f;

    public void DieInstantly()
    {
        //nothing
    }

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        hs.DealDamage(damage);
        if(hs.GetHealth() <= 0)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            GetComponent<Rigidbody2D>().gravityScale = 1;
            StartCoroutine(StunCo());
        }
        else
        {
            StartCoroutine(StunCo());
        }

    }

    private IEnumerator StunCo()
    {
        isStunned = true;
        SoundManager.instance.Play("cellCageHit");
        GetComponent<Animator>().Play("cellCageDamage");
        yield return new WaitForSeconds(stunTime);
        GetComponent<Animator>().Play("cellCageIdle");
        isStunned = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        hs = GetComponent<HealthSystem>();
    }


}
