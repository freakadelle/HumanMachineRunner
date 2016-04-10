using System;
using System.Diagnostics;
using Assets.Scripts.Model;

namespace Assets.Scripts.Controller
{
    public class KinectInputController : IController {

        private readonly KinectInputModel _kinectInputModel;
        private readonly PlayerController _playerController;

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
                    _playerController.HandleInput();
                    break;
                case "jump":
                    if (_kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINNECTMOVE_JUMPING)
                    {
                        _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINNECTMOVE_JUMPING;
                        _playerController.HandleInput();
                    }
                    break;
                case "duck":
                    if (_kinectInputModel.Kinectmove != KinectInputModel.KINECTMOVE.KINNECTMOVE_DUCKING)
                    {
                        _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINNECTMOVE_DUCKING;
                        _playerController.HandleInput();
                    }
                   
                    break;
                case "left":
                    _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINNECTMOVE_LANELEFT;
                    _playerController.HandleInput();
                    break;
                case "right":
                    _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINECTMOVE_LANERIGHT;
                    _playerController.HandleInput();
                    break;
            }
        }

        // From Interface
        public void Update()
        {
           
        }
    }
}
