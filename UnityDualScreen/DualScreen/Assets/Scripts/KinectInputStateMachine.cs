namespace Assets.Scripts
{
    public static class KinectInputStateMachine
    {
        public enum KINECTMOVE
        {
            KINNECTMOVE_IDLE = 0,
            KINNECTMOVE_JUMPING = 1,
            KINNECTMOVE_DUCKING = 2
        }

        public enum AvatarLane
        {
            LANELEFT = -10,
            LANEMIDDLE = 0,
            LANERIGHT = +10,
        }

        public static KINECTMOVE KinectMoveState { get; set; }
    }
}
