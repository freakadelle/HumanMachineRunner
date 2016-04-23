namespace Assets.Scripts
{
    public static class AvatarStateMachine
    {
        public enum AvatarMove
        {
            Idle = 0,
            Jumping = 1,
            Ducking = 2
        }

        public enum AvatarLane
        {
            Left,
            Middle,
            Right,
            Idle,
        }

        public static AvatarMove AvatarMoveState { get; set; }
        public static AvatarLane AvatarLaneState { get; set; }
    }
}
