using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Windows.Kinect;
using Assets.Scripts;
using KineticSpace.Kinect;

using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class KinectInputController : MonoBehaviour
{

    public Transform playerTransform;
    public AvatarController avatarController;

    private DefaultKinectGestureEvaluator _evaluator;
    //private Dictionary<JointType, Windows.Kinect.Joint> source; 
    private BodiesManager _bm;
    private List<MyGesture> myGestures;


    void OnApplicationQuit()
    {
        this._evaluator.Stop();
    }

    void Awake()
    {
        avatarController.enabled = false;

        QualitySettings.vSyncCount = 0;

        var dataPath = Path.Combine(Application.dataPath, "data");

        _bm = new BodiesManager();
        _bm.StateChanged += OnBodyStateChange;

        //Build some hardcoded Gestures
        myGestures = new List<MyGesture>();
        myGestures.Add(new JumpGesture());
        myGestures.Add(new CrouchGesture());

        //Add gestures from KineticSpace per file
        this._evaluator = new DefaultKinectGestureEvaluator(dataPath);
        this._evaluator.AddGesture("bend_right");
        this._evaluator.AddGesture("bend_left");
        this._evaluator.Start();
    }

    void Start()
    {
        //Game.init();
    }

    void Update()
    {
        
        var bodies = _evaluator._kinectSource.LastBodies;
        //var temp = _evaluator._kinectSource.Sensor.BodyFrameSource.BodyCount;

        waitForBodySource();

        //Todo: Die Referenz muss immer erneut mitgegeben werden. Weshalb kann er die Referenz beim Konstruktoraufruf nich benutzen.
        _bm.updateStates(bodies);

        //Handle different body states every frame
        if (_bm.State == bodiesState.NO_ACTIVE_SOURCE)
        {
            _bm.nextPossibleBody();
        } else if (_bm.State == bodiesState.SINGLE_SOURCE)
        {
            handleGestureInput();
        }
        else if(_bm.State == bodiesState.MULTIPLE_SOURCES)
        {
            //Todo: Was passiert bei multiple Sources
        } else if (_bm.State == bodiesState.INITIALIZE_SOURCE)
        {
            _bm.initializeBody();
        }

        Game.update();
    }

    //Implemented Methods
    //------------------------------------------------------------------------------------------------------

    private void OnBodyStateChange(object sender, MyEvArgs<bodiesState> _data)
    {
        Debug.Log("BodySourceManager State changed: " + _data.data);

        //Handle different body states once per change State
        if (_bm.State == bodiesState.NO_ACTIVE_SOURCE)
        {
            avatarController.enabled = false;
            _bm.reset();
            if (Game.isRunning)
            {
                Game.pause(true);
            }
        }
        else if (_bm.State == bodiesState.SINGLE_SOURCE)
        {
            avatarController.enabled = true;
            Game.start();
        }
        else if (_bm.State == bodiesState.MULTIPLE_SOURCES)
        {
            avatarController.enabled = false;
            Game.pause(true);
        }
        else if (_bm.State == bodiesState.INITIALIZE_SOURCE)
        {
            avatarController.enabled = false;
        }
    }

    private void waitForBodySource()
    {
        var bodies = _evaluator._kinectSource.LastBodies;

        if ( bodies != null)
        {
            //Falls Sensordaten vorhanden. Lege einen neuen BodyManager mit den SensorDaten der Bodies an.
            if (_bm.State == bodiesState.NO_DATA)
            {
                _bm.init(bodies);
            }
        }
    }

    private void handleGestureInput()
    {
        handleHardCodeGestures();
        handleKineticSpaceGestures();
    }

    private void handleKineticSpaceGestures()
    {
        //temporarily saves detected gestures
        var gestures = this._evaluator.DetectedGestures;

        if (gestures.Count > 0)
        {
            Debug.Log(gestures[0]);
        }

        //Clear gestures list
        this._evaluator.SignalDetectedGesturesProcessed();
    }

    private void handleHardCodeGestures()
    {
        //Detect and validate hardcode gestures from Gesturelist
        for (int i = 0; i < myGestures.Count; i++)
        {
            //Debug.Log(myGestures[i].MinInterval);
            if (myGestures[i].validate(_bm.ActiveSource, _bm.referenceSource) && myGestures[i].MinInterval <= 0)
            {
                myGestures[i].trigger();

                
            }
        }

        Vector3 tempPos = playerTransform.localPosition;
        tempPos.z += 0.01f;
        tempPos.x = _bm.getActJointPos(JointType.SpineMid).X * 2;
        tempPos.y = (_bm.getActJointPos(JointType.Neck).Y * 4) - 1;

        playerTransform.localPosition = tempPos;

    }
}
