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
        private static KinectInputModel _kinectInputModel; 
        private static PlayerModel _playerModel;
        private static PlayerController _playerController;
        private static KinectInputController _kinectInputController;

        public static FAKEINPUTCONTROLLER FakeInputController;

        private static bool VERBOSE = true;
        

        // All Controll Classes
        private static readonly HashSet<IController> AllControllers = new HashSet<IController>();

        public void Start () {

            // Initialize everything here - because we need transform component
            _kinectInputModel = new KinectInputModel(KinectInputModel.KINECTMOVE.KINNECTMOVE_IDLE);
            _playerModel = new PlayerModel(PlayerModel.STATE.STATE_STANDING, PlayerModel.LANE.LANE_MIDDLE, GameObject.Find("player").transform); // TODO: REPLACE WITH REAL PLAYER
            _playerController = new PlayerController(_playerModel, _kinectInputModel);
            _kinectInputController = new KinectInputController(_kinectInputModel, _playerController);

            // Register all Controller
            AllControllers.Add(_playerController);
            AllControllers.Add(_kinectInputController);


            // Register fake input controller
            FakeInputController = GetComponent<FAKEINPUTCONTROLLER>();
            Debug.Log(FakeInputController);
            FakeInputController.SetInputController(_kinectInputController, _kinectInputModel);
        }
	

        private void Update () {
        
            // calls all update() methods from all classes that implement the IController Interface
            foreach (var controller in AllControllers)
            {
                controller.Update();
            }


            if (VERBOSE)
            {
                PrintVerbose();
            }
        }

        private void PrintVerbose()
        {
            // VERBOSE
           Debug.Log("KinectInputModel State: " + _kinectInputModel.Kinectmove);
           Debug.Log("PlayerModel State: " + _playerModel.CurrentPlayerState);
           Debug.Log("PlayerModel Lane: " + _playerModel.CurrentPlayerLane);
           Debug.Log("isJumping: " + PlayerController.IsJumping);
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
