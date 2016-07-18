using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using Assets.Scripts;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Assets.Scripts.Kinect
{
    public enum BodiesState
    {
        NO_DATA,
        INITIALIZE_SOURCE,
        SINGLE_SOURCE,
        NO_ACTIVE_SOURCE,
        MULTIPLE_SOURCES,
        MULTIPLE_SOURCES_IN_FAVE,
        SOURCE_OUT_OF_FAVE_SPOT
    }

    public class BodiesManager
    {
        private bool _isSourceInitialized;
        public Body ReferenceSource;

        public int InitFrames;
        private const int _initFramesCap = 20;
        private const double _initTolerance = 0.15;

        private int _activeIndex;
        private BodiesState _state;
        public Body ActiveSource { get; set; }
        private List<Body> _bodyList;
        private int trackedBodies;

        private bool hasFaveSpot;

        private Blur camerFrontBlur;

        private static Vector2 faveSpotLimitsZ = new Vector2(1.25f, 2.8f);
        private static Vector2 faveSpotLimitsX = new Vector2(2f, -2f);

        //TODO: public static State to trigger weight overload UI on hoverboard
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
            //Init();
        }

        public BodiesManager(IList<Body> lastBodies)
        {
            BodyList = lastBodies;
        }
    
        public void Init(List<Body> src = null)
        {
            ActiveSource = null;
            _activeIndex = -1;
            BodyList = src;

            camerFrontBlur = GameObject.Find("CameraFront").GetComponent<Blur>();
            camerFrontBlur.enabled = false;

            if (BodyList == null || BodyList.Count <= 0)
            {
               // Debug.Log("NoData");
                State = BodiesState.NO_DATA;
            }
            else
            {
               // Debug.Log("No Active Source");
                State = BodiesState.NO_ACTIVE_SOURCE;
            }

            hasFaveSpot = true;

            _isSourceInitialized = false;
            InitFrames = _initFramesCap;
        }

        // TODO: TEST VIA CONSTRUCTOR
        public void UpdateStates(List<Body> src)
        {

            //Todo: Referenz lässt sich nicht speichern. Muss jedes mal neu abgelegt werden!
            BodyList = src;

            //Es lässt sich keine Referenz auf bodyList setzen, deshalb muss jedes mal activeSource neu gesetzt werden.
            if (State == BodiesState.SINGLE_SOURCE || State == BodiesState.INITIALIZE_SOURCE || State == BodiesState.SOURCE_OUT_OF_FAVE_SPOT)
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
                //NUMBER OF BODIES CALC
                //int tempNumerOfBodies = NumberOfBodiesTracked();
                //if (trackedBodies != tempNumerOfBodies)
                //    Debug.Log("NUMBER OF BODIES CHANGED: " + trackedBodies + " --> " + tempNumerOfBodies);

                //trackedBodies = tempNumerOfBodies;

                int numberOfBodiesInFaveSpot = 0;
                foreach (var body in BodyList)
                {
                    CameraSpacePoint tempBase = body.Joints[JointType.SpineBase].Position;
                    if (isInFaveSpot(tempBase) && body.IsTracked)
                    {
                        numberOfBodiesInFaveSpot++;
                    }
                }

                if (numberOfBodiesInFaveSpot > 1 && _state != BodiesState.MULTIPLE_SOURCES)
                {

                    State = BodiesState.MULTIPLE_SOURCES;
                    ActiveSource = null;
                    _isSourceInitialized = false;
                    //else if (numberOfBodiesInFaveSpot == 1)
                    //{
                    //    State = BodiesState.SINGLE_SOURCE;
                    //}
                    //else
                    //{
                    //    State = BodiesState.NO_ACTIVE_SOURCE;
                    //}
                }
                else if (numberOfBodiesInFaveSpot == 1 && _state != BodiesState.INITIALIZE_SOURCE && !_isSourceInitialized )
                {
                    if (ActiveSource == null)
                    {
                        NextPossibleBody();
                    }
                    State = BodiesState.INITIALIZE_SOURCE;
                } else if (numberOfBodiesInFaveSpot == 1 && _isSourceInitialized && _state != BodiesState.SINGLE_SOURCE && State != BodiesState.SOURCE_OUT_OF_FAVE_SPOT)
                {
                    if (ActiveSource == null)
                    {
                        NextPossibleBody();
                    }
                    State = BodiesState.SINGLE_SOURCE;   
                }
                else if (numberOfBodiesInFaveSpot <= 0 && _state != BodiesState.NO_ACTIVE_SOURCE)
                {
                    State = BodiesState.NO_ACTIVE_SOURCE;
                }
            }

            if (hasFaveSpot)
            {
               handleFaveSpot();
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
            //Debug.Log(InitFrames);

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
                if (BodyList[i].IsTracked && isInFaveSpot(BodyList[i].Joints[JointType.SpineBase].Position))
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

        private bool isInFaveSpot(CameraSpacePoint _point)
        {
            if ((_point.Z > faveSpotLimitsZ.x && _point.Z < faveSpotLimitsZ.y) && (_point.X < faveSpotLimitsX.x && _point.X > faveSpotLimitsX.y))
            {
                return true;
            }
            return false;
        }

        private void handleFaveSpot()
        {
            bool _isInFaveSpot = false;
            CameraSpacePoint basePos = new CameraSpacePoint();

            if (ActiveSource != null)
            {
                basePos = ActiveSource.Joints[JointType.SpineBase].Position;
                _isInFaveSpot = isInFaveSpot(basePos);

                //Debug.Log("BASE_POS: " + ActiveSource.Joints[JointType.SpineBase].ToString());
            }

            switch (State)
            {
                case BodiesState.SINGLE_SOURCE:
                    if (!_isInFaveSpot && State != BodiesState.SOURCE_OUT_OF_FAVE_SPOT)
                    {
                        camerFrontBlur.enabled = true;
                        State = BodiesState.SOURCE_OUT_OF_FAVE_SPOT;
                    }
                    break;
                case BodiesState.SOURCE_OUT_OF_FAVE_SPOT:



                    if (_isInFaveSpot && State != BodiesState.SINGLE_SOURCE)
                    {
                        camerFrontBlur.enabled = false;
                        State = BodiesState.SINGLE_SOURCE;
                    }
                    else
                    {
                        float diff = 0;
                        if (basePos.Z < faveSpotLimitsZ.x)
                        {
                            diff += faveSpotLimitsZ.x - basePos.Z;
                        }
                        else
                        {
                            diff += basePos.Z - faveSpotLimitsZ.y;
                        }

                        if (basePos.X < faveSpotLimitsX.y)
                        {
                            diff += basePos.X - faveSpotLimitsX.y;
                        }
                        else
                        {
                            diff += faveSpotLimitsX.x - basePos.Z;
                        }

                        camerFrontBlur.iterations = (int)(diff * 6);
                    }
                    break;
                case BodiesState.MULTIPLE_SOURCES:
                    //int numberOfBodiesInFaveSpot = 0;
                    //foreach (var body in BodyList)
                    //{
                    //    CameraSpacePoint tempBase = body.Joints[JointType.SpineBase].Position;
                    //    if (isInFaveSpot(tempBase) && body.IsTracked)
                    //    {
                    //        numberOfBodiesInFaveSpot++;
                    //    }
                    //}
                    //Debug.Log("numbers:" + numberOfBodiesInFaveSpot);

                    //if (numberOfBodiesInFaveSpot > 1)
                    //{
                    //    State = BodiesState.MULTIPLE_SOURCES_IN_FAVE;
                    //    ActiveSource = null;
                    //}
                    //else if (numberOfBodiesInFaveSpot == 1)
                    //{
                    //    State = BodiesState.SINGLE_SOURCE;
                    //}
                    //else
                    //{
                    //    State = BodiesState.NO_ACTIVE_SOURCE;
                    //}
                    break;
            }
        }

    }
}