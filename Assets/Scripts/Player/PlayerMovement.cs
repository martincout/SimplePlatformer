﻿using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Player
{
    public class PlayerMovement : PlayerController
    {
        //Jump

        [SerializeField] private float fallMultiplier = 2f;
        [SerializeField] private float lowFallMultiplier = 8f;
        [SerializeField] private float jumpForce = 4f;
        //Check Ground
        public float groundedHeight = 0.5f;
        public float heightOffset = 0.25f; // we dont want to cast from the players feet (may cast underground sometimes), so we offset it a bit
        public LayerMask groundLayer;
        //Movement
        [SerializeField] float speed;
        private Vector2 movementDirection;

        //Hurt Collider
        CapsuleCollider2D capsuleCollider;

        //Particles
        public ParticleSystem dustFootsteps;
        //States

        private bool isJumpingAnim = false;
        private bool isFallingAnim = false;
        private bool isAttackingAnim = false;

        //Components
        private SpriteRenderer sprRender;
        private AudioSource footsteps;

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

        public void Awake()
        {
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            sprRender = GetComponent<SpriteRenderer>();
            groundLayer = 1 << LayerMask.NameToLayer("Ground");
            capsuleCollider = GetComponent<CapsuleCollider2D>();
            footsteps = GetComponent<AudioSource>();
            dustFootsteps = transform.GetChild(2).GetComponent<ParticleSystem>();
        }

        public void Start()
        {
            pv.isGrounded = false;
        }

        public void FixedUpdate()
        {
            CheckGround();
            if (!pv.isStunned)
            {
                Move();
            }
            JumpUpdate();
            BetterJump();
        }
        private void Update()
        {
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
            isAttackingAnim = info.IsName(PlayerVariables.PLAYER_ATTACKING);
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
            if (rb2d.velocity.y < -0.2 && !pv.isGrounded)
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

        private void BetterJump()
        {
            if (rb2d.velocity.y < 0)
            {
                rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb2d.velocity.y > 0 && !jumpingHeld)
            {
                rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowFallMultiplier - 1) * Time.deltaTime;
            }

        }

        private void CheckGround()
        {
            //Raycast position calculation. (from the center-bottom of the collider 2d, with an offset)
            Vector3 raycastPositionLeft = capsuleCollider.bounds.center - new Vector3(0, capsuleCollider.bounds.extents.y, 0) - raycastLeftOffset;
            Vector3 raycastPositionRight = capsuleCollider.bounds.center - new Vector3(0, capsuleCollider.bounds.extents.y, 0) + raycastRightOffset;
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

        public void StartJumping()
        {
            if (pv.isGrounded) SoundManager.instance.Play("Jump");
            jumpingHeld = true;
        }

        public void CancelJumping()
        {
            jumpingHeld = false;
            pv.isJumping = false;
        }

        public void JumpUpdate()
        {
            if (pv.isGrounded && !jumpingHeld)
            {
                pv.isJumping = false;
            }
            else if (pv.isGrounded && jumpingHeld)
            {
                pv.isJumping = true;
            }

            if (pv.isJumping)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce * (Time.deltaTime * 50));
            }

        }

        private void Move()
        {
            //if(movementDirection.x != 0)
            //{
            //    Debug.Log(movementDirection.x);
            //}
            rb2d.velocity = new Vector2(movementDirection.x * speed * (Time.deltaTime * 50), rb2d.velocity.y);
        }

        private void footstep()
        {
            footsteps.Play();
        }
    }//end class
}//end namespace

