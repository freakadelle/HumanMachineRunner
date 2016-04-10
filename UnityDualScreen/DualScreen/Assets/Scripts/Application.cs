using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class  Application : MonoBehaviour
    {
        // initialize MVC
        private static KinectInputModel _kinectInputModel = new KinectInputModel(KinectInputModel.KINECTMOVE.KINNECTMOVE_IDLE);
        private static PlayerModel _playerModel = new PlayerModel(PlayerModel.STATE.STATE_STANDING, PlayerModel.LANE.LANE_MIDDLE, Vector3.zero);
        private static PlayerController _playerController = new PlayerController(_playerModel, _kinectInputModel);
        private static KinectInputController _kinectInputController = new KinectInputController(_kinectInputModel, _playerController);

        private static bool VERBOSE = true;
        

        // All Controll Classes
        private static readonly HashSet<IController> AllControllers = new HashSet<IController>();

        public void Start () {
            // Register all Controller
            AllControllers.Add(_playerController);
            AllControllers.Add(_kinectInputController);
        }
	

        private void Update () {
        
            // calls all update() methods from all classes that implement the IController Interface
            foreach (var controller in AllControllers)
            {
                controller.Update();
            }


            if (VERBOSE)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    _kinectInputController.HandleKinectInput("left");
                    PrintVerbose();
                }
            }
        }

        private void PrintVerbose()
        {
            // VERBOSE
           Debug.Log("KinectInputModel State:" + _kinectInputModel.Kinectmove.ToString());
           Debug.Log("PlayerModel State:" + _playerModel.CurrentPlayerState.ToString());
           Debug.Log("PlayerModel Lane:" + _playerModel.CurrentPlayerLane.ToString());
        }


        public static PlayerModel GetPlayerModel()
        {
            return _playerModel;
        }
        public static KinectInputModel GetKinectInputModel()
        {
            return _kinectInputModel;
        }

        
    }

}
