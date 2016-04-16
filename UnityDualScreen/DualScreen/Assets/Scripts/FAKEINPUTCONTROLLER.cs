using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts
{
    public class FAKEINPUTCONTROLLER : MonoBehaviour
    {
        private static KinectInputController _kinectInputController;
        private static KinectInputModel _kinectInputModel;


        public void SetInputController(KinectInputController kinectInputController, KinectInputModel kinectInputModel)
        {
            _kinectInputController = kinectInputController;
            _kinectInputModel = kinectInputModel;
        }

        // Use this for initialization
        private void Start ()
        {
        }
	
        // Update is called once per frame
        private void Update ()
        {
    
            if (Input.GetKey(KeyCode.Space) && _kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINNECTMOVE_JUMPING)
            {
                PlayerController.IsJumping = true;
                _kinectInputController.HandleKinectInput("jump");
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) && _kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINNECTMOVE_DUCKING && _kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINNECTMOVE_JUMPING)
            {
                _kinectInputController.HandleKinectInput("duck");
            }

            if (Input.GetKeyUp(KeyCode.LeftControl) && _kinectInputModel.Kinectmove == KinectInputModel.KINECTMOVE.KINNECTMOVE_DUCKING)
            {
                _kinectInputController.HandleKinectInput("idle");
            }

            if (Input.GetKey(KeyCode.A) && _kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINNECTMOVE_JUMPING)
            {
                _kinectInputController.HandleKinectInput("left");
            }

            if (Input.GetKey(KeyCode.D) && _kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINNECTMOVE_JUMPING)
            {
                _kinectInputController.HandleKinectInput("right");
               
            }
        }
    }
}
