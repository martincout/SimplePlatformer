using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimplePlatformer.Assets.Scripts.Player.States
{
    public abstract class PlayerBaseState
    {
        protected PlayerStateMachine _ctx;
        protected PlayerStateFactory _factory;

        public PlayerBaseState(PlayerStateMachine ctx, PlayerStateFactory factory)
        {
            _ctx = ctx;
            _factory = factory;
        }


        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract bool CheckSwitchStates();
        public abstract void InitializeSubState();

        public void UpdateStates() {
            //If it is true that is switching states, return
            if (CheckSwitchStates()) return;
        }
        public void SwitchState(PlayerBaseState newState) {
            //Current state exists state
            ExitState();

            //new state enters state
            newState.EnterState();

            _ctx.CurrentState = newState;
        }
        void SetSuperState() { }
        void SetSubState() { }

    }
}