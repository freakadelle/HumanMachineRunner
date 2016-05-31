using System;
using System.IO;
using Assets.Scripts.Kinect;
using UnityEngine;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour
    {
        //Serialize Fields
        [SerializeField, Range(0, 10)] public int SecondsUntilGameStart;

        //GameStates
        private enum GameState
        {
            Running,
            Paused,
            Preparing,
            Initialized,
            Stopped,
            Lost,
            Won
        }

        private static GameState _gameState;

        //Timer variables
        public DateTime StartTime { get; private set; }
        private DateTime _startPrepareTime = new DateTime();
        private DateTime _actTime;

        public HUD_View View;

        private static KinectInputController _kinectController;
        public AvatarController AvatarController;

#region KinectEventSystem
        /// <summary>
        /// Event System Class
        /// </summary>
        public class KinectUpdateEvent : GameEvent
        {
            public BodiesState BodieState { get; private set; }

            public KinectUpdateEvent(BodiesState bodieState)
            {
                BodieState = bodieState;
            }

            public class KinectEvent : GameEvent
            {
                public BodiesState BodieState { get; private set; }

                public KinectEvent(BodiesState bodieState)
                {
                    BodieState = bodieState;
                }
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
                Debug.Log("BodySourceManager State changed: " + e.BodieState);
                OnKinectStateUpdate(e.BodieState);
            }

            private static void OnStateChanged(KinectEvent e)
            {
                // Handle event here
                Debug.Log("BodySourceManager State changed: " + e.BodieState);
                OnKinectStateChanged(e.BodieState);
            }
        }
#endregion



        //--------------------------------
        //MONOBEHAVIOUR IMPLEMENT METHODS
        //--------------------------------

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

            //KinectController Events
          //  _kinectController.KinectStateChanged += OnKinectStateChanged;
          //  _kinectController.KinectStateUpdate += OnKinectStateUpdate;

            _kinectController.Start();

            AvatarController.enabled = false;

            Debug.Log("-- GAME PRE-INITIALIZE --");
        }

        public void Start()
        {
            View.warningView.enabled = true;
            View.elapsedTimeView.enabled = false;
            View.highscoreView.enabled = false;

            View.warningView.text = "Waiting for player!";

            _gameState = GameState.Initialized;

            Debug.Log("-- GAME INITIALIZED --");
        }

        public void Update()
        {
            _kinectController.Update();

            //Handle Game States
            _actTime = DateTime.Now;

            UpdateGameRelativeToGameState();
        }

        private void UpdateGameRelativeToGameState()
        {
            switch (_gameState)
            {
                case GameState.Running:
                    AvatarController.Score = 13;
                    break;
                case GameState.Preparing:
                    var timeUntilGameisPrepared = SecondsUntilGameStart - (_actTime.Second - _startPrepareTime.Second);
                    if (timeUntilGameisPrepared <= 0)
                    {
                        View.warningView.enabled = false;
                        View.elapsedTimeView.enabled = true;
                        View.highscoreView.enabled = true;

                        // Start Game
                        _gameState = GameState.Running;

                        StartTime = DateTime.Now.AddSeconds(-1);
                    }
                    else
                    {
                        View.warningView.text = timeUntilGameisPrepared.ToString();
                    }
                    break;

                case GameState.Paused:
                    break;
                case GameState.Initialized:
                    break;
                case GameState.Stopped:
                    break;
                case GameState.Lost:
                    break;
                case GameState.Won:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
                    _gameState = GameState.Paused;
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
                    //Every frame there is no body found, search for a new detected bodies
                    _kinectController.BodiesManager.NextPossibleBody();
                    break;
                case BodiesState.SINGLE_SOURCE:
                    //If single source detected, than handle input from that source
                    _kinectController.HandleKineticSpaceGestures();
                    _kinectController.HandleHardCodeGestures();
                    break;
                case BodiesState.MULTIPLE_SOURCES:
                    //Todo: Was passiert bei multiple Sources
                    break;
                case BodiesState.INITIALIZE_SOURCE:
                    //If body detected, initialize player norms
                    _kinectController.BodiesManager.InitializeBody();
                    break;
                case BodiesState.NO_DATA:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        //---------------------
        //GAME CONTROL METHODS
        //---------------------

        /*
    private void prepare(bool _value)
    {
        isPreparing = _value;
        startPrepareTime = DateTime.Now;
    }

    public void pause(bool _value, bool _prepare = true)
    {
        AvatarController.enabled = false;
        prepare(_prepare);
        isRunning = !_value && !_prepare;
        //isPaused = _value;
        Debug.Log("-- GAME PAUSED: " + _value + " --");
    }

    public void run(bool _prepare = true)
    {
        if (isPaused)
        {
            pause(false);
            prepare(_prepare);
        }
        else if (!isRunning)
        {
            startTime = DateTime.Now;
            prepare(_prepare);
            isRunning = !_prepare;
            AvatarController.enabled = true;
            Debug.Log("-- GAME STARTED --");
        }
        else
        {
            Debug.Log("-- GAME UNABLE TO START. NOT YET INITIALIZED --");
        }
    }
   
        public void stop()
        {
            _startTime = new DateTime();
            _actTime = new DateTime();
            isRunning = false;
            isInitialized = false;

            Debug.Log("-- GAME STOPPED --");
        }

        public void restart()
        {
            stop();
            //TODO: Method implementation
        }

        public void loose()
        {
            stop();
            Debug.Log("-- GAME LOOSE --");
            //TODO: Method implementation    
        }

        public void win()
        {
            stop();
            Debug.Log("-- GAME WON --");
            //TODO: Method implementation    
        }
    }
     */
    }
}
