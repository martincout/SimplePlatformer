using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimplePlatformer.Player
{
    public class Dummy : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private float stuntDuration;
        private HealthSystem hs;
        private bool death, stunned;
        public float knockbackThrust;
        private GameObject player;
        private Animator anim;
        [SerializeField] GameObject hitParticle;
        private Rigidbody2D rb2d;

        private void Start()
        {
            death = false;
            stunned = false;
            player = GameObject.Find("Player");
            anim = GetComponent<Animator>();
            hs = GetComponent<HealthSystem>();
            rb2d = GetComponent<Rigidbody2D>();
        }

        public void TakeDamage(float damage, Vector3 attackerPos)
        {
            hs.DealDamage(damage);
            SoundManager.instance.Play("Damage");
            PlayHurtParticle();
            
            if (!stunned)
            {
                if (transform.TransformDirection(transform.position - attackerPos).x >= 0)
                {
                    anim.Play("Dummy_Damage_Right");
                }
                else
                {
                    anim.Play("Dummy_Damage_Left");
                }
            }
            Knockback(attackerPos);
            if (hs.GetHealth() <= 0)
            {
                Die();
            }

        }


        private void PlayHurtParticle()
        {
            if (hitParticle != null)
            {
                GameObject instance = Instantiate(hitParticle, transform.position, Quaternion.identity);
                Destroy(instance, 1f);
            }
        }

        private void Knockback(Vector3 playerPos)
        {
            
            if (!stunned)
            {
                StartCoroutine(StunCo(stuntDuration));
            }
            Vector2 forceDirection = transform.TransformDirection(transform.position - playerPos);
            if (forceDirection.x > 0)
            {
                forceDirection = new Vector2(1, forceDirection.y);
            }
            else
            {
                forceDirection = new Vector2(-1, forceDirection.y);

            }
            rb2d.velocity = new Vector2();
            Vector2 force = forceDirection * knockbackThrust;
            rb2d.AddForce(force, ForceMode2D.Impulse);
            rb2d.velocity = new Vector2();
        }


        private IEnumerator StunCo(float duration)
        {
            
            stunned = true;
            yield return new WaitForSeconds(duration);
            stunned = false;
            anim.Play("DummyIdle");
        }

        private IEnumerator DieCo()
        {
            rb2d.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.55f);
            enabled = false;
            rb2d.velocity = Vector2.zero;

        }

        protected void Die()
        {
            SoundManager.instance.Play("Death");
            StopAllCoroutines();
            StartCoroutine(DieCo());
        }

        public void DieInstantly()
        {

        }
    }
}
