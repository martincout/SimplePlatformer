using SimplePlatformer.Assets.Scripts.Player;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace SimplePlatformer.Player
{
    /// <summary>
    /// Handles all the player general behaviour.
    /// - Input
    /// - Health, Damage, Knockback
    /// </summary>
    public partial class PlayerController : MonoBehaviour, IDamageable, IItem
    {
        [Header("Sub Behaviours")]
        public PlayerInteractable playerInteractableBehaviour;
        public float MaxHealth = 200;

        public HealthBar healthBar;

        //Action Maps
        private string actionMapPlayerControls = "PlayerControlls";
        private string actionMapMenuControls = "UI";

        //Aux
        protected float invincibleTime = 1f;
        protected static Vector2 rawInputMovement;

        //Take damage
        private float stunTime = 0.3f;
        internal HealthSystem healthSystem;

        protected CharacterParticles characterParticles;
        protected Animator anim;
        protected Rigidbody2D rb2d;
        protected Renderer render;
        private PlayerInput _playerInput;

        public bool isJumping;
        public bool movePrevent;
        public bool cannotAttack;
        public bool isFacingRight = true;
        public bool isAttacking;
        public bool isStunned;
        public bool itsDying;
        public bool invincible;
        public bool airAttacked;
        public bool isGrounded;
        public bool canInteract = true;
        public bool isBowAttacking;

        public float thrust = 10f;
        private string currentControlScheme;

        public bool GetPlayerItsDying()
        {
            return itsDying;
        }

        private void Awake()
        {
            //General
            _playerInput = GetComponent<PlayerInput>();
            playerInteractableBehaviour = GetComponent<PlayerInteractable>();
            healthSystem = gameObject.AddComponent<HealthSystem>();
            characterParticles = GetComponent<CharacterParticles>();
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            render = transform.GetChild(0).GetComponent<Renderer>();
            //Combat
            hitBoxPos = transform.GetChild(1).transform;
            render = GetComponent<Renderer>();
            comboState = ComboState.NONE;
            //Movement
            sprRender = GetComponent<SpriteRenderer>();
            groundLayer = 1 << LayerMask.NameToLayer("Ground");
            boxCollider = GetComponent<BoxCollider2D>();
            footsteps = GetComponent<AudioSource>();
            dustFootsteps = transform.GetChild(2).GetComponent<ParticleSystem>();
        }
        private void Start()
        {
            if (healthBar == null) healthBar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
            healthSystem.SetHealthBar(healthBar);
            healthSystem.SetMaxHealth(MaxHealth);
        }

        private void Update()
        {
            if (Time.timeScale == 0) return;

            //Movement
            if (!isStunned && !movePrevent)
            {
                if (!isAttacking)
                {
                    AnimationUpdate();
                }
            }

            UpdateMovementData(rawInputMovement);

            //Combat
            CheckHitBoxColission();
            //Suspend the player in air when air attacking
            SuspendInAir();

            //Used to exit the animation state when we are doing combos
            anim.SetFloat("timeCombo", elapsedNextCombo);
            //Combos
            if (elapsedNextCombo > 0)
            {
                elapsedNextCombo -= Time.deltaTime;
            }
            //Finished Attacking
            else if (!comboState.Equals(ComboState.NONE))
            {
                isAttacking = false;
                airAttacked = true;
                isBowAttacking = false;
                comboState = ComboState.NONE;
                elapsedAttackRate = attackRate;
                rb.drag = initialDrag;
            }

            //Attack rate
            if (elapsedAttackRate > 0)
            {
                elapsedAttackRate -= Time.deltaTime;
            }

           
        }

        internal IEnumerator InvinbibleCo()
        {
            yield return new WaitForSeconds(invincibleTime);
            SetAlpha(1f);
            invincible = false;
        }

        internal void SetInvincible()
        {
            invincible = true;
            SetAlpha(0.7f);
            StartCoroutine(InvinbibleCo());
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
                SetInvincible();
                //Check
                if (healthSystem.GetHealth() > 0)
                {
                    SoundManager.instance.Play("Damage");
                    anim.Play(PlayerAnimations.PLAYER_HURT);
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
            GameEvents.OnPlayerDeath?.Invoke();
            GameManager.GetInstance().TogglePlayerDeath(true);
            itsDying = false;
            Destroy(gameObject);
        }

        internal void Knockback(Vector3 attackerPos, float thrust)
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

        public void DisablePlayerState(bool set)
        {
            switch (set)
            {
                case true:
                    cannotAttack = true;
                    movePrevent = true;
                    canInteract = false;
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    anim.Play(PlayerAnimations.PLAYER_IDLE);
                    break;
                case false:
                    cannotAttack = false;
                    movePrevent = false;
                    canInteract = true;
                    break;
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

        public void OnBowAttack(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerCombatBehaviour.BowAttack();
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

            if (_playerInput.currentControlScheme != currentControlScheme)
            {

                currentControlScheme = _playerInput.currentControlScheme;

                //playerVisualsBehaviour.UpdatePlayerVisuals();
                RemoveAllBindingOverrides();
            }
        }

        //This is automatically called from PlayerInput, when the input device has been disconnected and can not be identified
        //IE: Device unplugged or has run out of batteries

        public void OnDeviceLost()
        {
            currentControlScheme = _playerInput.currentControlScheme;
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
            InputActionRebindingExtensions.RemoveAllBindingOverrides(_playerInput.currentActionMap);
        }

        //Switching Action Maps ----

        public void EnableGameplayControls()
        {
            //_playerInput.SwitchCurrentActionMap(actionMapPlayerControls);
        }

        public void EnablePauseMenuControls()
        {
            _playerInput.SwitchCurrentActionMap(actionMapMenuControls);
        }

    }
}

