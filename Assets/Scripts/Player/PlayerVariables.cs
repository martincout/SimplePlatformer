
namespace SimplePlatformer.Player
{
    public class PlayerVariables
    {
        //Animation Const
        public static readonly string PLAYER_IDLE = "playerIdle";
        public static readonly string PLAYER_WALK = "playerWalk";
        public static readonly string PLAYER_JUMP = "playerJump";
        public static readonly string PLAYER_FALLING = "playerFalling";
        public static readonly string PLAYER_HURT = "playerHurt";
        public static readonly string PLAYER_INVINCIBLE = "playerInvincible";
        public static readonly string PLAYER_AIRATTACK = "playerAirAttack";
        public static readonly string PLAYER_AIRATTACK2 = "playerAirAttack2";
        public static readonly string PLAYER_ATTACK1 = "playerAttack";
        public static readonly string PLAYER_ATTACK2 = "playerAttack2";
        public static readonly string PLAYER_ATTACK3 = "playerAttack3";
        public static readonly string PLAYER_BOW = "playerBow";
        public static readonly string PLAYER_BOWAIR = "playerBowAir";

        //States
        public bool isJumping;
        public bool movePrevent;
        public bool cannotAttack;
        public bool isFacingRight;
        public bool isAttacking;
        public bool isStunned;
        public bool itsDying;
        public bool invincible;
        public bool airAttacked;
        public bool isGrounded;
        public bool canInteract;
        public bool isBowAttacking;
        public string secret;

        public PlayerVariables()
        {
            this.isJumping = false;
            this.cannotAttack = false;
            this.movePrevent = false;
            this.isFacingRight = true;
            this.isAttacking = false;
            this.isStunned = false;
            this.itsDying = false;
            this.invincible = false;
            this.airAttacked = false;
            this.canInteract = true;
        }

        
        
    }
}

