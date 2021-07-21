using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Player
{
    public class PlayerCombat : PlayerController
    {
        //Attack    
        [Header("Attack")]
        public Transform hitBoxPos;
        [SerializeField] private float attackRate = 0.3f;
        [SerializeField] private float attackDamage = 10f;
        //The time elapsed for the next hit with the hit box (attack)
        [SerializeField] private float initialDrag;
        [SerializeField] private float attackDrag;
        [SerializeField] private GameObject arrowPF;
        [SerializeField] private Transform arrowTR;
        [SerializeField] private bool hasBow;
        [Header("HitBox")]
        public Vector3 boxSize;
        public float rotation;
        private bool hitboxEnable;

        //Combos
        public enum ComboState
        {
            NONE,
            FIRST,
            SECOND,
            THIRD
        }
        private ComboState comboState;

        private float timeNextCombo = 0.3f;
        [SerializeField] private float elapsedNextCombo = 0;
        [SerializeField] private float elapsedAttackRate = 0;
        public LayerMask enemyLayer;
        public LayerMask damageableLayer;

        private void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();
            hitBoxPos = transform.GetChild(1).transform;
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            render = GetComponent<Renderer>();
        }
        private void Start()
        {
            comboState = ComboState.NONE;
        }
        public void CheckHitBoxColission()
        {
            if (!hitboxEnable) return;
            Collider2D[] hit = Physics2D.OverlapBoxAll(hitBoxPos.position, boxSize, rotation, enemyLayer | damageableLayer);
            foreach (Collider2D col in hit)
            {
                if (col.GetComponent<IDamageable>() != null)
                {
                    col.GetComponent<IDamageable>().TakeDamage(attackDamage, transform.position);
                    if (col.GetComponent<FallingCellCage>())
                    {
                        Knockback(col.transform.position, 15f);
                    }
                    hitboxEnable = false;
                    //if (!isJumping) StartCoroutine(ImpulseBackwards());
                }
            }
        }

        private void Update()
        {
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
                pv.isAttacking = false;
                pv.airAttacked = true;
                pv.isBowAttacking = false;
                comboState = ComboState.NONE;
                elapsedAttackRate = attackRate;
                rb2d.drag = initialDrag;
            }

            //Attack rate
            if (elapsedAttackRate > 0)
            {
                elapsedAttackRate -= Time.deltaTime;
            }

        }


        public void OnDrawGizmosSelected()
        {
            if (hitBoxPos == null)
                return;
            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(hitBoxPos.position, hitBoxPos.rotation, hitBoxPos.localScale);

            Gizmos.DrawCube(Vector3.zero, boxSize); // Because size is halfExtents
        }

        /// <summary>
        /// Handles all the attack behaviour, is called by the Input System
        /// - Checks for states
        /// - Sets isAttacking
        /// - Handles the combo attacks and times
        /// - Floor drag
        /// </summary>
        public void Attack()
        {
            if (!pv.isStunned && !pv.itsDying && !pv.cannotAttack && !pv.isBowAttacking)
            {
                #region Attack
                //Cooldown of the attack finished and if we are not in a Combo
                if (elapsedAttackRate <= 0 || !comboState.Equals(ComboState.NONE))
                {

                    //If i'm grounded
                    if (pv.isGrounded)
                    {
                        pv.isAttacking = true;
                        //Check for the Combo state
                        switch (comboState)
                        {
                            case ComboState.NONE:
                                anim.Play(PlayerVariables.PLAYER_ATTACK1);
                                comboState = ComboState.FIRST;
                                elapsedNextCombo = timeNextCombo;
                                break;
                            case ComboState.FIRST:
                                anim.Play(PlayerVariables.PLAYER_ATTACK2);
                                comboState = ComboState.SECOND;
                                elapsedNextCombo = 0.3f;
                                elapsedAttackRate = 1f;
                                break;
                            case ComboState.SECOND:
                                anim.Play(PlayerVariables.PLAYER_ATTACK3);
                                comboState = ComboState.THIRD;
                                elapsedNextCombo = 0.4f;

                                break;
                        }
                        //don't slide on the floor
                        //rb2d.drag = attackDrag;
                    }
                    //If I'm in the Air
                    else if (!pv.airAttacked)
                    {
                        pv.isAttacking = true;
                        //Check for the Combo state
                        switch (comboState)
                        {
                            case ComboState.NONE:
                                //SetSuspendInAir();
                                anim.Play(PlayerVariables.PLAYER_AIRATTACK);
                                comboState = ComboState.FIRST;
                                elapsedNextCombo = timeNextCombo;
                                break;
                            case ComboState.FIRST:
                                anim.Play(PlayerVariables.PLAYER_AIRATTACK2);
                                comboState = ComboState.SECOND;
                                elapsedNextCombo = 0.4f;
                                break;
                        }
                    }

                }
            }
            #endregion
        }

        /// <summary>
        /// Handles the bow Attack Behaviour.
        /// - Instantiates the arrow
        /// - Sets isAttacking
        /// - Timers
        /// - Floor drag
        /// </summary>
        public void BowAttack()
        {
            if (!pv.isStunned && !pv.itsDying && !pv.cannotAttack)
            {
                if (elapsedAttackRate <= 0)
                {
                    pv.isAttacking = true;
                    pv.isBowAttacking = true;
                    StartCoroutine(EnableMovementAfter(0.7f));

                    //Animation
                    if (pv.isGrounded)
                    {
                        anim.Play(PlayerVariables.PLAYER_BOW);

                    }
                    else
                    {
                        anim.Play(PlayerVariables.PLAYER_BOWAIR);
                    }

                    //Instatiate
                    StartCoroutine(InstantiateArrow(0.4f));

                    //Timers and Combo
                    comboState = ComboState.FIRST;
                    elapsedAttackRate = 1f;
                    elapsedNextCombo = 0.7f;

                    //Sets gravity to 0 when in the air
                    if (!pv.isGrounded)
                    {
                        SetSuspendInAir();
                    }
                }
            }
        }

        private IEnumerator InstantiateArrow(float _sec)
        {
            yield return new WaitForSeconds(_sec);
            GameObject instance = Instantiate(arrowPF, arrowTR.position, Quaternion.identity);
            instance.GetComponent<Arrow>().Setup(pv.isFacingRight);
            ManageArrows.AddArrow(instance.GetComponent<Arrow>());
        }

        private void SetSuspendInAir()
        {
            rb2d.gravityScale = 0;
        }

        private void SuspendInAir()
        {
            if (pv.isAttacking && !pv.isGrounded)
            {
                if (!pv.isStunned) rb2d.velocity = Vector2.zero;
            }
            else
            {
                rb2d.gravityScale = 1f;
            }
        }

        public void SwishSound()
        {
            SoundManager.instance.Play("Swish");
        }


        /// <summary>
        /// Enables the Hitbox turning hitboxEnable true, and then false
        /// </summary>
        /// <param name="_s"></param>
        /// <returns></returns>
        public IEnumerator EnableHitboxForSeconds(float _s)
        {
            hitboxEnable = true;
            yield return new WaitForSeconds(_s);
            hitboxEnable = false;
        }

    }

}
