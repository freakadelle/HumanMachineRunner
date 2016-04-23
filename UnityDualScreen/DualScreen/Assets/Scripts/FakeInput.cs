using UnityEngine;

namespace Assets.Scripts
{
    public class FakeInput : MonoBehaviour
    {
       
        // Update is called once per frame
        public void Update ()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Jumping;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Ducking;
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                switch (AvatarStateMachine.AvatarLaneState)
                {
                    case AvatarStateMachine.AvatarLane.Middle:
                        AvatarStateMachine.AvatarLaneState = AvatarStateMachine.AvatarLane.Left;
                        break;
                        case AvatarStateMachine.AvatarLane.Right:
                        AvatarStateMachine.AvatarLaneState = AvatarStateMachine.AvatarLane.Middle;
                        break;
                }
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                switch (AvatarStateMachine.AvatarLaneState)
                {
                    case AvatarStateMachine.AvatarLane.Middle:
                        AvatarStateMachine.AvatarLaneState = AvatarStateMachine.AvatarLane.Right;
                        break;
                    case AvatarStateMachine.AvatarLane.Left:
                        AvatarStateMachine.AvatarLaneState = AvatarStateMachine.AvatarLane.Middle;
                        break;
                }
            }
        }
    }
}
