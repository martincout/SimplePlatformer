using UnityEngine;

namespace SimplePlatformer.Assets.Scripts.Player.States
{
    internal class PlayerGroundedState : PlayerBaseState
    {
        // Raycast to check if it's grounded
        private RaycastHit2D raycastLeft;
        private RaycastHit2D raycastRight;
        public PlayerGroundedState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory)
        {
        }

        public override bool CheckSwitchStates()
        {
            //If the player is not grounded and the jump is pressed, switch to jump state
            if (!CheckGround() && _ctx.IsJumping)
            {
                SwitchState(_factory.Jump());
                return true;
            }
            //If the player is not grounded and jump is not pressed, switch to fall state
            if (!CheckGround() && !_ctx.IsJumping)
            {
                SwitchState(_factory.Jump());
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
        }

        public override void ExitState()
        {
        }

        public override void InitializeSubState()
        {
        }

        public override void UpdateState()
        {
        }

        private bool CheckGround()
        {
            //Raycast position calculation. (from the center-bottom of the collider 2d, with an offset)
            Vector3 raycastPositionLeft = _ctx.BoxCollider.bounds.center - new Vector3(0, _ctx.BoxCollider.bounds.extents.y, 0) - _ctx.RaycastLeftOffset;
            Vector3 raycastPositionRight = _ctx.BoxCollider.bounds.center - new Vector3(0, _ctx.BoxCollider.bounds.extents.y, 0) + _ctx.RaycastRightOffset;
            //Raycast2d
            raycastLeft = Physics2D.Raycast(raycastPositionLeft, Vector2.down, _ctx.GroundedHeight, _ctx.GroundLayer);
            raycastRight = Physics2D.Raycast(raycastPositionRight, Vector2.down, _ctx.GroundedHeight, _ctx.GroundLayer);

            Color color;
            //Check if grounded
            if (raycastLeft.collider != null || raycastRight.collider != null)
            {
                return true;
                //Grounded
                color = Color.green;
                Debug.DrawRay(raycastPositionLeft, Vector2.down * _ctx.GroundedHeight, color);
                Debug.DrawRay(raycastPositionRight, Vector2.down * _ctx.GroundedHeight, color);
            }
            else
            {
                return false;
                //Not Grounded
                color = Color.red;
                Debug.DrawRay(raycastPositionLeft, Vector2.down * _ctx.GroundedHeight, color);
                Debug.DrawRay(raycastPositionRight, Vector2.down * _ctx.GroundedHeight, color);
            }

        }
    }
}
