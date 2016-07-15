using System;
using System.Collections;
using System.IO;
using Assets.Scripts.Kinect;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField, Range(0, 10)]
        public int SecondsUntilGameStart;

        //GameStates
        public enum GameState
        {
            Running,
            Paused,
            Preparing,
            Initialized,
            Stopped,
            Lost,
            Won,
            MultipleSources
        }

        public static GameState _gameState;
        private static KinectInputController _kinectController;
        private static bool _boolHelperSwitch;
        private bool _kinectConnected = false;

        public AvatarController AvatarController;

        public static IUpdate ComponentWhereUpdateCallShouldBeExecuted;
        public static string GetHighScore { get; set; }


        public void OnApplicationQuit()
        {
            _kinectController.Stop();
        }

        public void Awake()
        {
            _kinectController = new KinectInputController(Path.Combine(Application.dataPath, "data"));

            // Coded gestures
            _kinectController.AddHardCodeGesture(new JumpGesture());
            _kinectController.AddHardCodeGesture(new CrouchGesture());

            // Gestures from KineticSpace via file
            _kinectController.AddGesture("bend_right");
            _kinectController.AddGesture("bend_left");

            try
            {
                _kinectController.Start();
                _kinectConnected = true;
            }
            catch (DllNotFoundException)
            {
                Debug.LogError("Kinect disconnected!");
            }


            Debug.Log("-- GAME PRE-INITIALIZE --");
        }

        public void Start()
        {
            _gameState = GameState.Initialized;

            Debug.Log("-- GAME INITIALIZED --");
        }

        public void Update()
        {
            // Handle kinect
            _kinectController.Update();
            //Handle Game States
            UpdateGameRelativeToGameState();

            if (_kinectConnected)
            {
                // Handle Interface
                ComponentWhereUpdateCallShouldBeExecuted.ExternalUpdateMethod();
            }
        }


        /// <summary>
        /// Returns switched Bool after given timme
        /// </summary>
        /// <param name="boolToBeSwitched"></param>
        /// <param name="timeToWait"></param>
        public void ReturnBoolSwitchedAfterWaiting(ref bool boolToBeSwitched, float timeToWait)
        {
            _boolHelperSwitch = boolToBeSwitched;
            StartCoroutine(SwitchBoolAfterTime(boolToBeSwitched, timeToWait));
        }

        private IEnumerator SwitchBoolAfterTime(bool boolToBeSwitched, float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            _boolHelperSwitch = !boolToBeSwitched;
        }

        private void UpdateGameRelativeToGameState()
        {
            switch (_gameState)
            {
                case GameState.Running:
                    UnPause();
                    break;
                case GameState.Preparing:
                    StartCoroutine(StartGameWithCountDown(SecondsUntilGameStart));
                    break;
                case GameState.Paused:
                    Pause();
                    break;
                case GameState.Initialized:
                    // Pause();
                    break;
                case GameState.Stopped:
                    break;
                case GameState.Lost:
                    SceneManager.LoadScene(1);
                    break;
                case GameState.Won:
                    break;
                case GameState.MultipleSources:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private static void Pause()
        {
            Time.timeScale = 0;
        }

        private static void UnPause()
        {
            Time.timeScale = 1;
        }

        private static IEnumerator StartGameWithCountDown(int countDownTime)
        {
            yield return new WaitForSeconds(countDownTime);
            // Start Game
            _gameState = GameState.Running;
        }

        #region KinectEventSystem

        /// <summary>
        /// Event System Class
        /// </summary>
        public class KinectUpdateEvent : GameEvent
        {
            public BodiesState BodieState { get; private set; }

            public KinectUpdateEvent(BodiesState bodieState)
            {
                OnEnable();
                BodieState = bodieState;
            }

            protected virtual void OnEnable()
            {

                Events.instance.AddListener<KinectUpdateEvent>(OnStateChanged);
                Events.instance.AddListener<KinectEvent>(OnStateChanged);
            }

            protected virtual void OnDisable()
            {
                Events.instance.RemoveListener<KinectUpdateEvent>(OnStateChanged);
                Events.instance.RemoveListener<KinectEvent>(OnStateChanged);
            }

            private static void OnStateChanged(KinectUpdateEvent e)
            {
                // Handle event here
                // Debug.Log("BodySourceManager Update changed: " + e.BodieState);
                OnKinectStateUpdate(e.BodieState);
            }

            private static void OnStateChanged(KinectEvent e)
            {
                // Handle event here
                Debug.Log("BodySourceManager State changed: " + e.BodieState);
                OnKinectStateChanged(e.BodieState);
            }
        }

        public class KinectEvent : GameEvent
        {
            public BodiesState BodieState { get; private set; }

            public KinectEvent(BodiesState bodieState)
            {
                BodieState = bodieState;
            }
        }

        private static void OnKinectStateChanged(BodiesState bodiesState)
        {
            switch (bodiesState)
            {
                case BodiesState.NO_ACTIVE_SOURCE:
                    if (_gameState == GameState.Running)
                        _gameState = GameState.Paused;
                    break;
                case BodiesState.SINGLE_SOURCE:
                    _gameState = GameState.Running;
                    break;
                case BodiesState.MULTIPLE_SOURCES:
                    _gameState = GameState.MultipleSources;
                    break;
                case BodiesState.INITIALIZE_SOURCE:
                    _gameState = GameState.Paused;
                    break;
                case BodiesState.NO_DATA:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void OnKinectStateUpdate(BodiesState bodiesState)
        {

            switch (bodiesState)
            {
                case BodiesState.NO_ACTIVE_SOURCE:
                    // Search for body
                    KinectInputController.BodiesManager.NextPossibleBody();
                    break;
                case BodiesState.SINGLE_SOURCE:
                    // Handle body
                    _kinectController.HandleKineticSpaceGestures();
                    _kinectController.HandleHardCodeGestures();
                    break;
                case BodiesState.MULTIPLE_SOURCES:
                    //TODO: Implement
                    break;
                case BodiesState.INITIALIZE_SOURCE:
                    // Initialize
                    KinectInputController.BodiesManager.InitializeBody();
                    break;
                case BodiesState.NO_DATA:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
