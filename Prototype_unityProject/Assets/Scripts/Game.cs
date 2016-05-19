using System;
using UnityEngine;
using System.IO;
using Assets.Scripts;

public class Game : MonoBehaviour {

    //Serialize Fields
    [SerializeField(), Range(0,10)]
    public int secondsUntilGameStart;

    //GameStates
    public bool isRunning;
    public bool isPaused;
    private bool isPreparing = false;
    private bool isInitialized = false;
    
    //Timer variables
    private DateTime startTime;
    private DateTime startPrepareTime;
    private DateTime actTime;

    public HUD_View view;

    private KinectInputController kinectController;
    public AvatarController avatarController;

    private Player player;

    
    //--------------------------------
    //MONOBEHAVIOUR IMPLEMENT METHODS
    //--------------------------------

    void OnApplicationQuit()
    {
        kinectController.Stop();
    }

    void Awake()
    {
        kinectController = new KinectInputController(Path.Combine(Application.dataPath, "data"));

        //Build some hardcoded Gestures
        kinectController.addHardCodeGesture(new JumpGesture());
        kinectController.addHardCodeGesture(new CrouchGesture());

        //Add gestures from KineticSpace per file
        kinectController.AddGesture("bend_right");
        kinectController.AddGesture("bend_left");

        //KinectController Events
        kinectController.KinectStateChanged += OnKinectStateChanged;
        kinectController.KinectStateUpdate += OnKinectStateUpdate;

        kinectController.Start();

        avatarController.enabled = false;

        Debug.Log("-- GAME PRE-INITIALIZE --");
    }

    void Start()
    {
        view.warningView.enabled = true;
        view.elapsedTimeView.enabled = false;
        view.highscoreView.enabled = false;

        view.warningView.text = "Waiting for player!";

        player = new Player();

        isInitialized = true;

        Debug.Log("-- GAME INITIALIZED --");
    }

    void Update()
    {
        kinectController.update();

        //Handle Game States
        actTime = DateTime.Now;

        if (isRunning)
        {
            TimeSpan t = actTime - startTime;
            player.score(13);

            view.elapsedTimeView.text = t.Seconds.ToString("##") + ":" + t.Milliseconds.ToString("##");
            view.highscoreView.text = player.highscore + " Pt.";
        }
        else if (isPreparing)
        {
            int t = secondsUntilGameStart - (actTime.Second - startPrepareTime.Second);

            if (t <= 0)
            {
                view.warningView.enabled = false;
                view.elapsedTimeView.enabled = true;
                view.highscoreView.enabled = true;
                isPreparing = false;
                isRunning = true;

                startTime = DateTime.Now.AddSeconds(-1);
            }
            else
            {
                view.warningView.text = t.ToString();
            }
        }
    }


    //---------------------------
    //KINECT INPUT EVENT LISTENER
    //---------------------------


    //Handle different Kinect states once per change State
    private void OnKinectStateChanged(object sender, MyEvArgs<bodiesState> _data)
    {
        bodiesState state = _data.data;

        if (state == bodiesState.NO_ACTIVE_SOURCE)
        {
            //reset();
            if (isRunning)
            {
                pause(true);
            }
        }
        else if (state == bodiesState.SINGLE_SOURCE)
        {
            run();
        }
        else if (state == bodiesState.MULTIPLE_SOURCES)
        {
            pause(true);
        }
        else if (state == bodiesState.INITIALIZE_SOURCE)
        {
            pause(true);
        }
    }

    //Handle different Kinect states every frame
    private void OnKinectStateUpdate(object sender, MyEvArgs<bodiesState> _data)
    {
        bodiesState state = _data.data;

        if (state == bodiesState.NO_ACTIVE_SOURCE)
        {
            //Every frame there is no body found, search for a new detected bodies
            kinectController.bodiesManager.nextPossibleBody();
        }
        else if (state == bodiesState.SINGLE_SOURCE)
        {
            //If single source detected, than handle input from that source
            kinectController.handleKineticSpaceGestures();
            kinectController.handleHardCodeGestures();
        }
        else if (state == bodiesState.MULTIPLE_SOURCES)
        {
            //Todo: Was passiert bei multiple Sources
        }
        else if (state == bodiesState.INITIALIZE_SOURCE)
        {
            //If body detected, initialize player norms
            kinectController.bodiesManager.initializeBody();
        }
    }


    //---------------------
    //GAME CONTROL METHODS
    //---------------------

    private void prepare(bool _value)
    {
        isPreparing = _value;
        startPrepareTime = DateTime.Now;
    }

    public void pause(bool _value, bool _prepare = true)
    {
        avatarController.enabled = false;
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
            avatarController.enabled = true;
            Debug.Log("-- GAME STARTED --");
        }
        else
        {
            Debug.Log("-- GAME UNABLE TO START. NOT YET INITIALIZED --");
        }
    }

    public void stop()
    {
        startTime = new DateTime();
        actTime = new DateTime();
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
