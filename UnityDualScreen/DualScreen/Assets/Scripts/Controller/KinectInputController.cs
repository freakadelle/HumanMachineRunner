using System;
using System.Diagnostics;
using Assets.Scripts.Model;

namespace Assets.Scripts.Controller
{
    public class KinectInputController : IController {

        private  KinectInputModel _kinectInputModel;
        private  PlayerController _playerController;

        public KinectInputController(KinectInputModel kinectInputModel, PlayerController playerController)
        {
            _kinectInputModel = kinectInputModel;
            _playerController = playerController;
        }

        public void HandleKinectInput(string kinectInput)
        {
            switch (kinectInput)
            {
                default:
                    _kinectInputModel.Kinectmove= KinectInputModel.KINECTMOVE.KINNECTMOVE_IDLE;
                    break;
                case "jump":
                    if (_kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINNECTMOVE_JUMPING)
                        _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINNECTMOVE_JUMPING;
                    break;
                case "duck":
                    if (_kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINNECTMOVE_DUCKING)
                        _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINNECTMOVE_DUCKING;
                    break;
                case "left":
                    if (_kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINNECTMOVE_LANELEFT) 
                        _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINNECTMOVE_LANELEFT;
                    break;
                case "right":
                    if (_kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINECTMOVE_LANERIGHT)
                        _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINECTMOVE_LANERIGHT;
                    break;
            }
        }

        // From Interface
        public void Update()
        {
            // needed - if HandleKinectInput() is calling HandleInput() if KinctMove State changes it breaks - lags :(
            _playerController.HandleInput();
        }
    }
}
