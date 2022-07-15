namespace SimplePlatformer.Assets.Scripts.Player.States
{
    public class PlayerStateFactory
    {
        PlayerStateMachine _context;
        public PlayerStateFactory(PlayerStateMachine currentContext)
        {
            _context = currentContext;
        }

        public PlayerBaseState Idle() => new PlayerIdleState(_context,this);
        public PlayerBaseState Run() => new PlayerRunState(_context,this);
        public PlayerBaseState Jump() => new PlayerJumpState(_context,this);
        public PlayerBaseState Attack() => new PlayerAttackState(_context,this);
        public PlayerBaseState Grounded() => new PlayerGroundedState(_context,this);
        public PlayerBaseState Fall() => new PlayerGroundedState(_context,this);
        public PlayerBaseState Air() => new PlayerGroundedState(_context,this);
    }
}
