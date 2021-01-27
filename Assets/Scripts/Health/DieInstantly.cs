using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieInstantly : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IDamageable>() != null)
        {
            collision.GetComponent<IDamageable>().DieInstantly();
        }
    }
}
