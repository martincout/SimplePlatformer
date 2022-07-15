using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Assets.Scripts.Player.States
{
    public class PlayerAirState : PlayerBaseState
    {
        public PlayerAirState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory)
        {
        }

        public override bool CheckSwitchStates()
        {
            return true;
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
    }
}