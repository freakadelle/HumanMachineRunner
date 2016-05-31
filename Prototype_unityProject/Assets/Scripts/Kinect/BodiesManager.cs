using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Scripts.Kinect
{
    public enum BodiesState
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

        public int InitFrames;
        private const int _initFramesCap = 25;
        private const double _initTolerance = 0.1;

        private int _activeIndex;
        private BodiesState _state;
        public Body ActiveSource { get; set; }

        public BodiesState State
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

        // Test via Constructor
        private IList<Body> BodyList { get; set; }

        public BodiesManager()
        {
        
        }

        public BodiesManager(BodyFrameSource lastBodies)
        {
            BodyList = lastBodies;
        }
    
        public void Init(List<Body> src = null)
        {
            ActiveSource = null;
            _activeIndex = -1;
            BodyList = src;

            if (BodyList == null || BodyList.Count <= 0)
            {
                State = BodiesState.NO_DATA;
            }
            else
            {
                State = BodiesState.NO_ACTIVE_SOURCE;
            }

            _isSourceInitialized = false;
            InitFrames = _initFramesCap;
        }

        // TODO: TEST VIA CONSTRUCTOR
        public void UpdateStates(/*List<Body> src*/)
        {
            //Todo: Referenz lässt sich nicht speichern. Muss jedes mal neu abgelegt werden!
            //_bodyList = src;

            //Es lässt sich keine Referenz auf bodyList setzen, deshalb muss jedes mal activeSource neu gesetzt werden.
            if (State == BodiesState.SINGLE_SOURCE || State == BodiesState.INITIALIZE_SOURCE)
            {
                ActiveSource = BodyList[_activeIndex];
            }

            //Wechel Status, in Abhängigkeit der Anzahl erkannter Bodies.
            if ((BodyList == null || BodyList.Count <= 0) && State != BodiesState.NO_DATA)
            {
                State = BodiesState.NO_DATA;
            }
            else if(State != BodiesState.NO_DATA)
            {

            
                //Get amount of tracked bodies
                int trackedBodies = NumberOfBodiesTracked();

                if (trackedBodies > 1 && _state != BodiesState.MULTIPLE_SOURCES)
                {
                    State = BodiesState.MULTIPLE_SOURCES;
                }
                else if (trackedBodies == 1 && ActiveSource != null && _state != BodiesState.INITIALIZE_SOURCE && !_isSourceInitialized )
                {
                    State = BodiesState.INITIALIZE_SOURCE;
                } else if (_isSourceInitialized && _state != BodiesState.SINGLE_SOURCE && State != BodiesState.MULTIPLE_SOURCES)
                {
                    State = BodiesState.SINGLE_SOURCE;
                }
                else if (trackedBodies <= 0 && _state != BodiesState.NO_ACTIVE_SOURCE)
                {
                    State = BodiesState.NO_ACTIVE_SOURCE;
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
            for (var i = 0; i < BodyList.Count; i++)
            {
                if (BodyList[i].IsTracked)
                {
                    SetActiveBody(i);
                }
            }
        }

        public void SetActiveBody(int ind)
        {
            ActiveSource = BodyList[ind];
            _activeIndex = ind;
        }

        private int NumberOfBodiesTracked()
        {
            return BodyList.Count(t => t.IsTracked);
        }

        private bool MultipleBodiesTracked()
        {
            return BodyList.Where((t, i) => t.IsTracked && _activeIndex != i).Any();
        }
    }
}