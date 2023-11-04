using SimplePlatformer.Assets.Scripts.Player;
using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Player
{
    public partial class PlayerController : MonoBehaviour
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

        //Movement
        private Vector2 movementDirection;
        [Tooltip("The maximum speed that can perform gradually.")]
        [SerializeField] private float maxSpeed = 10f;

        [Tooltip("Current speed of the object.")]
        [SerializeField] private float speed = 0f;

        [Tooltip("The acceleration is how fast will the object reach the maximum speed.")]
        [SerializeField] private float acceleration = 5f;

        [Tooltip("The decelaration is how fast will the object reach a speed of 0.")]
        [SerializeField] private float decelaration = 5f;

        //Hurt Collider
        BoxCollider2D boxCollider;

        //Particles
        public ParticleSystem dustFootsteps;
        //States
        private bool isJumpingAnim = false;
        private bool isFallingAnim = false;
        private bool isAttackingAnim = false;
        private bool jumpingHeld;
        private bool isJumping;

        //Components
        private SpriteRenderer sprRender;
        private AudioSource footsteps;

        public void FixedUpdate()
        {
            CheckGround();
            if (isGrounded)
            {
                hangTimeCounter = hangTime;
            }
            else
            {
                hangTimeCounter -= Time.deltaTime;
            }

            Move();
            UpdateJump();
            BetterJump();
        }

        private void AnimationUpdate()
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

            isJumpingAnim = info.IsName(PlayerAnimations.PLAYER_JUMP);
            isFallingAnim = info.IsName(PlayerAnimations.PLAYER_FALLING);
            isAttackingAnim = info.IsName(PlayerAnimations.PLAYER_ATTACK1);
            Flip();
            if (!isJumpingAnim && isGrounded && !isAttackingAnim)
            {
                if (movementDirection.x != 0)
                {
                    anim.Play(PlayerAnimations.PLAYER_WALK);
                    dustFootsteps.Play();
                }
                else
                {
                    anim.Play(PlayerAnimations.PLAYER_IDLE);
                }
            }
            if (isJumping && !isJumpingAnim && !isFallingAnim)
            {
                anim.Play(PlayerAnimations.PLAYER_JUMP);
            }
            if (rb.velocity.y < -0.2 && !isGrounded)
            {
                anim.Play(PlayerAnimations.PLAYER_FALLING);
            }
        }

        private void Flip()
        {
            if (movementDirection.x != 0 && !isAttacking)
            {
                if (movementDirection.x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    isFacingRight = true;
                }
                else
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    isFacingRight = false;
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
                isGrounded = true;
                airAttacked = false;
                color = Color.green;
                Debug.DrawRay(raycastPositionLeft, Vector2.down * groundedHeight, color);
                Debug.DrawRay(raycastPositionRight, Vector2.down * groundedHeight, color);
            }
            else
            {
                isGrounded = false;
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
            if (isGrounded) SoundManager.instance.Play("Jump");
            this.jumpingHeld = true;
        }

        public void CancelJumping()
        {
            jumpingHeld = false;
            isJumping = false;
        }

        public void UpdateJump()
        {
            if (hangTimeCounter > 0 && !jumpingHeld)
            {
                isJumping = false;
            }
            if (hangTimeCounter > 0 && jumpingHeld)
            {
                isJumping = true;
            }

            if (isJumping)
            {
                hangTimeCounter = 0f;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        private void Move()
        {
            CalculateSpeed();
            rb.velocity = new Vector2(movementDirection.x * speed, rb.velocity.y);
        }

        private void footstep()
        {
            footsteps.Play();
        }

        /// <summary>
        /// Gradually increments and decreases the speed
        /// </summary>
        private void CalculateSpeed()
        {
            if ((speed < maxSpeed) && movementDirection.x != 0)
            {
                speed += acceleration * Time.deltaTime;
            }
            else
            {
                if (speed > decelaration * Time.deltaTime)
                {
                    speed -= decelaration * Time.deltaTime;
                }
                else
                {
                    speed = 0;
                }
            }
        }
    }//end class
}//end namespace

