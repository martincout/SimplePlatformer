using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IDamageable
{
    public float speed;
    public float damage;
    public GameObject player;
    private Vector2 direction;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        direction = player.transform.position - transform.position;
    }

    private void FixedUpdate()
    {
        rb.velocity = direction.normalized * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Collision"))
        {
            GetComponent<Animator>().Play("projectileDestroy");
            if (collision.GetComponent<IDamageable>() != null)
            {
                collision.GetComponent<IDamageable>().TakeDamage(damage, transform.position);
            }
            SoundManager.instance.Play("Crack");
            Destroy(gameObject,0.4f);
            GetComponent<CircleCollider2D>().enabled = false;
        }

        
    }

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Animator>().Play("projectileDestroy");
        Destroy(gameObject, 0.4f);
        SoundManager.instance.Play("Crack");
    }


    public void DieInstantly()
    {
        //Nothing
    }
}   
