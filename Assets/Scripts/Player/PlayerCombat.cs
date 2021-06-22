using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Player
{
    public class PlayerCombat : PlayerBase
    {
        //Attack    
        [Header("Attack")]
        public Transform hitBoxPos;
        [SerializeField] private float attackRate = 0.3f;
        [SerializeField] private float attackDamage = 10f;
        //The time elapsed for the next hit with the hit box (attack)
        [SerializeField] private float initialDrag;
        [SerializeField] private float attackDrag;
        [Header("HitBox")]
        public Vector3 boxSize;
        public float rotation;


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
            
        }


        public void OnDrawGizmos()
        {
            if (hitBoxPos == null)
                return;
            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(hitBoxPos.position, hitBoxPos.rotation, hitBoxPos.localScale);

            Gizmos.DrawCube(Vector3.zero, boxSize); // Because size is halfExtents
        }
        public void Attack()
        {

            #region Check Input
            //Cooldown of the attack finished and if we are not in a Combo
            if (elapsedAttackRate <= 0 || !comboState.Equals(ComboState.NONE))
            {
                //Check for input and if the animation of the combo finished
                
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
                                break;
                            case ComboState.FIRST:
                                anim.Play(PLAYER_COMBO);
                                comboState = ComboState.SECOND;
                                elapsedNextCombo = 0.2f;
                                break;
                        }


                    }
                    //If I'm in the Air
                    else if (!airAttacked)
                    {
                        comboState = ComboState.FIRST;

                        elapsedNextCombo = 0.2f;
                        anim.Play(PLAYER_AIRATTACK);
                        airAttacked = true;

                    }
                    //don't slide on the floor
                    rb2d.drag = attackDrag;

                
            }
            #endregion
        }

        public void SwishSound()
        {
            SoundManager.instance.Play("Swish");
        }

    }

}
