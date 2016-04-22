using UnityEngine;

namespace Assets.Scripts
{
    public class FakeInput : MonoBehaviour
    {
       
        // Update is called once per frame
        private void Update ()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                KinectInputStateMachine.KinectMoveState = KinectInputStateMachine.KINECTMOVE.KINNECTMOVE_JUMPING;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                KinectInputStateMachine.KinectMoveState = KinectInputStateMachine.KINECTMOVE.KINNECTMOVE_DUCKING;
            }
            if (Input.GetKey(KeyCode.A) && KinectInputStateMachine.KinectMoveState != KinectInputStateMachine.KINECTMOVE.KINNECTMOVE_LANELEFT)
            {
                if(KinectInputStateMachine.KinectMoveState == KinectInputStateMachine.KINECTMOVE.KINNECTMOVE_LANEMIDDLE)
                    KinectInputStateMachine.KinectMoveState = KinectInputStateMachine.KINECTMOVE.KINNECTMOVE_LANELEFT;
                if(KinectInputStateMachine.KinectMoveState == KinectInputStateMachine.KINECTMOVE.KINECTMOVE_LANERIGHT)
                    KinectInputStateMachine.KinectMoveState = KinectInputStateMachine.KINECTMOVE.KINNECTMOVE_LANEMIDDLE;
            }
            if (Input.GetKey(KeyCode.D) && KinectInputStateMachine.KinectMoveState != KinectInputStateMachine.KINECTMOVE.KINECTMOVE_LANERIGHT)
            {
                if (KinectInputStateMachine.KinectMoveState == KinectInputStateMachine.KINECTMOVE.KINNECTMOVE_LANEMIDDLE)
                    KinectInputStateMachine.KinectMoveState = KinectInputStateMachine.KINECTMOVE.KINECTMOVE_LANERIGHT;
                if (KinectInputStateMachine.KinectMoveState == KinectInputStateMachine.KINECTMOVE.KINNECTMOVE_LANELEFT)
                    KinectInputStateMachine.KinectMoveState = KinectInputStateMachine.KINECTMOVE.KINNECTMOVE_LANEMIDDLE;
            }
        }
    }
}
