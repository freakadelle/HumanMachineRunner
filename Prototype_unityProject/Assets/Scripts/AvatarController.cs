﻿using System;
using Assets.Scripts;
using Assets.Scripts.Kinect;
using UnityEngine;

/*TODO: Keyboard Controls: A - rotate to left, D - rotate to right, C - duck, SPACE - jump, Left Arrow - strafe to left, Right Arrow - strafe to right
        --> Add Kinect gestures to if/else of control keys (e.g. if (Input.GetButton("Jump") || Kinect.Gestures = Jump) ) 
*/
//TODO: Set StrafeValue to Kinect input

public class AvatarController : MonoBehaviour
{
    //Avatar
    public float StrafeValue = 10;

    public float GravityMultiplier = 30;
    public int Speed = 7;

    private Vector2 _input;
    private CharacterController _controller;
    private Vector3 _moveDirection = Vector3.zero;
    private const int StickToGroundForce = 1;
    private Transform _cameras;

    //Duck
    private const float DuckNegativeCameraOffset = -0.5f;
    private Vector3 _camsPos;
    private float _controllerHeight;
    private Vector3 _controllerCenter;

    //Rotation
    public float RotationSpeed = 3f;
    private const float FloatingPointErrorThreshold = 7f;
    private static bool _dirtyFlag = true;

    //Fuel
    public float GetFuel { get; private set; }

    public enum AvatarRotation
    {
        Zero,
        FortyFive,
        Ninety,
        OneHundredThirtyFive,
        OneHundredEighty,
        TwoHundredTwentyFive,
        TwoHundredSeventy,
        ThreeHundredFifteen
    }

    public static AvatarRotation AvatarRotationState { get; set; }

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        AvatarRotationState = AvatarRotation.Ninety;
        _cameras = gameObject.transform.FindChild("Cameras");
        _camsPos = _cameras.localPosition;
        _controllerCenter = _controller.center;
        _controllerHeight = _controller.height;
    }

    private void Update()
    {
        ////////////////////////////// FUEL //////////////////////////////////////////
        GetFuel = Fuel.GetFuel;
        if (GetFuel <= 0)
        {
            Game._gameState = Game.GameState.Lost;
        }
        /////////////////////////////////////////////////////////////////////////////

        _input = GetInput();

        Vector3 desiredMove = transform.forward * _input.y + transform.right * _input.x;
        //Vector3 desiredMove = transform.forward * _input.y;

        _moveDirection.x = desiredMove.x * Speed;
        _moveDirection.z = desiredMove.z * Speed;

        if (_controller.isGrounded)
        {
            _moveDirection.y = -StickToGroundForce;

            /////////////////////////////// JUMP /////////////////////////////////////
            if (Input.GetButtonDown("Jump") || KinectInputController.KinectGestureState == KinectGestureStates.JUMPING)
            {
                _moveDirection.y = GravityMultiplier;
            }
            
            /////////////////////////////////////////////////////////////////////////

            ////////////////////////////// DUCK ////////////////////////////////////
            if (Input.GetKeyDown(KeyCode.C) || KinectInputController.KinectGestureState == KinectGestureStates.DUCKING)
            {
                Duck();
            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                StandUp();
            }
            /////////////////////////////////////////////////////////////////////////
        }
        else
        {
            _moveDirection += new Vector3(0, -3f, 0) * GravityMultiplier * Time.deltaTime;
        }

        /////////////////////////////// ROTATE //////////////////////////////////
        HandleAvatarRotation();
        if ((Input.GetKeyDown(KeyCode.A) || KinectInputController.KinectGestureState == KinectGestureStates.ROT_LEFT) && _dirtyFlag)
        {
            _dirtyFlag = false;
            if (AvatarRotationState != AvatarRotation.Zero)
                AvatarRotationState--;
            else
                AvatarRotationState = AvatarRotation.ThreeHundredFifteen;
        }
        if ((Input.GetKeyDown(KeyCode.D) || KinectInputController.KinectGestureState == KinectGestureStates.ROT_RIGHT) && _dirtyFlag)
        {
            _dirtyFlag = false;
            if (AvatarRotationState !=
                AvatarRotation.ThreeHundredFifteen)
                AvatarRotationState++;
            else
                AvatarRotationState = AvatarRotation.Zero;
        }
        ///////////////////////////////////////////////////////////////////////////

        //_controller.transform.position = new Vector3(KinectInputController.kinectHeadPos.X, _controller.transform.position.y, _controller.transform.position.z);
        //_cameras.localPosition = new Vector3(KinectInputController.kinectHeadPos.X, _cameras.localPosition.y, _cameras.localPosition.z);
        _controller.Move(_moveDirection * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.tag.Equals("barrier"))
        {
            Game._gameState = Game.GameState.Lost;
            //SceneManager.LoadScene(1);
        }
    }

    private Vector2 GetInput()
    {
        ///////////////////////////////////// STRAFE ///////////////////////////////
        float horizontal;

        if (Input.GetKey(KeyCode.LeftArrow) || KinectInputController.kinectMovingState == KinectMovingStates.MOV_LEFT)
        {
            horizontal = -StrafeValue;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || KinectInputController.kinectMovingState == KinectMovingStates.MOV_RIGHT)
        {
            horizontal = StrafeValue;
        }
        else
        {
            horizontal = 0;
        }

        ///////////////////////////////////////////////////////////////////////////


        /////////////////////////////// DRIVE /////////////////////////////////////
        float vertical = Speed;
        ///////////////////////////////////////////////////////////////////////////


        var input = new Vector2(horizontal, vertical);
        //var input = new Vector2(0, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }
        return input;
    }

    private void Duck()
    {
        _cameras.localPosition = new Vector3(_cameras.localPosition.x, DuckNegativeCameraOffset, _cameras.localPosition.z);
        _controller.height = 1.5f;
        _controller.center = new Vector3(0, -0.5f, 0);
    }

    private void StandUp()
    {
        //_cameras.localPosition = _camsPos;
        _cameras.localPosition = new Vector3(_cameras.localPosition.x, _camsPos.y, _cameras.localPosition.z);
        _controller.height = _controllerHeight;
        _controller.center = _controllerCenter;
    }

    private void HandleAvatarRotation()
    {
        if (!_controller.isGrounded)
            return;

        switch (AvatarRotationState)
        {
            case AvatarRotation.Zero:
                RotateToAngle(0);
                _checkAndFixFloatingPointErrors(0);
                break;
            case AvatarRotation.FortyFive:
                RotateToAngle(45);
                _checkAndFixFloatingPointErrors(45);
                break;
            case AvatarRotation.Ninety:
                RotateToAngle(90);
                _checkAndFixFloatingPointErrors(90);
                break;
            case AvatarRotation.OneHundredThirtyFive:
                RotateToAngle(135);
                _checkAndFixFloatingPointErrors(135);
                break;
            case AvatarRotation.OneHundredEighty:
                RotateToAngle(180);
                _checkAndFixFloatingPointErrors(180);
                break;
            case AvatarRotation.TwoHundredTwentyFive:
                RotateToAngle(225);
                _checkAndFixFloatingPointErrors(225);
                break;
            case AvatarRotation.TwoHundredSeventy:
                RotateToAngle(270);
                _checkAndFixFloatingPointErrors(270);
                break;
            case AvatarRotation.ThreeHundredFifteen:
                RotateToAngle(315);
                _checkAndFixFloatingPointErrors(315);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RotateToAngle(int angle)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0),
            Time.fixedDeltaTime * RotationSpeed);
    }

    private void _checkAndFixFloatingPointErrors(float angle)
    {

        if (!(Math.Abs(transform.localEulerAngles.y - angle) < FloatingPointErrorThreshold)) return;
        transform.localEulerAngles = new Vector3(0, (int)angle, 0);

        _dirtyFlag = true;
    }

}

