using TMPro;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Item item;
    public HealthSystem hs;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (hs != null)
            {
                item.DoSomething(hs);
            }
            else
            {
                item.DoSomething();
            }
            GameObject instance = Instantiate(item.particleGameobject, transform.position, Quaternion.identity);
            SoundManager.instance.Play("Coin");
            if (item.name.Equals("Coin"))
            {
                GameStatus.GetInstance().AddScore((int)item.value);
            }
            Destroy(gameObject);
            Destroy(instance, 1f);
        }
    }
}
