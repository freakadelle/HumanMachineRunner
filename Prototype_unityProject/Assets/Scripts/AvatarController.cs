using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class AvatarController : MonoBehaviour
    {

        public float AvatarJumpSpeed = 8f;
        public float AvatarWalkSpeed = 6f;
        public float LevelGravity = 20.0f;
        public float Score { get; set; }

        private CharacterController _characterController;
        private Vector3 _moveDirection = Vector3.zero;

        public void Start()
        {
            _characterController = GetComponent<CharacterController>();
            // Set inital states
            _setAvatarMoveStateToIdle();
        }


        public void Update()
        {
            if (_characterController.isGrounded)
            {
                // Always move in z
                var moveSpeed = AvatarWalkSpeed*Time.deltaTime;
                _moveDirection = new Vector3(0, 0, moveSpeed);

                _avatarMovement();

                _moveDirection = transform.TransformDirection(_moveDirection);
                _moveDirection *= AvatarWalkSpeed;
            }

            _setAvatarMoveStateToIdle();

            _moveDirection.y -= LevelGravity * Time.deltaTime;

            _characterController.Move(_moveDirection * Time.deltaTime);
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
                    gameObject.transform.localScale = Vector3.one;
                    break;
                case AvatarStateMachine.AvatarMove.Left:
                    gameObject.transform.Rotate(new Vector3(0,1,0), -45);
                    break;
                case AvatarStateMachine.AvatarMove.Right:
                    gameObject.transform.Rotate(new Vector3(0, 1, 0), 45);
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
