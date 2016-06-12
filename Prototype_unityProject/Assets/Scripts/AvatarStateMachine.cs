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


        public enum AvatarRotation
        {
            Zero,
            FortyFive,
            Ninety,
            OneHundredThirtyFive,
            OneHundredEighty,
            TwoHundredTwentyFive,
            TwoHundredSeventy,
            ThreeHundredFifteen
        }

        public static AvatarRotation AvatarRotationState { get; set; }

        public static AvatarMove AvatarMoveState { get; set; }
    }
}
