using System.Collections;
using UnityEngine;

public class DeathFromVoid : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        EventSystem.SetDeathFromVoid();
        StartCoroutine( Die(collision));
    }

    private IEnumerator Die(Collider2D collider)
    {
       yield return new WaitForSeconds(0.1f);
        if (collider.CompareTag("Player"))
        {
            if (collider.GetComponent<IDamageable>() != null)
            {
                collider.GetComponent<IDamageable>().DieInstantly();
            }
        }
    }
}
