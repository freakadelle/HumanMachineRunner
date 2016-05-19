using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

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

    private Body activeSource;
    private bool isSourceInitialized;
    public Body referenceSource;
    private List<Body> bodyList;

    private int initFrames;
    private int initFramesCap = 25;
    private double initTolerance = 0.1;

    private int activeIndex;
    private bodiesState state;

    public event EventHandler<MyEvArgs<bodiesState>> StateChanged;

    public void init(List<Body> _src = null)
    {
        activeSource = null;
        activeIndex = -1;
        bodyList = _src;

        if (bodyList == null || bodyList.Count <= 0)
        {
            State = bodiesState.NO_DATA;
        }
        else
        {
            State = bodiesState.NO_ACTIVE_SOURCE;
        }

        isSourceInitialized = false;
        initFrames = initFramesCap;
    }

    public void updateStates(List<Body> _src)
    {

        //Todo: Referenz lässt sich nicht speichern. Muss jedes mal neu abgelegt werden!
        bodyList = _src;

        //Es lässt sich keine Referenz auf bodyList setzen, deshalb muss jedes mal activeSource neu gesetzt werden.
        if (State == bodiesState.SINGLE_SOURCE || State == bodiesState.INITIALIZE_SOURCE)
        {
            activeSource = bodyList[activeIndex];
        }

        //Wechel Status, in Abhängigkeit der Anzahl erkannter Bodies.
        if ((bodyList == null || bodyList.Count <= 0) && State != bodiesState.NO_DATA)
        {
            State = bodiesState.NO_DATA;
        }
        else if(State != bodiesState.NO_DATA)
        {

            
            //Get amount of tracked bodies
            int trackedBodies = numberOfBodiesTracked();

            if (trackedBodies > 1 && state != bodiesState.MULTIPLE_SOURCES)
            {
                State = bodiesState.MULTIPLE_SOURCES;
            }
            else if (trackedBodies == 1 && activeSource != null && state != bodiesState.INITIALIZE_SOURCE && !isSourceInitialized )
            {
                State = bodiesState.INITIALIZE_SOURCE;
            } else if (isSourceInitialized && state != bodiesState.SINGLE_SOURCE && State != bodiesState.MULTIPLE_SOURCES)
            {
                State = bodiesState.SINGLE_SOURCE;
            }
            else if (trackedBodies <= 0 && state != bodiesState.NO_ACTIVE_SOURCE)
            {
                State = bodiesState.NO_ACTIVE_SOURCE;
            }
        }
    }

    public CameraSpacePoint getActJointPos(JointType _type)
    {
        return activeSource.Joints[_type].Position;
    }

    public CameraSpacePoint getRefJointPos(JointType _type)
    {
        return referenceSource.Joints[_type].Position;
    }

    public void initializeBody()
    {
        //Take every 10 Frames a reference from the original body
        if (initFrames % 5 == 0 || initFrames == initFramesCap || initFrames < 3)
        {
            referenceSource = activeSource;
        }

        initFrames--;
        Debug.Log(initFrames);

        for (int i = 0; i < activeSource.Joints.Count; i++)
        {
            //Difference between refernce and original
            double differenceY = referenceSource.Joints[(JointType) i].Position.Y - activeSource.Joints[(JointType) i].Position.Y;
            //double differenceX = referenceSource.Joints[(JointType)i].Position.X - activeSource.Joints[(JointType)i].Position.X;

            if (differenceY > initTolerance)
            {
                Debug.Log("Initialize cancelled " + (JointType)i + " | " + differenceY);
                initFrames = initFramesCap;
                break;
            }
        }

        //That happens when difference is over tolerance
        if (initFrames <= 0)
        {
            isSourceInitialized = true;
            referenceSource = activeSource;
        }
    }

    public void reset()
    {
        activeSource = null;
        referenceSource = null;
        activeIndex = -1;
        isSourceInitialized = false;
        initFrames = initFramesCap;
    }

    public void nextPossibleBody()
    {
        for (int i = 0; i < bodyList.Count; i++)
        {
            if (bodyList[i].IsTracked)
            {
                setActiveBody(i);
            }
        }
    }

    public void setActiveBody(int ind)
    {
        activeSource = bodyList[ind];
        activeIndex = ind;
    }

    private int numberOfBodiesTracked()
    {
        int nbr = 0;
        for (int i = 0; i < bodyList.Count; i++)
        {
            if (bodyList[i].IsTracked)
            {
                nbr++;
            }
        }

        return nbr;
    }

    private bool multipleBodiesTracked()
    {
        for (int i = 0; i < bodyList.Count; i++)
        {
            if (bodyList[i].IsTracked && activeIndex != i)
            {
                return true;
            }
        }

        return false;
    }

    //CHANGED LISTENER

    protected virtual void OnStateChanged(bodiesState _value)
    {
        if (StateChanged != null)
        {
            StateChanged(this, new MyEvArgs<bodiesState>(_value));
        }
    }

    //PROPERTIES
    public Body ActiveSource
    {
        get { return activeSource; }
        set { activeSource = value; }
    }

    public bodiesState State
    {
        get { return state; }
        set
        {
            state = value;
            OnStateChanged(value);
        }
    }
}
