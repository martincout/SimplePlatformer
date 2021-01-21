using System.Collections;
using UnityEngine;

/// <summary>
/// Handles States
/// </summary>
namespace SimplePlatformer.Player
{
    public class PlayerBase : MonoBehaviour, IDamagable
    {
        //Animation Const
        public static readonly string PLAYER_IDLE = "playerIdle";
        public static readonly string PLAYER_WALK = "playerWalk";
        public static readonly string PLAYER_JUMP = "playerJump";
        public static readonly string PLAYER_FALLING = "playerFalling";
        public static readonly string PLAYER_ATTACKING = "playerAttack";
        public static readonly string PLAYER_HURT = "playerHurt";
        public static readonly string PLAYER_INVINCIBLE = "playerInvincible";
        public static readonly string PLAYER_AIRATTACK = "playerAirAttack";

        //States
        protected static bool movePrevent;
        protected static bool isFacingRight;
        protected static bool isJumping;
        protected static bool isAttacking;
        protected static bool isStunned;
        protected static bool itsDying;
        protected static bool invincible;
        protected static bool airAttacked;

        //Aux
        protected float invincibleTime = 1f;
        protected float cooldownInvincible = 0f;
        //Values
        protected Vector2 axisDir;

        //Take damage
        private float hurtTime = 0.3f;
        internal HealthSystem healthSystem;
        protected CharacterParticles characterParticles;
        protected Animator anim;
        protected Rigidbody2D rb2d;
        protected Renderer render;

        private void Awake()
        {

            healthSystem = GetComponent<HealthSystem>();
            characterParticles = GetComponent<CharacterParticles>();
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            render = GetComponent<Renderer>();
        }

        private void Start()
        {
            movePrevent = false;
            isFacingRight = true;
            isJumping = false;
            isAttacking = false;
            isStunned = false;
            itsDying = false;
            invincible = false;
            airAttacked = false;
        }

        private void Update()
        {
            if (cooldownInvincible > 0)
            {
                cooldownInvincible -= Time.deltaTime;

            }
            else
            {
                invincible = false;
                //Alpha to 100% NOT IN INVINCIBLE STATE
                SetAlpha(1f);
            }
        }

        internal IEnumerator EnableMovementAfter(float seconds)
        {
            movePrevent = true;
            yield return new WaitForSeconds(seconds);
            movePrevent = false;
        }

        internal void SetInvincible(float cooldown)
        {
            invincible = true;
            cooldownInvincible = cooldown;
        } 

        internal void SetAlpha(float alpha)
        {
            Color c = render.material.color;
            c.a = alpha;
            render.material.color = c;
        }

        public void TakeDamage(float damage, Vector3 attackerPos)
        {
            if (!invincible)
            {
                //Decrease Health
                healthSystem.DealDamage(damage);

                characterParticles.PlayParticle(Type.HURT);
                //Check
                if (healthSystem.GetHealth() > 0)
                {
                    SoundManager.instance.Play("Damage");
                    anim.Play(PLAYER_HURT);
                    StartCoroutine(KnockCo(attackerPos));
                    if (!isStunned)
                    {
                        StartCoroutine(StunCo());
                        SetInvincible(invincibleTime);
                        SetAlpha(0.7f);
                    }
                }
                else
                {
                    SoundManager.instance.Play("Death");
                    StopAllCoroutines();
                    StartCoroutine(KnockCo(attackerPos));
                    StartCoroutine(DieCo());
                }
            }

        }
        public  void TakeDamage(float damage)
        {

            if (!invincible)
            {
                //Decrease Health
                healthSystem.DealDamage(damage);

                characterParticles.PlayParticle(Type.HURT);
                //Check
                if (healthSystem.GetHealth() > 0)
                {
                    SoundManager.instance.Play("Damage");
                    anim.Play(PLAYER_HURT);
                    if (!isStunned)
                    {
                        StartCoroutine(StunCo());
                        SetInvincible(invincibleTime);
                        Color c = render.material.color;
                        c.a = 0.7f;
                        render.material.color = c;
                    }
                }
                else
                {
                    SoundManager.instance.Play("Death");
                    StopAllCoroutines();
                    StartCoroutine(DieCo());
                }
            }

        }

        public void DieInstantly()
        {
            SoundManager.instance.Play("Death");
            characterParticles.PlayParticle(Type.HURT);
            healthSystem.SetHealth(0);
            StopAllCoroutines();
            StartCoroutine(DieCo());
        }

        private IEnumerator DieCo()
        {
            itsDying = true;
            GetComponent<PlayerMovement>().enabled = false;
            rb2d.velocity = Vector2.zero;
            anim.Play("playerDie");
            yield return new WaitForSeconds(0.55f);
            Destroy(gameObject);
            EventSystem.DeathHandler?.Invoke();
        }

        private IEnumerator StunCo()
        {
            isStunned = true;
            yield return new WaitForSeconds(hurtTime);
            isStunned = false;
        }

        private IEnumerator KnockCo(Vector3 attackerPos)
        {
            Vector2 forceDirection = transform.position - attackerPos;
            Vector2 force = forceDirection.normalized * 10; // Thrust * the vector
            rb2d.AddForce(force, ForceMode2D.Impulse);
            yield return new WaitForSeconds(hurtTime);
            rb2d.velocity = new Vector2();
        }

        
    }
}

