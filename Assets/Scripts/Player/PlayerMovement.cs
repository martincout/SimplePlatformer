using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Player
{
    public class PlayerMovement : PlayerBase
    {
        //Jump
        [SerializeField] private float fallMultiplier = 2f;
        [SerializeField] private float lowFallMultiplier = 8f;
        [SerializeField] private float jumpForce = 4f;
        //Movement
        [SerializeField] float speed;
        //Check
        public float groundedHeight = 0.5f;
        public float heightOffset = 0.25f; // we dont want to cast from the players feet (may cast underground sometimes), so we offset it a bit
        BoxCollider2D boxCollider2d;
        public LayerMask groundLayer;
        internal bool isGrounded;

        public ParticleSystem dustFootsteps;

        private bool isJumpingAnim = false;
        private bool isFallingAnim = false;
        private bool isAttackingAnim = false;

        //Components
        private SpriteRenderer sprRender;
        private AudioSource footsteps;

        public void Awake()
        {
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            sprRender = GetComponent<SpriteRenderer>();
            groundLayer = 1 << LayerMask.NameToLayer("Ground");
            boxCollider2d = GetComponent<BoxCollider2D>();
            footsteps = GetComponent<AudioSource>();
            dustFootsteps = transform.GetChild(1).GetComponent<ParticleSystem>();
        }

        public void Start()
        {
            isGrounded = false;
        }

        public void FixedUpdate()
        {
            CheckGround();
            if (!isStunned)
            {
                Move();
            }
            BetterJump();
        }
        private void Update()
        {
            if (!isStunned)
            {
                if (!isAttacking)
                {
                    AnimationUpdate();
                }
                Movement();
                Jump();
            }
        }

        private void AnimationUpdate()
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

            isJumpingAnim = info.IsName(PLAYER_JUMP);
            isFallingAnim = info.IsName(PLAYER_FALLING);
            isAttackingAnim = info.IsName(PLAYER_ATTACKING);
            Flip();
            if (!isJumpingAnim && isGrounded && !isAttackingAnim)
            {
                if (axisDir.x != 0)
                {
                    anim.Play(PLAYER_WALK);
                    dustFootsteps.Play();
                }
                else
                {
                    anim.Play(PLAYER_IDLE);
                }
            }
            if (Input.GetButtonDown("Jump") && !isJumpingAnim && !isFallingAnim)
            {
                anim.Play(PLAYER_JUMP);
            }
            if (rb2d.velocity.y < -0.2)
            {
                anim.Play(PLAYER_FALLING);
            }
        }

        private void Flip()
        {
            if (Input.GetAxisRaw("Horizontal") != 0 && !isAttacking)
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    sprRender.flipX = false;
                    isFacingRight = true;
                }
                else
                {
                    sprRender.flipX = true;
                    isFacingRight = false;
                }
            }
        }

        private void BetterJump()
        {
            if (rb2d.velocity.y < 0)
            {
                rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb2d.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowFallMultiplier - 1) * Time.deltaTime;
            }
        }

        private void CheckGround()
        {
            RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, heightOffset, groundLayer);
            Color rayColor;
            if (raycastHit.collider != null)
            {
                isGrounded = true;
                airAttacked = false;
                rayColor = Color.green;
            }
            else
            {
                isGrounded = false;
                rayColor = Color.red;
            }
            Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + heightOffset), rayColor);
            Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + heightOffset), rayColor);
            Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, boxCollider2d.bounds.extents.y + heightOffset), Vector2.right * (boxCollider2d.bounds.extents.x * 2f), rayColor);

        }


        private void Movement()
        {
            if (Input.GetButton("Horizontal") && !movePrevent)
            {
                axisDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxis("Vertical"));
            }
            else
            {
                axisDir = Vector2.zero;
            }
        }

        private void Jump()
        {
            if (isGrounded)
            {
                isJumping = false;
                if (Input.GetButtonDown("Jump"))
                {
                    isJumping = true;
                    SoundManager.instance.Play("Jump");
                    rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y);
                    rb2d.velocity = Vector2.up * jumpForce;
                }
            }
            else
            {
                isJumping = true;
            }
        }

        private void Move()
        {
            rb2d.velocity = new Vector2(axisDir.x * speed, rb2d.velocity.y);
        }

        private void footstep()
        {
            footsteps.Play();
        }
    }//end class
}//end namespace

