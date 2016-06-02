namespace Assets.Scripts
{
    public static class AvatarStateMachine
    {
        public enum AvatarMove
        {
            Idle = 0,
            Jumping = 1,
            Ducking = 2,
            Left = 3,
            Right = 4
        }

        public static AvatarMove AvatarMoveState { get; set; }
    }
}
