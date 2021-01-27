using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IDamageable
{
    public float speed;
    public float damage;
    public GameObject player;
    private Vector2 direction;

    private void OnEnable()
    {
        EventSystem.RespawnHandler += UpdatePlayer;
    }

    void UpdatePlayer(GameObject _player)
    {
        player = _player;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
         direction = player.transform.position - transform.position;
    }

    private void Update()
    {
        
        GetComponent<Rigidbody2D>().velocity = direction.normalized * speed * Time.deltaTime;

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
            Destroy(gameObject,0.4f);
            GetComponent<CircleCollider2D>().enabled = false;
        }

        
    }

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        GetComponent<Animator>().Play("projectileDestroy");
        Destroy(gameObject, 0.4f);
    }

    public void DieInstantly()
    {
        //Nothing
    }
}   
