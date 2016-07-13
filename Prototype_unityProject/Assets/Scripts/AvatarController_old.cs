using System;
using Assets.Scripts.Kinect;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class AvatarController : MonoBehaviour
    {
        public float AvatarJumpSpeed = 2f;
        public float AvatarWalkSpeed = 10f;
        public float LevelGravity = 20.0f;
        public float AvatarRotationSpeed = 2.0f;
        public float FloatingPointErrorThreshold = 5f;
        public float DuckNegativeCameraOffset = -0.5f;
        public float DuckNegativeColliderOffset = 2f;
        public float KinectXPositionSensitivy = 0.5f;
        public float GetFuel { get; private set; }

        public static CharacterController CharacterController { get; private set; }

        public AvatarStateMachine.AvatarRotation InitialAvatarRotation;
        private Vector3 _moveDirection = Vector3.zero;
        private static bool _dirtyFlag = true;
        private static Camera _camera;
        private static float _originalCameraYPosition;
        private static float _originalCharacterControllerHeight;
        private static float _kinectPositionX;

        public void Start()
        {
            CharacterController = GetComponent<CharacterController>();
            // Set inital states
            _setAvatarMoveStateToIdle();

            AvatarStateMachine.AvatarRotationState = InitialAvatarRotation;

            GetFuel = Fuel.GetFuel;

            var cams = GetComponentsInChildren<Camera>();
            _camera = cams[0];
            _originalCharacterControllerHeight = CharacterController.height;
            _originalCameraYPosition = _camera.transform.position.y;

            _kinectPositionX = 0;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.gameObject.tag.Equals("barrier"))
            {
                Game._gameState = Game.GameState.Lost;
                SceneManager.LoadScene(1);
            }
        }

        public void Update()
        {

            GetFuel = Fuel.GetFuel;
            Debug.Log(GetFuel);
            if (GetFuel.Equals(0))
            {
                Game._gameState = Game.GameState.Lost;
                //Debug purpose only
                SceneManager.LoadScene(1);
            }


            // Debug.Log(AvatarStateMachine.AvatarMoveState);
            if (AvatarStateMachine.AvatarMoveState != AvatarStateMachine.AvatarMove.Ducking)
                _originalCameraYPosition = _camera.transform.localPosition.y;

            if (Input.GetKeyDown(KeyCode.A) && AvatarStateMachine.AvatarMoveState == AvatarStateMachine.AvatarMove.Running)
                AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Left;

            if (Input.GetKeyDown(KeyCode.D) && AvatarStateMachine.AvatarMoveState == AvatarStateMachine.AvatarMove.Running)
                AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Right;

            if (Input.GetKeyDown(KeyCode.Space) && AvatarStateMachine.AvatarMoveState == AvatarStateMachine.AvatarMove.Running)
                AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Jumping;

            if (Input.GetKey(KeyCode.LeftControl) && AvatarStateMachine.AvatarMoveState == AvatarStateMachine.AvatarMove.Running)
                AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Ducking;

            if (Input.GetKeyUp(KeyCode.LeftControl))
                AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Running;

            if (Input.GetKey(KeyCode.LeftArrow))
                _kinectPositionX = -KinectXPositionSensitivy;

            if (Input.GetKey(KeyCode.RightArrow))
                _kinectPositionX = KinectXPositionSensitivy;

            // Always move in z
            var moveSpeed = AvatarWalkSpeed * Time.deltaTime;
            _moveDirection = new Vector3(_kinectPositionX, 0, moveSpeed);

            _avatarMovement();
            _handleAvatarRotation();

            _moveDirection = transform.TransformDirection(_moveDirection);
            _moveDirection *= AvatarWalkSpeed;

            _moveDirection.y -= LevelGravity * Time.deltaTime;



            if (AvatarStateMachine.AvatarMoveState != AvatarStateMachine.AvatarMove.Left &&
              AvatarStateMachine.AvatarMoveState != AvatarStateMachine.AvatarMove.Right &&
               CharacterController.isGrounded && AvatarStateMachine.AvatarMoveState != AvatarStateMachine.AvatarMove.Ducking)
                _setAvatarMoveStateToIdle();

            _kinectPositionX = 0;
        }

        void FixedUpdate()
        {
            CharacterController.Move(_moveDirection * Time.deltaTime);
        }

        private void _avatarMovement()
        {
            //Debug.Log(_originalCameraYPosition);
            switch (AvatarStateMachine.AvatarMoveState)
            {
                case AvatarStateMachine.AvatarMove.Jumping:
                    if (CharacterController.isGrounded)
                        _moveDirection.y = AvatarJumpSpeed;
                    break;
                case AvatarStateMachine.AvatarMove.Ducking:
                    // TODO: Get Collider and scale it down
                    _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, DuckNegativeCameraOffset, _camera.transform.localPosition.z);
                    CharacterController.height = 1.5f;
                    CharacterController.center = new Vector3(0, -0.5f, 0);
                    break;
                case AvatarStateMachine.AvatarMove.Running:
                    _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, _originalCameraYPosition, _camera.transform.localPosition.z);
                    CharacterController.height = _originalCharacterControllerHeight;
                    CharacterController.center = new Vector3(0, 0, 0);
                    break;
                case AvatarStateMachine.AvatarMove.Left:
                    if (_dirtyFlag)
                    {
                        _dirtyFlag = false;
                        if (AvatarStateMachine.AvatarRotationState != AvatarStateMachine.AvatarRotation.Zero)
                            AvatarStateMachine.AvatarRotationState--;
                        else
                            AvatarStateMachine.AvatarRotationState =
                                AvatarStateMachine.AvatarRotation.ThreeHundredFifteen;
                    }
                    break;
                case AvatarStateMachine.AvatarMove.Right:
                    if (_dirtyFlag)
                    {
                        _dirtyFlag = false;
                        if (AvatarStateMachine.AvatarRotationState !=
                            AvatarStateMachine.AvatarRotation.ThreeHundredFifteen)
                            AvatarStateMachine.AvatarRotationState++;
                        else
                            AvatarStateMachine.AvatarRotationState = AvatarStateMachine.AvatarRotation.Zero;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void _handleAvatarRotation()
        {
            if (!CharacterController.isGrounded || AvatarStateMachine.AvatarMoveState == AvatarStateMachine.AvatarMove.Jumping || AvatarStateMachine.AvatarMoveState == AvatarStateMachine.AvatarMove.Ducking)
                return;

            switch (AvatarStateMachine.AvatarRotationState)
            {
                case AvatarStateMachine.AvatarRotation.Zero:
                    _rotateToAngle(0);
                    _checkAndFixFloatingPointErrors(0);
                    break;
                case AvatarStateMachine.AvatarRotation.FortyFive:
                    _rotateToAngle(45);
                    _checkAndFixFloatingPointErrors(45);
                    break;
                case AvatarStateMachine.AvatarRotation.Ninety:
                    _rotateToAngle(90);
                    _checkAndFixFloatingPointErrors(90);
                    break;
                case AvatarStateMachine.AvatarRotation.OneHundredThirtyFive:
                    _rotateToAngle(135);
                    _checkAndFixFloatingPointErrors(135);
                    break;
                case AvatarStateMachine.AvatarRotation.OneHundredEighty:
                    _rotateToAngle(180);
                    _checkAndFixFloatingPointErrors(180);
                    break;
                case AvatarStateMachine.AvatarRotation.TwoHundredTwentyFive:
                    _rotateToAngle(225);
                    _checkAndFixFloatingPointErrors(225);
                    break;
                case AvatarStateMachine.AvatarRotation.TwoHundredSeventy:
                    _rotateToAngle(270);
                    _checkAndFixFloatingPointErrors(270);
                    break;
                case AvatarStateMachine.AvatarRotation.ThreeHundredFifteen:
                    _rotateToAngle(315);
                    _checkAndFixFloatingPointErrors(315);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void _rotateToAngle(int angle)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0),
                Time.fixedDeltaTime * AvatarRotationSpeed);
        }

        private void _checkAndFixFloatingPointErrors(float angle)
        {
            if (!(Math.Abs(transform.localEulerAngles.y - angle) < FloatingPointErrorThreshold)) return;

            _setAvatarMoveStateToIdle();
            transform.localEulerAngles = new Vector3(0, (int)angle, 0);
            _dirtyFlag = true;
        }

        private static void _setAvatarMoveStateToIdle()
        {
            if (CharacterController.isGrounded)
                AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Running;
        }
    }
}