using System.Collections.Generic;
using System.Linq;
using KineticSpace.Kinect;
using UnityEngine;

namespace Assets.Scripts.Kinect
{
    public class KinectInputController : DefaultKinectGestureEvaluator
    {
        public BodiesManager BodiesManager;
        private readonly List<MyGesture> _myGestures;
        
        public KinectInputController(string baseDataPath) : base(baseDataPath)
        {
            QualitySettings.vSyncCount = 0;
            // TODO: _kinectSource.Sensor.BodyFrameSource
            BodiesManager = new BodiesManager(_kinectSource.LastBodies);
            //BodiesManager.State = BodiesState.NO_DATA;

            _myGestures = new List<MyGesture>();
            // ReSharper disable once ObjectCreationAsStatement
            new DefaultKinectGestureEvaluator(baseDataPath);
        }

        public void Update()
        {
            //Look for bodies
            //var bodies = _kinectSource.LastBodies;

            WaitForBodySource();

            //Todo: Die Referenz muss immer erneut mitgegeben werden. Weshalb kann er die Referenz beim Konstruktoraufruf nich benutzen.
            BodiesManager.UpdateStates(/*bodies*/);

            //  OnKinectStateUpdate(bodiesManager.State);
            // New event system fire on every update
            Events.instance.Raise(new Game.KinectUpdateEvent(BodiesManager.State));
        }
    
        private void WaitForBodySource()
        {
            var bodies = _kinectSource.LastBodies;
            if (bodies == null) return; // early out

            //Falls Sensordaten vorhanden. Lege einen neuen BodyManager mit den SensorDaten der Bodies an.
            if (BodiesManager.State == BodiesState.NO_DATA)
            {
                BodiesManager.Init(bodies);
            }
        }


        public void HandleGestureInput()
        {
            HandleHardCodeGestures();
            HandleKineticSpaceGestures();
        }

        public void HandleKineticSpaceGestures()
        {
            //temporarily saves detected gestures
            var gestures = DetectedGestures;

            if (gestures != null && gestures.Count > 0)
                Debug.Log(gestures[0]);
            
            //Clear gestures list
            SignalDetectedGesturesProcessed();
        }

        public void HandleHardCodeGestures()
        {
            //Detect and validate hardcode gestures from Gesturelist
            foreach (var gesture in _myGestures.Where(gesture => gesture.validate(BodiesManager.ActiveSource, BodiesManager.ReferenceSource) && gesture.MinInterval <= 0))
                gesture.trigger();
        }

        public void AddHardCodeGesture(MyGesture gesture)
        {
            _myGestures.Add(gesture);
        }
    }
}
