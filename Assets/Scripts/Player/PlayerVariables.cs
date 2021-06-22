
namespace SimplePlatformer.Player
{
    public class PlayerVariables
    {
        //Animation Const
        public static readonly string PLAYER_IDLE = "playerIdle";
        public static readonly string PLAYER_WALK = "playerWalk";
        public static readonly string PLAYER_JUMP = "playerJump";
        public static readonly string PLAYER_FALLING = "playerFalling";
        public static readonly string PLAYER_ATTACKING = "playerAttack";
        public static readonly string PLAYER_HURT = "playerHurt";
        public static readonly string PLAYER_INVINCIBLE = "playerInvincible";
        public static readonly string PLAYER_AIRATTACK = "playerAirAttack";
        public static readonly string PLAYER_COMBO = "playerAttack2";
        //States
        public bool isJumping = false;
        public bool movePrevent;
        public bool isFacingRight;
        public bool isAttacking;
        public bool isStunned;
        public bool itsDying;
        public bool invincible;
        public bool airAttacked;
        public bool cannotAttack;
        public bool isGrounded;
    }
}

