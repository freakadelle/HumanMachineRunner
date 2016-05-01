using System.IO;
using KineticSpace.Kinect;

using UnityEngine;

public class KinecticSpaceController : MonoBehaviour
{
    private DefaultKinectGestureEvaluator _evaluator;

    void OnApplicationQuit()
    {
        this._evaluator.Stop();
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;

        var dataPath = Path.Combine(Application.dataPath, "data");

        this._evaluator = new DefaultKinectGestureEvaluator(dataPath);

        this._evaluator.AddGesture("Jump");
        //this._evaluator.AddGesture("stierr");

        this._evaluator.Start();
    }

    void Update()
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
}