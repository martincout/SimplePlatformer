using UnityEngine;

namespace SimplePlatformer.Player
{
    public class PlayerCombat : PlayerBase
    {


        //Attack    
        [Header("Attack")]
        public Transform attackPoint;
        [SerializeField] private float attackRange = 0.1f;
        [SerializeField] private float attackRate = 0.3f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float offsetAttack = 1.4f;
        //The time elapsed for the next hit with the hit box (attack)
        [SerializeField] private float initialDrag;
        [SerializeField] private float attackDrag;

        //Combos
        public enum ComboState
        {
            NONE,
            FIRST,
            SECOND
        }
        private ComboState comboState;

        [SerializeField] private float timeNextCombo = 0.3f;
        private float elapsedNextCombo = 0;
        private float elapsedAttackRate = 0;
        public LayerMask enemyLayer;

        private void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();
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
            Collider2D[] hit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
            foreach (Collider2D col in hit)
            {
                if (col.GetComponent<IDamageable>() != null)
                {
                    col.GetComponent<IDamageable>().TakeDamage(attackDamage, transform.position);
                    //if (!isJumping) StartCoroutine(ImpulseBackwards());
                }
            }
        }

        private void Update()
        {
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
                comboState = ComboState.NONE;
                elapsedAttackRate = attackRate;
                rb2d.drag = initialDrag;
            }

            //Attack rate
            if (elapsedAttackRate > 0)
            {
                elapsedAttackRate -= Time.deltaTime;
            }
            

            //Attack
            if (!isStunned && !itsDying)
            {
                Attack();
            }
        }


        private void OnDrawGizmos()
        {
            if (attackPoint == null)
                return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        private void Attack()
        {
            #region Flip Attack Point
            float axisRaw = Input.GetAxisRaw("Horizontal");
            if (axisRaw != 0 && !isAttacking)
            {
                if (axisRaw > 0)
                {
                    attackPoint.localPosition = new Vector3(offsetAttack, 0, attackPoint.position.z);
                }
                else
                {
                    attackPoint.localPosition = new Vector3(-offsetAttack, 0, attackPoint.position.z);
                }
            }
            #endregion
            #region Check Input
            //Cooldown of the attack finished and if we are not in a Combo
            if (elapsedAttackRate <= 0 || !comboState.Equals(ComboState.NONE))
            {
                //Check for input and if the animation of the combo finished
                if (Input.GetButtonDown("Attack"))
                {
                    isAttacking = true;
                    //If i'm grounded
                    if (!isJumping)
                    {
                        //Check for the Combo state
                        switch (comboState)
                        {
                            case ComboState.NONE:
                                anim.Play(PLAYER_ATTACKING);
                                comboState = ComboState.FIRST;
                                elapsedNextCombo = timeNextCombo;
                                SoundManager.instance.Play("Swish");
                                break;
                            case ComboState.FIRST:
                                anim.Play(PLAYER_COMBO);
                                comboState = ComboState.SECOND;
                                elapsedNextCombo = 0.2f;
                                SoundManager.instance.Play("Swish");
                                break;
                        }
                        
                        
                    }
                    //If I'm in the Air
                    else if (!airAttacked)
                    {
                        comboState = ComboState.FIRST;
                        SoundManager.instance.Play("Swish");
                        elapsedNextCombo = 0.2f;
                        anim.Play(PLAYER_AIRATTACK);
                        airAttacked = true;

                    }
                    //don't slide on the floor
                    rb2d.drag = attackDrag;

                }
            }
            #endregion
        }

        //private IEnumerator ImpulseBackwards()
        //{
        //    //The force it's greater as the velocity of the player increases to apply more impulse backwards
        //    float velocityX = rb2d.velocity.x;
        //    float force = 1.4f + Mathf.Abs(velocityX);

        //    if (isFacingRight)
        //    {
        //        rb2d.AddForce(new Vector2(-force, 0), ForceMode2D.Impulse);
        //    }
        //    else
        //    {
        //        rb2d.AddForce(new Vector2(force, 0), ForceMode2D.Impulse);
        //    }
        //    yield return new WaitForSeconds(0.1f);
        //    axisDir = Vector2.zero;
        //}

    }

}
