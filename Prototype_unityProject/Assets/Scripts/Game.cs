﻿using System;
using System.IO;
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

        private KinectInputController _kinectController;
        public AvatarController AvatarController;

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
            _kinectController.addHardCodeGesture(new JumpGesture());
            _kinectController.addHardCodeGesture(new CrouchGesture());

            // Gestures from KineticSpace via file
            _kinectController.AddGesture("bend_right");
            _kinectController.AddGesture("bend_left");

            //KinectController Events
            _kinectController.KinectStateChanged += OnKinectStateChanged;
            _kinectController.KinectStateUpdate += OnKinectStateUpdate;

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
            _kinectController.update();

            //Handle Game States
            _actTime = DateTime.Now;

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

        //---------------------------
        //KINECT INPUT EVENT LISTENER
        //---------------------------


        //Handle different Kinect states once per change State
        private void OnKinectStateChanged(object sender, MyEvArgs<bodiesState> data)
        {
            switch (data.data)
            {
                case bodiesState.NO_ACTIVE_SOURCE:
                    if (_gameState == GameState.Running)
                        _gameState = GameState.Paused;
                    break;
                case bodiesState.SINGLE_SOURCE:
                    _gameState = GameState.Running;
                    break;
                case bodiesState.MULTIPLE_SOURCES:
                    _gameState = GameState.Paused;
                    break;
                case bodiesState.INITIALIZE_SOURCE:
                    _gameState = GameState.Paused;
                    break;
                case bodiesState.NO_DATA:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //Handle different Kinect states every frame
        private void OnKinectStateUpdate(object sender, MyEvArgs<bodiesState> data)
        {
            // TODO: Replace with switch
            switch (data.data)
            {
                case bodiesState.NO_ACTIVE_SOURCE:
                    //Every frame there is no body found, search for a new detected bodies
                    _kinectController.bodiesManager.nextPossibleBody();
                    break;
                case bodiesState.SINGLE_SOURCE:
                    //If single source detected, than handle input from that source
                    _kinectController.handleKineticSpaceGestures();
                    _kinectController.handleHardCodeGestures();
                    break;
                case bodiesState.MULTIPLE_SOURCES:
                    //Todo: Was passiert bei multiple Sources
                    break;
                case bodiesState.INITIALIZE_SOURCE:
                    //If body detected, initialize player norms
                    _kinectController.bodiesManager.initializeBody();
                    break;
                case bodiesState.NO_DATA:
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
