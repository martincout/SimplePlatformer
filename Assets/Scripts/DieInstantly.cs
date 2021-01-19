using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieInstantly : DoDamage
{
    protected  override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IDamagable>() != null)
        {
            collision.GetComponent<IDamagable>().DieInstantly();
        }
    }
}
