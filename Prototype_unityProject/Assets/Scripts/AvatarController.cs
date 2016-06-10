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
        public float Score { get; set; }

        public static CharacterController CharacterController { get; private set; }
        private Vector3 _moveDirection = Vector3.zero;

        public void Start()
        {
            CharacterController = GetComponent<CharacterController>();
            // Set inital states
            _setAvatarMoveStateToIdle();
        }


        public void Update()
        {
            if (CharacterController.isGrounded)
            {
                // Always move in z
                var moveSpeed = AvatarWalkSpeed*Time.deltaTime;
                _moveDirection = new Vector3(0, 0, moveSpeed);

                _avatarMovement();

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
            Quaternion targetRotation;
            float diff;
            const float degree = 1;

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
                    gameObject.transform.localScale = Vector3.one;
                    break;
                case AvatarStateMachine.AvatarMove.Left:
                        targetRotation = Quaternion.Euler(0, 45, 0);
                        gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * AvatarTurnSpeed);
                        diff = gameObject.transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y;
                        if (Mathf.Abs(diff) <= degree)
                            _setAvatarMoveStateToIdle();
                    break;
                case AvatarStateMachine.AvatarMove.Right:
                        targetRotation = Quaternion.Euler(0, 315, 0) * new Quaternion(0, -1, 0, 0);
                        gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * AvatarTurnSpeed);
                        diff = gameObject.transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y;
                        if (Mathf.Abs(diff) <= degree)
                            _setAvatarMoveStateToIdle();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void _setAvatarMoveStateToIdle()
        {
            AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Idle;
        }
    }
}
