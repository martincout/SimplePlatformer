using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        //Jump

        [Tooltip("Multipies the fall after jump"), SerializeField] private float fallMultiplier = 2f;
        [Tooltip("When the player Stops Pressing Jump in mid air, multiplies the fall"), SerializeField] private float lowFallMultiplier = 8f;
        [SerializeField] private float jumpForce = 4f;
        [Tooltip("Hang time in the air"), SerializeField] private float hangTime = 0.2f;
        [Tooltip("Hang time counter, decreasing value"), SerializeField] private float hangTimeCounter = 0f;
        //Check Ground
        public float groundedHeight = 0.5f;
        public float heightOffset = 0.25f; // we dont want to cast from the players feet (may cast underground sometimes), so we offset it a bit
        public LayerMask groundLayer;
        /// <summary>
        /// Raycast to check if it's grounded
        /// </summary>
        private RaycastHit2D raycastLeft;
        private RaycastHit2D raycastRight;
        /// <summary>
        /// Offset from the center-bottom of the collider 2d
        /// </summary>
        public Vector3 raycastLeftOffset = new Vector2(0.5f, 0);
        public Vector3 raycastRightOffset = new Vector2(0.5f, 0);

        private PlayerVariables pv;

        //Movement
        [SerializeField] float speed;
        private Vector2 movementDirection;

        //Hurt Collider
        BoxCollider2D boxCollider;

        //Particles
        public ParticleSystem dustFootsteps;
        //States
        private bool isJumpingAnim = false;
        private bool isFallingAnim = false;
        private bool isAttackingAnim = false;
        private bool jumpingHeld;

        //Components
        private SpriteRenderer sprRender;
        private AudioSource footsteps;
        private Rigidbody2D rb;
        private Animator anim;
       
        public void Setup(PlayerVariables pv)
        {
            this.pv = pv;
        }

        public void Awake()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            sprRender = GetComponent<SpriteRenderer>();
            groundLayer = 1 << LayerMask.NameToLayer("Ground");
            boxCollider = GetComponent<BoxCollider2D>();
            footsteps = GetComponent<AudioSource>();
            dustFootsteps = transform.GetChild(2).GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            pv = new PlayerVariables();
        }

        public void FixedUpdate()
        {
            if (pv.movePrevent) 
                rb.velocity = Vector2.zero;
            CheckGround();
            if (pv.isGrounded)
            {
                hangTimeCounter = hangTime;
            }
            else
            {
                hangTimeCounter -= Time.deltaTime;
            }

            if (!pv.isStunned && !pv.movePrevent)
            {
                Move();
                JumpUpdate();
                BetterJump();
            }
        }
        private void Update()
        {
            if (pv.movePrevent) return;
            if (!pv.isStunned)
            {
                if (!pv.isAttacking)
                {
                    AnimationUpdate();
                }
            }
        }

        public void UpdateMovementData(Vector2 newMovementDirection)
        {
            movementDirection = newMovementDirection;
        }


        private void AnimationUpdate()
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

            isJumpingAnim = info.IsName(PlayerVariables.PLAYER_JUMP);
            isFallingAnim = info.IsName(PlayerVariables.PLAYER_FALLING);
            isAttackingAnim = info.IsName(PlayerVariables.PLAYER_ATTACK1);
            Flip();
            if (!isJumpingAnim && pv.isGrounded && !isAttackingAnim)
            {
                if (movementDirection.x != 0)
                {
                    anim.Play(PlayerVariables.PLAYER_WALK);
                    dustFootsteps.Play();
                }
                else
                {
                    anim.Play(PlayerVariables.PLAYER_IDLE);
                }
            }
            if (pv.isJumping && !isJumpingAnim && !isFallingAnim)
            {
                anim.Play(PlayerVariables.PLAYER_JUMP);
            }
            if (rb.velocity.y < -0.2 && !pv.isGrounded)
            {
                anim.Play(PlayerVariables.PLAYER_FALLING);
            }
        }

        private void Flip()
        {
            if (movementDirection.x != 0 && !pv.isAttacking)
            {
                if (movementDirection.x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    pv.isFacingRight = true;
                }
                else
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    pv.isFacingRight = false;
                }
            }
        }

        private void CheckGround()
        {
            //Raycast position calculation. (from the center-bottom of the collider 2d, with an offset)
            Vector3 raycastPositionLeft = boxCollider.bounds.center - new Vector3(0, boxCollider.bounds.extents.y, 0) - raycastLeftOffset;
            Vector3 raycastPositionRight = boxCollider.bounds.center - new Vector3(0, boxCollider.bounds.extents.y, 0) + raycastRightOffset;
            //Raycast2d
            raycastLeft = Physics2D.Raycast(raycastPositionLeft, Vector2.down, groundedHeight, groundLayer);
            raycastRight = Physics2D.Raycast(raycastPositionRight, Vector2.down, groundedHeight, groundLayer);

            Color color;
            //Check if grounded
            if (raycastLeft.collider != null || raycastRight.collider != null)
            {
                pv.isGrounded = true;
                pv.airAttacked = false;
                color = Color.green;
                Debug.DrawRay(raycastPositionLeft, Vector2.down * groundedHeight, color);
                Debug.DrawRay(raycastPositionRight, Vector2.down * groundedHeight, color);
            }
            else
            {
                pv.isGrounded = false;
                color = Color.red;
                Debug.DrawRay(raycastPositionLeft, Vector2.down * groundedHeight, color);
                Debug.DrawRay(raycastPositionRight, Vector2.down * groundedHeight, color);
            }

        }

        //JUMP METHODS --------------

        private void BetterJump()
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !jumpingHeld)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowFallMultiplier - 1) * Time.deltaTime;
            }

        }

        public void StartJumping()
        {
            if (pv.isGrounded && !pv.movePrevent) SoundManager.instance.Play("Jump");
            this.jumpingHeld = true;
        }

        public void CancelJumping()
        {
            jumpingHeld = false;
            pv.isJumping = false;
        }

        public void JumpUpdate()
        {
            if (hangTimeCounter > 0 && !jumpingHeld)
            {
                pv.isJumping = false;
            }
            if (hangTimeCounter > 0 && jumpingHeld)
            {
                pv.isJumping = true;
            }

            if (pv.isJumping)
            {
                hangTimeCounter = 0f;
                float multiplies = 50;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * (Time.deltaTime * multiplies));
            }

        }


        private void Move()
        {
            float multiplies = 50;
            rb.velocity = new Vector2(movementDirection.x * speed * (Time.deltaTime * multiplies), rb.velocity.y);
        }

        private void footstep()
        {
            footsteps.Play();
        }
    }//end class
}//end namespace

