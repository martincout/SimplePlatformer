using UnityEngine;
using UnityEngine.InputSystem;

namespace SimplePlatformer.Assets.Scripts.Player.States
{
    public class PlayerStateMachine : MonoBehaviour, IPlayer
    {
        public PlayerData playerData;
        private Vector2 rawInputMovement;
        private Animator anim;
        private Rigidbody2D _rb;
        private Renderer render;
        public float thrust = 10f;
        private string currentControlScheme;
        public Vector2 WorldPosition { get => this.transform.position; }
        // Current State -------------------------------------------------------------------------------------
        public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        private PlayerBaseState _currentState;
        // Grounded State ------------------------------------------------------------------------------------
        public BoxCollider2D BoxCollider { get => _boxCollider; set => _boxCollider = value; }
        private PlayerStateFactory _stateFactory;
        private BoxCollider2D _boxCollider;
        public Vector3 RaycastLeftOffset { get => playerData.RayCastLeftOffset; }
        public Vector3 RaycastRightOffset { get => playerData.RaycastRightOffset; }
        public float GroundedHeight { get => playerData.GroundedHeight; }
        public int GroundLayer { get => playerData.GroundLayer; }

        // Jump State ----------------------------------------------------------------------------------------
        public bool IsJumping { get; set; }

        // Movement Logic ------------------------------------------------------------------------------------
        private Vector2 MovementDirection { get; set; }
        private float Speed { get; set; }
        private float MaxSpeed { get => playerData.MaxSpeed; }
        private float Acceleration { get => playerData.Acceleration; }
        private float Decelaration { get => playerData.Decelaration; }
        

        private void Awake()
        {
            //State Machine
            _stateFactory = new PlayerStateFactory(this);
            _currentState = _stateFactory.Grounded();
            _currentState.EnterState();
            //Colliders
            _boxCollider = GetComponent<BoxCollider2D>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _currentState.UpdateState();
            // Applied movement
            Move();
            Debug.Log(Speed);
        }

        #region Input
        //INPUT SYSTEM ACTION METHODS --------------

        //This is called from PlayerInput; when a joystick or arrow keys has been pushed.
        //It stores the input Vector as a Vector3 to then be used by the smoothing function.


        public void OnMovement(InputAction.CallbackContext value)
        {
            Vector2 inputMovement = value.ReadValue<Vector2>();
            rawInputMovement = new Vector2(inputMovement.x, _rb.velocity.y);
        }

        //This is called from PlayerInput, when a button has been pushed, that corresponds with the 'Attack' action
        public void OnAttack(InputAction.CallbackContext value)
        {
            if (value.started)
            {
            }
        }

        //This has a hold Interaction. When the hold ends, starts falling
        public void OnJump(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                IsJumping = true;
            }
            if (value.performed || value.canceled)
            {
                IsJumping = false;
            }

        }

        public void OnInteract(InputAction.CallbackContext value)
        {
            if (value.started)
            {
            }
        }

        public void OnBowAttack(InputAction.CallbackContext value)
        {
            if (value.started)
            {
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
        #endregion

        private void Move()
        {
            CalculateSpeed();
            UpdateMovement(rawInputMovement);
            _rb.velocity = new Vector2(MovementDirection.x * Speed, _rb.velocity.y);
        }

        public void UpdateMovement(Vector2 newMovementDirection)
        {
            MovementDirection = newMovementDirection;
        }

        /// <summary>
        /// Gradually increments and decreases the speed
        /// </summary>
        private void CalculateSpeed()
        {
            if ((Speed < MaxSpeed) && MovementDirection.x != 0)
            {
                Speed += Acceleration * Time.deltaTime;
            }
            else
            {
                if (Speed > Decelaration * Time.deltaTime)
                {
                    Speed -= Decelaration * Time.deltaTime;
                }
                else
                {
                    Speed = 0;
                }
            }
        }
    }
}