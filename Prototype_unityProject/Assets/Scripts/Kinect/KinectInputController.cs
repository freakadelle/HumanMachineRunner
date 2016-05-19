using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Windows.Kinect;
using Assets.Scripts;
using KineticSpace.Kinect;

using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class KinectInputController : DefaultKinectGestureEvaluator
{

    public BodiesManager bodiesManager;
    private List<MyGesture> myGestures;

    //Sate Event Handler
    public event EventHandler<MyEvArgs<bodiesState>> KinectStateChanged;
    public event EventHandler<MyEvArgs<bodiesState>> KinectStateUpdate;
    public event EventHandler<MyEvArgs<string>> GestureDetected;

    public KinectInputController(string baseDataPath) : base(baseDataPath)
    {
        QualitySettings.vSyncCount = 0;

        bodiesManager = new BodiesManager();
        bodiesManager.StateChanged += OnBodyStateChange;

        myGestures = new List<MyGesture>();

        new DefaultKinectGestureEvaluator(baseDataPath);
    }

    public void update()
    {
        //Look for bodies
        var bodies = _kinectSource.LastBodies;

        waitForBodySource();

        //Todo: Die Referenz muss immer erneut mitgegeben werden. Weshalb kann er die Referenz beim Konstruktoraufruf nich benutzen.
        bodiesManager.updateStates(bodies);

        OnKinectStateUpdate(bodiesManager.State);
    }

    //Implemented Methods
    //------------------------------------------------------------------------------------------------------

    private void OnBodyStateChange(object sender, MyEvArgs<bodiesState> _data)
    {
        Debug.Log("BodySourceManager State changed: " + _data.data);
        OnKinectStateChanged(_data.data);
    }

    private void waitForBodySource()
    {
        var bodies = _kinectSource.LastBodies;

        if ( bodies != null)
        {
            //Falls Sensordaten vorhanden. Lege einen neuen BodyManager mit den SensorDaten der Bodies an.
            if (bodiesManager.State == bodiesState.NO_DATA)
            {
                bodiesManager.init(bodies);
            }
        }
    }

    public void handleGestureInput()
    {
        handleHardCodeGestures();
        handleKineticSpaceGestures();
    }

    public void handleKineticSpaceGestures()
    {
        //temporarily saves detected gestures
        var gestures = DetectedGestures;

        if (gestures.Count > 0)
        {
            Debug.Log(gestures[0]);
        }

        //Clear gestures list
        SignalDetectedGesturesProcessed();
    }

    public void handleHardCodeGestures()
    {
        //Detect and validate hardcode gestures from Gesturelist
        for (int i = 0; i < myGestures.Count; i++)
        {
            //Debug.Log(myGestures[i].MinInterval);
            if (myGestures[i].validate(bodiesManager.ActiveSource, bodiesManager.referenceSource) && myGestures[i].MinInterval <= 0)
            {
                myGestures[i].trigger();

                
            }
        }
    }

    public void addHardCodeGesture(MyGesture _gesture)
    {
        myGestures.Add(_gesture);
    }

    //CHANGED LISTENER
    protected virtual void OnKinectStateChanged(bodiesState _value)
    {
        if (KinectStateChanged != null)
        {
            KinectStateChanged(this, new MyEvArgs<bodiesState>(_value));
        }
    }

    protected virtual void OnKinectStateUpdate(bodiesState _value)
    {
        if (KinectStateUpdate != null)
        {
            KinectStateUpdate(this, new MyEvArgs<bodiesState>(_value));
        }
    }
}
