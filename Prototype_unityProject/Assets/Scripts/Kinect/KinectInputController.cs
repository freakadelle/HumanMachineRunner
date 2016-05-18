using System.Collections.Generic;
using System.IO;
using Windows.Kinect;
using KineticSpace.Kinect;

using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class KinectInputController : MonoBehaviour
{

    public Transform playerTransform;
    public FirstPersonController playerController;
    public CharacterController playerChar;

    private DefaultKinectGestureEvaluator _evaluator;
    //private Dictionary<JointType, Windows.Kinect.Joint> source; 
    private BodiesManager _bm;
    private List<MyGesture> myGestures; 

    //private Game game;

    void OnApplicationQuit()
    {
        this._evaluator.Stop();
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;

        var dataPath = Path.Combine(Application.dataPath, "data");

        _bm = new BodiesManager();

        //Build some hardcoded Gestures
        myGestures = new List<MyGesture>();

        myGestures.Add(new JumpGesture());
        myGestures.Add(new CrouchGesture());

        //Add gestures from KineticSpace per file
        this._evaluator = new DefaultKinectGestureEvaluator(dataPath);

        this._evaluator.AddGesture("bend_right");
        this._evaluator.AddGesture("bend_left");
        //this._evaluator.AddGesture("test");
        //this._evaluator.AddGesture("test_3");

        this._evaluator.Start();
    }

    void Update()
    {
        
        var bodies = _evaluator._kinectSource.LastBodies;

        waitForBodySource();

        //Todo: Die Referenz muss immer erneut mitgegeben werden. Weshalb kann er die Referenz beim Konstruktoraufruf nich benutzen.
        _bm.updateStates(bodies);

        //Handle different body states
        if (_bm.State == bodiesState.NO_ACTIVE_SOURCE)
        {
            _bm.reset();
            _bm.nextPossibleBody();
        } else if (_bm.State == bodiesState.SINGLE_SOURCE)
        {
            handleHardCodeGestures();
            handleKineticSpaceGestures();
        }
        else if(_bm.State == bodiesState.MULTIPLE_SOURCES)
        {
            //_bm.removeActiveBody();
            //_bm.nextPossibleBody();
            //Todo: Was passiert bei multiple Sources
        } else if (_bm.State == bodiesState.INITIALIZE_SOURCE)
        {
            _bm.initializeBody();
        }
    }

    //Implemented Methods
    //------------------------------------------------------------------------------------------------------

    private void waitForBodySource()
    {
        var bodies = _evaluator._kinectSource.LastBodies;

        if ( bodies != null)
        {
            //Falls Sensordaten vorhanden. Lege einen neuen BodyManager mit den SensorDaten der Bodies an.
            if (_bm.State == bodiesState.NO_DATA)
            {
                _bm = new BodiesManager(bodies);
            }
        }
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
        tempPos.x = _bm.getActJointPos(JointType.SpineMid).X * 2;
        tempPos.y = (_bm.getActJointPos(JointType.Neck).Y * 20f) - 6;

        playerTransform.localPosition = tempPos;

    }
}