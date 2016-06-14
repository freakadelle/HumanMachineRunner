using System;
using UnityEngine;


namespace Assets.Scripts
{
    public class AvatarController : MonoBehaviour
    {

        public float AvatarJumpSpeed = 2f;
        public float AvatarWalkSpeed = 10f;
        public float LevelGravity = 20.0f;
        public float AvatarTurnSpeed = 20.0f;
        public float AvatarRotationSpeed = 2.0f;
        public float FloatingPointErrorThreshold = 5f;
        public float Fuel { get; set; }
        public static bool CollectedFuel { get; set; }
        //TODO: methode to trigger collectedFuel on collision see Fuel.cs


        public static CharacterController CharacterController { get; private set; }

        public  AvatarStateMachine.AvatarRotation InitialAvatarRotation;
        private Vector3 _moveDirection = Vector3.zero;
        private static bool _dirtyFlag = true;


        public void Start()
        {
            CharacterController = GetComponent<CharacterController>();
            // Set inital states
            _setAvatarMoveStateToIdle();
            
            AvatarStateMachine.AvatarRotationState = InitialAvatarRotation;

            Fuel = 1;
        }


        public void Update()
        {

            if(Input.GetKeyDown(KeyCode.A))
                AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Left;

            if (Input.GetKeyDown(KeyCode.D))
                AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Right;

          //  UnityEngine.Debug.Log(AvatarStateMachine.AvatarMoveState);

            if (CharacterController.isGrounded)
            {
                // Always move in z
                var moveSpeed = AvatarWalkSpeed*Time.deltaTime;
                _moveDirection = new Vector3(0, 0, moveSpeed);

                _avatarMovement();
                _handleAvatarRotation();

                _moveDirection = transform.TransformDirection(_moveDirection);
                _moveDirection *= AvatarWalkSpeed;
            }

            if(AvatarStateMachine.AvatarMoveState != AvatarStateMachine.AvatarMove.Left && AvatarStateMachine.AvatarMoveState != AvatarStateMachine.AvatarMove.Right)
               _setAvatarMoveStateToIdle();

            _moveDirection.y -= LevelGravity * Time.deltaTime;

            CharacterController.Move(_moveDirection * Time.deltaTime);
        }


       
        private void _avatarMovement()
        {
         
            switch (AvatarStateMachine.AvatarMoveState)
            {
                case AvatarStateMachine.AvatarMove.Jumping:
                    _moveDirection.y = AvatarJumpSpeed;
                    break;                  
                case AvatarStateMachine.AvatarMove.Ducking:
                    gameObject.transform.localScale = new Vector3(1, 0.5f, 1);  // TODO: fix this in final build!
                    break;
                case AvatarStateMachine.AvatarMove.Idle:
                    // TODO: Fix this in final build. Right now this is necessary for duck.
                    //gameObject.transform.localScale = Vector3.one;
                    break;
                case AvatarStateMachine.AvatarMove.Left:
                    if (_dirtyFlag)
                    {
                        _dirtyFlag = false;
                        if(AvatarStateMachine.AvatarRotationState != AvatarStateMachine.AvatarRotation.Zero)
                            AvatarStateMachine.AvatarRotationState--;
                        else
                            AvatarStateMachine.AvatarRotationState = AvatarStateMachine.AvatarRotation.ThreeHundredFifteen;
                            
                        
                    }
                    break;
                case AvatarStateMachine.AvatarMove.Right:
                    if (_dirtyFlag)
                    {
                        _dirtyFlag = false;
                        if(AvatarStateMachine.AvatarRotationState != AvatarStateMachine.AvatarRotation.ThreeHundredFifteen)
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
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0), Time.fixedDeltaTime * AvatarRotationSpeed);
        }

        private void _checkAndFixFloatingPointErrors(float angle)
        {
            Debug.Log(transform.localEulerAngles.y);
            if (!(Math.Abs(transform.localEulerAngles.y - angle) < FloatingPointErrorThreshold)) return;
            
            _setAvatarMoveStateToIdle();
            transform.localEulerAngles = new Vector3(0, (int) angle, 0);
            _dirtyFlag = true;
        }

        private static void _setAvatarMoveStateToIdle()
        {
            AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Idle;
        }
    }
}
