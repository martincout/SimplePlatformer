using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles States
/// </summary>
namespace SimplePlatformer.Player
{
    public class PlayerController : MonoBehaviour, IDamageable, IItem
    {
        [Header("Sub Behaviours")]
        public PlayerMovement playerMovementBehaviour;
        public PlayerCombat playerCombatBehaviour;
        public PlayerInteractable playerInteractableBehaviour;

        public static PlayerVariables pv = new PlayerVariables();

        //Action Maps
        private string actionMapPlayerControls = "PlayerControlls";
        private string actionMapMenuControls = "UI";

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


        public static PlayerInput playerInput;
        protected static bool jumpingHeld = false;

        public float thrust = 10f;
        private string currentControlScheme;

        public bool GetPlayerItsDying()
        {
            return pv.itsDying;
        }

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            playerMovementBehaviour = GetComponent<PlayerMovement>();
            playerCombatBehaviour = GetComponent<PlayerCombat>();
            playerInteractableBehaviour = GetComponent<PlayerInteractable>();
            healthSystem = GetComponent<HealthSystem>();
            characterParticles = GetComponent<CharacterParticles>();
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            render = transform.GetChild(0).GetComponent<Renderer>();

        }
        private void Start()
        {
            currentControlScheme = "Keyboard&Mouse";
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
            playerInput.SwitchCurrentActionMap("PlayerControlls");
            healthSystem.SetHealthBar(GameObject.Find("HealthBar").GetComponent<HealthBar>());
            pv.cannotAttack = false;
            pv.movePrevent = false;
            pv.isFacingRight = true;
            pv.isJumping = false;
            pv.isAttacking = false;
            pv.isStunned = false;
            pv.itsDying = false;
            pv.invincible = false;
            pv.airAttacked = false;
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
                pv.invincible = false;
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
            pv.movePrevent = true;
            yield return new WaitForSeconds(seconds);
            pv.movePrevent = false;
        }

        internal void SetInvincible(float cooldown)
        {
            pv.invincible = true;
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
            if (!pv.invincible && !pv.itsDying)
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
                    anim.Play(PlayerVariables.PLAYER_HURT);
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
            pv.itsDying = true;
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerCombat>().enabled = false;
            rb2d.velocity = Vector2.zero;
            anim.Play("playerDie");
            yield return new WaitForSeconds(0.55f);
            EventSystems.OnPlayerDeath?.Invoke();
            GameManager.GetInstance().TogglePlayerDeath(true);
            pv.itsDying = false;
            Destroy(gameObject);
        }

        protected void Knockback(Vector3 attackerPos, float thrust)
        {
            if (!pv.isStunned)
            {
                StartCoroutine(StunCo());
                StartCoroutine(KnockCo(attackerPos, thrust));
            }
        }

        private IEnumerator StunCo()
        {
            pv.isStunned = true;
            yield return new WaitForSeconds(stunTime);
            pv.isStunned = false;
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

        /// <summary>
        /// Item interaction, only with health items
        /// </summary>
        /// <param name="item"></param>
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
            rawInputMovement = new Vector2(inputMovement.x, rb2d.velocity.y);
        }

        //This is called from PlayerInput, when a button has been pushed, that corresponds with the 'Attack' action
        public void OnAttack(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerCombatBehaviour.Attack();
            }
        }

        //This has a hold Interaction. When the hold ends, starts falling
        public void OnJump(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerMovementBehaviour.StartJumping();
            }
            if (value.performed || value.canceled)
            {
                playerMovementBehaviour.CancelJumping();
            }

        }

        public void OnInteract(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInteractableBehaviour.Interact();
            }
        }

        //This is called from Player Input, when a button has been pushed, that correspons with the 'TogglePause' action
        public void OnTogglePause(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                GameManager.GetInstance().TogglePauseState();
            }
        }

        //INPUT SYSTEM AUTOMATIC CALLBACKS --------------

        //This is automatically called from PlayerInput, when the input device has changed
        //(IE: Keyboard -> Xbox Controller)
        public void OnControlsChanged()
        {

            if (playerInput.currentControlScheme != currentControlScheme)
            {

                currentControlScheme = playerInput.currentControlScheme;

                //playerVisualsBehaviour.UpdatePlayerVisuals();
                RemoveAllBindingOverrides();
            }
        }

        //This is automatically called from PlayerInput, when the input device has been disconnected and can not be identified
        //IE: Device unplugged or has run out of batteries

        public void OnDeviceLost()
        {
            currentControlScheme = playerInput.currentControlScheme;
            //playerVisualsBehaviour.SetDisconnectedDeviceVisuals();
        }


        public void OnDeviceRegained()
        {
            StartCoroutine(WaitForDeviceToBeRegained());
        }

        IEnumerator WaitForDeviceToBeRegained()
        {
            yield return new WaitForSeconds(0.1f);
            //playerVisualsBehaviour.UpdatePlayerVisuals();
        }

        void RemoveAllBindingOverrides()
        {
            InputActionRebindingExtensions.RemoveAllBindingOverrides(playerInput.currentActionMap);
        }

        //Switching Action Maps ----

        public void EnableGameplayControls()
        {
            playerInput.SwitchCurrentActionMap(actionMapPlayerControls);
        }

        public void EnablePauseMenuControls()
        {
            playerInput.SwitchCurrentActionMap(actionMapMenuControls);
        }

    }
}

