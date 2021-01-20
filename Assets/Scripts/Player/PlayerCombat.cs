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
        [SerializeField] private bool checkForHitBox = false;
        [SerializeField] private int manyHits = 1;

        [Header("Hurt")]

        private float coolDownAttack = 0f;
        public LayerMask enemyLayer;

        private void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            render = GetComponent<Renderer>();
        }
        private void FixedUpdate()
        {
            if (checkForHitBox)
            {
                Collider2D[] hit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
                foreach (Collider2D col in hit)
                {
                    if (col.GetComponent<IDamagable>() != null && manyHits >= 1)
                    {
                        col.GetComponent<IDamagable>().TakeDamage(attackDamage, transform.position);
                        manyHits -= 1;
                        //if (!isJumping) StartCoroutine(ImpulseBackwards());
                    }
                }
            }
            else
            {
                manyHits = 0;
            }
        }

        private void Update()
        {

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
            #region Check Collision
            
            if (Time.time > coolDownAttack)
            {
                isAttacking = false;
                rb2d.drag = 1;
                if (Input.GetButtonDown("Attack"))
                {
                    isAttacking = true;

                    if (!isJumping)
                    {
                        //slides on the floor
                        rb2d.drag = 8;
                        SoundManager.instance.Play("Swish");
                        coolDownAttack = Time.time + attackRate;
                        anim.Play(PLAYER_ATTACKING);
                    }
                    else if (!airAttacked)
                    {
                        SoundManager.instance.Play("Swish");
                        coolDownAttack = Time.time + 0.2f;
                        anim.Play(PLAYER_AIRATTACK);
                        airAttacked = true;

                    }

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
