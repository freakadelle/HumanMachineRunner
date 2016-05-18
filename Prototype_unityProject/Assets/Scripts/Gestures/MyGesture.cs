using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public abstract class MyGesture
{

    private int minInterval;
    private int minIntervalCap;
    public string name;
    public List<JointTolerance> tolerances; 

    public MyGesture()
    {
        minInterval = 0;
        MinIntervalCap = 0;
    }

    abstract public bool validate(Body _act, Body _ref);
    abstract public void trigger();

    //PROPERTIES

    public int MinIntervalCap
    {
        get { return minIntervalCap; }
        set
        {
            minIntervalCap = value;
        }
    }

    public int MinInterval
    {
        get { return minInterval; }
        set
        {
            minInterval = value;
            minInterval = Mathf.Max(minInterval, 0);
        }
    }
}
