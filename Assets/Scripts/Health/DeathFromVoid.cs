using System.Collections;
using UnityEngine;

public class DeathFromVoid : MonoBehaviour
{
    public float waitToDie = 0.3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !GameManager.GetInstance().playerIsDead)
        {
            StartCoroutine(Die(collision));
        }
    }

    private IEnumerator Die(Collider2D collider)
    {
        yield return new WaitForSeconds(waitToDie);

        if (collider.GetComponent<IDamageable>() != null)
        {
            collider.GetComponent<IDamageable>().DieInstantly();
        }
        GameManager.GetInstance().TogglePlayerDeath(true);
    }
}
