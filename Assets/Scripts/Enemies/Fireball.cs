using SimplePlatformer.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour, IDamageable
{
    public float speed;
    public float damage;
    public GameObject player;
    private Vector3 direction;
    public GameObject fireballParticle;
    private float destroyAfter = 0.2f;

    private void Start()
    {
        float destroyParticleAfter = 2f;
        GameObject particle = Instantiate(fireballParticle, transform.position, Quaternion.identity);
        Destroy(particle, destroyParticleAfter);
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        SoundManager.instance.Play("FireballBorn");
    }

    private void FixedUpdate()
    {
        //GetComponent<Rigidbody2D>().velocity = direction.normalized * speed * Time.deltaTime;
        //GetComponent<Rigidbody2D>().AddForce(direction.normalized * speed * Time.deltaTime);
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Collision"))
        {
            SoundManager.instance.Play("FireballExplotion");
            GetComponent<Animator>().Play("fireballDestroy");
            if (collision.GetComponent<IDamageable>() != null)
            {
                collision.GetComponent<IDamageable>().TakeDamage(damage, transform.position);
            }

            Destroy(gameObject, destroyAfter);
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        SoundManager.instance.Play("FireballExplotion");
        GetComponent<Animator>().Play("fireballDestroy");
        this.GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, destroyAfter);
    }

    public void DieInstantly()
    {
        //Nothing
    }
}
