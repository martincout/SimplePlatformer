using SimplePlatformer.Assets.Scripts.Player;
using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Player
{
    public partial class PlayerController : MonoBehaviour
    {
        PlayerController playerController;
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
        [SerializeField] public bool hasBow = false;
        [Header("HitBox")]
        public Vector3 boxSize;
        public float rotation; 
        [HideInInspector] public bool hitboxEnable;

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
                        playerController.Knockback(col.transform.position, 15f);
                    }
                    hitboxEnable = false;
                    //if (!isJumping) StartCoroutine(ImpulseBackwards());
                }
            }
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
            if (!isStunned && !isBowAttacking)
            {
                #region Attack
                //Cooldown of the attack finished and if we are not in a Combo
                if (elapsedAttackRate <= 0 || !comboState.Equals(ComboState.NONE))
                {

                    //If i'm grounded
                    if (isGrounded)
                    {
                        isAttacking = true;
                        //Check for the Combo state
                        switch (comboState)
                        {
                            case ComboState.NONE:
                                anim.Play(PlayerAnimations.PLAYER_ATTACK1);
                                comboState = ComboState.FIRST;
                                elapsedNextCombo = timeNextCombo;
                                break;
                            case ComboState.FIRST:
                                anim.Play(PlayerAnimations.PLAYER_ATTACK2);
                                comboState = ComboState.SECOND;
                                elapsedNextCombo = 0.3f;
                                elapsedAttackRate = 1f;
                                break;
                            case ComboState.SECOND:
                                anim.Play(PlayerAnimations.PLAYER_ATTACK3);
                                comboState = ComboState.THIRD;
                                elapsedNextCombo = 0.4f;
                                break;
                        }
                        //don't slide on the floor
                        rb.drag = attackDrag;
                    }
                    //If I'm in the Air
                    else if (!airAttacked)
                    {
                        isAttacking = true;
                        //Check for the Combo state
                        switch (comboState)
                        {
                            case ComboState.NONE:
                                //SetSuspendInAir();
                                anim.Play(PlayerAnimations.PLAYER_AIRATTACK);
                                comboState = ComboState.FIRST;
                                elapsedNextCombo = timeNextCombo;
                                break;
                            case ComboState.FIRST:
                                anim.Play(PlayerAnimations.PLAYER_AIRATTACK2);
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
            if (!hasBow) return;

            if (elapsedAttackRate <= 0)
            {
                isAttacking = true;
                isBowAttacking = true;
                StartCoroutine(EnableMovementAfter(0.7f));

                //Animation
                if (isGrounded)
                {
                    anim.Play(PlayerAnimations.PLAYER_BOW);

                }
                else
                {
                    anim.Play(PlayerAnimations.PLAYER_BOWAIR);
                }

                //Instatiate
                StartCoroutine(InstantiateArrow(0.4f));

                //Timers and Combo
                comboState = ComboState.FIRST;
                elapsedAttackRate = 1f;
                elapsedNextCombo = 0.7f;

                //Sets gravity to 0 when in the air
                if (!isGrounded)
                {
                    SetSuspendInAir();
                }
            }

        }

        private IEnumerator InstantiateArrow(float _sec)
        {
            yield return new WaitForSeconds(_sec);
            GameObject instance = Instantiate(arrowPF, arrowTR.position, Quaternion.identity);
            instance.GetComponent<Arrow>().Setup(isFacingRight);
            ManageArrows.AddArrow(instance.GetComponent<Arrow>());
        }

        private void SetSuspendInAir()
        {
            rb.gravityScale = 0;
        }

        private void SuspendInAir()
        {
            if (isAttacking && !isGrounded)
            {
                rb.velocity = Vector2.zero;
            }
            else
            {
                rb.gravityScale = 1f;
            }
        }

        public void SwishSound()
        {
            SoundManager.instance.Play("Swish");
        }

        private IEnumerator EnableMovementAfter(float _sec)
        {
            movePrevent = true;
            yield return new WaitForSeconds(_sec);
            movePrevent = false;
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
