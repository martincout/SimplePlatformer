using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles States
/// </summary>
namespace SimplePlatformer.Player
{
    public class PlayerBase : MonoBehaviour, IDamageable, IItem
    {
        [Header("Sub Behaviours")]
        public PlayerMovement playerMovementBehaviour;

        //Animation Const
        public static readonly string PLAYER_IDLE = "playerIdle";
        public static readonly string PLAYER_WALK = "playerWalk";
        public static readonly string PLAYER_JUMP = "playerJump";
        public static readonly string PLAYER_FALLING = "playerFalling";
        public static readonly string PLAYER_ATTACKING = "playerAttack";
        public static readonly string PLAYER_HURT = "playerHurt";
        public static readonly string PLAYER_INVINCIBLE = "playerInvincible";
        public static readonly string PLAYER_AIRATTACK = "playerAirAttack";
        public static readonly string PLAYER_COMBO = "playerAttack2";

        //States
        protected static bool movePrevent;
        protected static bool isFacingRight;
        protected static bool isJumping;
        protected static bool isAttacking;
        protected static bool isStunned;
        [HideInInspector] public static bool itsDying;
        protected static bool invincible;
        protected static bool airAttacked;
        protected static bool cannotAttack;

        //Aux
        protected float invincibleTime = 1f;
        protected float cooldownInvincible = 0f;

        protected static Vector2 rawInputMovement;

        //Take damage
        private float stunTime = 0.3f;
        internal HealthSystem healthSystem;
        protected CharacterParticles characterParticles;
        protected Animator anim;
        protected Rigidbody2D rb2d;
        protected Renderer render;


        public PlayerInput playerInput;

        private readonly float thrust = 30f;

        public bool GetPlayerItsDying()
        {
            return itsDying;
        }

        private void Awake()
        {
            playerMovementBehaviour = GetComponent<PlayerMovement>();
            healthSystem = GetComponent<HealthSystem>();
            characterParticles = GetComponent<CharacterParticles>();
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            render = transform.GetChild(0).GetComponent<Renderer>();
        }

        private void Start()
        {
            cannotAttack = false;
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
            UpdatePlayerMovement();
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


        void UpdatePlayerMovement()
        {

            playerMovementBehaviour.UpdateMovementData(rawInputMovement);
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
            if (!invincible && !itsDying)
            {

                //Decrease Health
                healthSystem.DealDamage(damage);
                characterParticles.PlayParticle(Type.HURT);
                SetInvincible(invincibleTime);
                SetAlpha(0.7f);
                //Check
                if (healthSystem.GetHealth() > 0)
                {
                    SoundManager.instance.Play("Damage");
                    anim.Play(PLAYER_HURT);
                    Knockback(attackerPos, thrust);
                }
                else
                {
                    SoundManager.instance.Play("Death");
                    StopAllCoroutines();
                    Knockback(attackerPos, thrust);
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
            GetComponent<PlayerCombat>().enabled = false;
            rb2d.velocity = Vector2.zero;
            anim.Play("playerDie");
            yield return new WaitForSeconds(0.55f);
            Destroy(gameObject);
            EventSystem.DeathHandler?.Invoke();
        }

        protected void Knockback(Vector3 attackerPos, float thrust)
        {
            if (!isStunned)
            {
                StartCoroutine(StunCo());
                StartCoroutine(KnockCo(attackerPos, thrust));
            }
        }

        private IEnumerator StunCo()
        {
            isStunned = true;
            yield return new WaitForSeconds(stunTime);
            isStunned = false;
        }

        private IEnumerator KnockCo(Vector3 attackerPos, float _thrust)
        {
            float force = _thrust;
            Vector2 forceDirection = transform.TransformDirection(transform.position - attackerPos);
            if (forceDirection.x > 0)
            {
                forceDirection = new Vector2(force, forceDirection.normalized.y);
            }
            else
            {
                forceDirection = new Vector2(-force, forceDirection.normalized.y);

            }
            rb2d.velocity = new Vector2();
            rb2d.AddForce(forceDirection, ForceMode2D.Impulse);
            yield return new WaitForSeconds(stunTime);
            rb2d.velocity = new Vector2();
        }

        public void ItemInteraction(Item item)
        {
            if (item.category.Equals(Item.Category.CONSUMABLE))
            {
                healthSystem.Heal(item.value);
            }
        }


        //INPUT SYSTEM ACTION METHODS --------------

        //This is called from PlayerInput; when a joystick or arrow keys has been pushed.
        //It stores the input Vector as a Vector3 to then be used by the smoothing function.


        public void OnMovement(InputAction.CallbackContext value)
        {
            Vector2 inputMovement = value.ReadValue<Vector2>();
            rawInputMovement = new Vector2(inputMovement.x, inputMovement.y);
        }

        //This is called from PlayerInput, when a button has been pushed, that corresponds with the 'Attack' action
        public void OnAttack(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                Debug.Log("Attack");
            }
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerMovementBehaviour.Jump();
                Debug.Log("Performed");
            }

            if (value.canceled)
            {
                Debug.Log("Canceled");
            }
        }

        //This is called from Player Input, when a button has been pushed, that correspons with the 'TogglePause' action
        public void OnTogglePause(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                Debug.Log("Pause");
            }
        }
    }
}

