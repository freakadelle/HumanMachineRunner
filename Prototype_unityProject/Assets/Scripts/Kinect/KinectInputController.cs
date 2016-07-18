using System;
using System.Collections.Generic;
using Windows.Kinect;
using KineticSpace.Kinect;
using UnityEngine;

namespace Assets.Scripts.Kinect
{
    public class KinectInputController : DefaultKinectGestureEvaluator
    {
        public static BodiesManager BodiesManager;
        private readonly List<MyGesture> _myGestures;

        public static KinectGestureStates kinectGestureState;
        public static KinectMovingStates kinectMovingState;
        public static CameraSpacePoint kinectHeadPos;
        public static CameraSpacePoint kinectBasePos;

        private CameraSpacePoint kinectHeadPos_ref;
        private CameraSpacePoint kinectBasePos_ref;
        private float xRecognitionThreshold;
        
        public KinectInputController(string baseDataPath) : base(baseDataPath)
        {
            QualitySettings.vSyncCount = 0;
            // TODO: _kinectSource.Sensor.BodyFrameSource
            BodiesManager = new BodiesManager(_kinectSource.LastBodies);

            _myGestures = new List<MyGesture>();

            kinectGestureState = KinectGestureStates.IDLE;
            kinectMovingState = KinectMovingStates.STRAIGHT;

            // ReSharper disable once ObjectCreationAsStatement
            new DefaultKinectGestureEvaluator(baseDataPath);
        }

        public void Update()
        {
            //Look for bodies
            var bodies = _kinectSource.LastBodies;

            WaitForBodySource();

            //Todo: Die Referenz muss immer erneut mitgegeben werden. Weshalb kann er die Referenz beim Konstruktoraufruf nich benutzen.
            BodiesManager.UpdateStates(bodies);

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
            //var gestures = DetectedGestures;

            //if (gestures != null && gestures.Count > 0)
            //{
            //    if (gestures[0] == "ll")
            //    {
            //        KinectGestureState = KinectGestureStates.ROT_LEFT;
            //    } else if (gestures[0] == "rr")
            //    {
            //        KinectGestureState = KinectGestureStates.ROT_RIGHT;
            //    }
            //}
            
            ////Clear gestures list
            //SignalDetectedGesturesProcessed();
        }

        public void HandleHardCodeGestures()
        {
            //Detect and validate hardcode gestures from Gesturelist
            //foreach (var gesture in _myGestures.Where(gesture => gesture.validate(BodiesManager.ActiveSource, BodiesManager.ReferenceSource) && gesture.MinInterval <= 0))
            //    gesture.trigger();

            //Get initialized BodyReference Points
            kinectBasePos_ref = BodiesManager.ReferenceSource.Joints[JointType.SpineBase].Position;
            kinectHeadPos_ref = BodiesManager.ReferenceSource.Joints[JointType.Head].Position;

            //Get activeSource kinectData
            kinectBasePos = BodiesManager.ActiveSource.Joints[JointType.SpineBase].Position;
            kinectHeadPos = BodiesManager.ActiveSource.Joints[JointType.Head].Position;
            
            //Limit CameraSpacePoint to boundaries
            kinectBasePos = limitCameraSpacePointTo(kinectBasePos, new Vector2(-1f, 1f), new Vector2(-999, 999), new Vector2(-999, 999));
            kinectHeadPos = limitCameraSpacePointTo(kinectHeadPos, new Vector2(-1, 1), new Vector2(-999, 999), new Vector2(-999, 999));

            //KineticSpace detected Gesture
            var gestures = DetectedGestures;

            if (kinectBasePos.X - xRecognitionThreshold > 0.005f)
            {
                //kinectMovingState = KinectMovingStates.MOV_RIGHT;
            } else if (kinectBasePos.X - xRecognitionThreshold < -0.005f)
            {
                //kinectMovingState = KinectMovingStates.MOV_LEFT;
            }
            else
            {
                kinectMovingState = KinectMovingStates.STRAIGHT;
            }

            if ((kinectHeadPos.Y - kinectHeadPos_ref.Y) < -0.20f)
            {
                KinectGestureState = KinectGestureStates.DUCKING;
            } else if ((kinectHeadPos.Y - kinectHeadPos_ref.Y) > 0.10f)
            {
                KinectGestureState = KinectGestureStates.JUMPING;
            }
            else if ((kinectBasePos.X - kinectHeadPos_ref.X) > 0.5f)
            {
                kinectMovingState = KinectMovingStates.MOV_RIGHT;
            }
            else if ((kinectBasePos.X - kinectHeadPos_ref.X) < -0.5f)
            {
                kinectMovingState = KinectMovingStates.MOV_LEFT;
            }
            else if (gestures != null && gestures.Count > 0 && gestures[0] == "left")
            {
                KinectGestureState = KinectGestureStates.ROT_LEFT;
            }
            else if (gestures != null && gestures.Count > 0 && gestures[0] == "right")
            {
                KinectGestureState = KinectGestureStates.ROT_RIGHT;
            }
            else
            {
                KinectGestureState = KinectGestureStates.IDLE;
            }

            xRecognitionThreshold = kinectBasePos.X;

            //Clear gestures list
            SignalDetectedGesturesProcessed();
        }

        public void AddHardCodeGesture(MyGesture gesture)
        {
            _myGestures.Add(gesture);
        }

        private CameraSpacePoint limitCameraSpacePointTo(CameraSpacePoint camP, Vector2 limX, Vector2 limY, Vector2 limZ)
        {
            camP.X = Math.Max(camP.X, limX.x);
            camP.X = Math.Min(camP.X, limX.y);
            camP.Y = Math.Max(camP.Y, limY.x);
            camP.Y = Math.Min(camP.Y, limY.y);
            camP.Z = Math.Max(camP.Z, limZ.x);
            camP.Z = Math.Min(camP.Z, limZ.y);
            return camP;
        }

        public static KinectGestureStates KinectGestureState
        {
            get
            {
                return kinectGestureState;
            }
            set
            {
                kinectGestureState = value;
            }
        }
    }

    public enum KinectGestureStates
    {
        DUCKING,
        JUMPING,
        ROT_LEFT,
        ROT_RIGHT,
        IDLE
    }

    public enum KinectMovingStates
    {
        MOV_LEFT,
        MOV_RIGHT,
        STRAIGHT
    }
}
