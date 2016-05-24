using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using Assets.Scripts;

public enum bodiesState
{
    NO_DATA,
    INITIALIZE_SOURCE,
    SINGLE_SOURCE,
    NO_ACTIVE_SOURCE,
    MULTIPLE_SOURCES
}

public class BodiesManager
{
    private bool _isSourceInitialized;
    public Body ReferenceSource;
    private List<Body> _bodyList;

    public int InitFrames;
    private const int _initFramesCap = 25;
    private const double _initTolerance = 0.1;

    private int _activeIndex;
    private bodiesState _state;
    public Body ActiveSource { get; set; }

    public bodiesState State
    {
        get { return _state; }
        set
        {
            _state = value;
            // Replaced with new event system
            //OnStateChanged(value);
            // Raise new event in game class
            // no more event passing through 2 classes 
            Events.instance.Raise(new Game.KinectEvent(value));
        }
    }
    
    public void Init(List<Body> src = null)
    {
        ActiveSource = null;
        _activeIndex = -1;
        _bodyList = src;

        if (_bodyList == null || _bodyList.Count <= 0)
        {
            State = bodiesState.NO_DATA;
        }
        else
        {
            State = bodiesState.NO_ACTIVE_SOURCE;
        }

        _isSourceInitialized = false;
        InitFrames = _initFramesCap;
    }

    public void UpdateStates(List<Body> src)
    {
        //Todo: Referenz lässt sich nicht speichern. Muss jedes mal neu abgelegt werden!
        _bodyList = src;

        //Es lässt sich keine Referenz auf bodyList setzen, deshalb muss jedes mal activeSource neu gesetzt werden.
        if (State == bodiesState.SINGLE_SOURCE || State == bodiesState.INITIALIZE_SOURCE)
        {
            ActiveSource = _bodyList[_activeIndex];
        }

        //Wechel Status, in Abhängigkeit der Anzahl erkannter Bodies.
        if ((_bodyList == null || _bodyList.Count <= 0) && State != bodiesState.NO_DATA)
        {
            State = bodiesState.NO_DATA;
        }
        else if(State != bodiesState.NO_DATA)
        {

            
            //Get amount of tracked bodies
            int trackedBodies = NumberOfBodiesTracked();

            if (trackedBodies > 1 && _state != bodiesState.MULTIPLE_SOURCES)
            {
                State = bodiesState.MULTIPLE_SOURCES;
            }
            else if (trackedBodies == 1 && ActiveSource != null && _state != bodiesState.INITIALIZE_SOURCE && !_isSourceInitialized )
            {
                State = bodiesState.INITIALIZE_SOURCE;
            } else if (_isSourceInitialized && _state != bodiesState.SINGLE_SOURCE && State != bodiesState.MULTIPLE_SOURCES)
            {
                State = bodiesState.SINGLE_SOURCE;
            }
            else if (trackedBodies <= 0 && _state != bodiesState.NO_ACTIVE_SOURCE)
            {
                State = bodiesState.NO_ACTIVE_SOURCE;
            }
        }
    }

    public CameraSpacePoint GetActJointPos(JointType type)
    {
        return ActiveSource.Joints[type].Position;
    }

    public CameraSpacePoint GetRefJointPos(JointType type)
    {
        return ReferenceSource.Joints[type].Position;
    }

    public void InitializeBody()
    {
        //Take every 10 Frames a reference from the original body
        if (InitFrames % 5 == 0 || InitFrames == _initFramesCap || InitFrames < 3)
            ReferenceSource = ActiveSource;
        

        InitFrames--;
        Debug.Log(InitFrames);

        for (var i = 0; i < ActiveSource.Joints.Count; i++)
        {
            //Difference between reference and original
            double differenceY = ReferenceSource.Joints[(JointType) i].Position.Y - ActiveSource.Joints[(JointType) i].Position.Y;
            if (!(differenceY > _initTolerance)) continue;

            Debug.Log("Initialize canceled " + (JointType)i + " | " + differenceY);
            InitFrames = _initFramesCap;
            break;
        }

        //That happens when difference is over tolerance
        if (InitFrames > 0) return;

        _isSourceInitialized = true;
        ReferenceSource = ActiveSource;
    }

    public void Reset()
    {
        ActiveSource = null;
        ReferenceSource = null;
        _activeIndex = -1;
        _isSourceInitialized = false;
        InitFrames = _initFramesCap;
    }

    public void NextPossibleBody()
    {
        for (var i = 0; i < _bodyList.Count; i++)
        {
            if (_bodyList[i].IsTracked)
            {
                SetActiveBody(i);
            }
        }
    }

    public void SetActiveBody(int ind)
    {
        ActiveSource = _bodyList[ind];
        _activeIndex = ind;
    }

    private int NumberOfBodiesTracked()
    {
        return _bodyList.Count(t => t.IsTracked);
    }

    private bool MultipleBodiesTracked()
    {
        return _bodyList.Where((t, i) => t.IsTracked && _activeIndex != i).Any();
    }
}
