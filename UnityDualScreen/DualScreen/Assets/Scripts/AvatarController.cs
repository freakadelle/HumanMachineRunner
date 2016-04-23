using UnityEngine;

namespace Assets.Scripts
{
    public class AvatarController : MonoBehaviour
    {

        public float AvatarJumpSpeed = 8f;
        public float AvatarWalkSpeed = 6f;
        public float LevelGravity = 20.0f;

        public float[] AvatarLanePositionFloats = new float[3]; 

        private CharacterController _characterController;
        private Vector3 _moveDirection = Vector3.zero;

        public void Start()
        {
            _characterController = GetComponent<CharacterController>();
            // Set inital states
            _setAvatarMoveStateToIdle();
            AvatarStateMachine.AvatarLaneState = AvatarStateMachine.AvatarLane.Middle;

            // Set inital Lane positions
            AvatarLanePositionFloats[0] = -3F;
            AvatarLanePositionFloats[1] = 0F;
            AvatarLanePositionFloats[2] = 3F;
        }


        public void Update()
        {
            Debug.Log(AvatarStateMachine.AvatarLaneState);

            if (_characterController.isGrounded)
            {
                // Always move in z
                var moveSpeed = AvatarWalkSpeed*Time.deltaTime;
                _moveDirection = new Vector3(0, 0, moveSpeed);

                _avatarMovement();
                _avatarLaneState();

                _moveDirection = transform.TransformDirection(_moveDirection);
                _moveDirection *= AvatarWalkSpeed;
            }

            _setAvatarMoveStateToIdle();

            _moveDirection.y -= LevelGravity * Time.deltaTime;
            //_moveDirection.x -= LevelGravity * Time.deltaTime;
            _characterController.Move(_moveDirection * Time.deltaTime);
        }

        private void _avatarLaneState()
        {
            switch (AvatarStateMachine.AvatarLaneState)
            {
                case AvatarStateMachine.AvatarLane.Left:
                    _setMoveDirectionWithinLaneSwitch(0);
                    break;
                case AvatarStateMachine.AvatarLane.Middle:
                    _setMoveDirectionWithinLaneSwitch(1);
                    break;
                case AvatarStateMachine.AvatarLane.Right:
                    _setMoveDirectionWithinLaneSwitch(2);
                    break;
            }
        }


        // TODO: Fix this to prevent player bouncing left & right in one lane
        private void _setMoveDirectionWithinLaneSwitch(int arrayIndex)
        {
            if (gameObject.transform.position.x < AvatarLanePositionFloats[arrayIndex])
                _moveDirection.x = AvatarWalkSpeed * Time.deltaTime;
            if (gameObject.transform.position.x > AvatarLanePositionFloats[arrayIndex])
                _moveDirection.x = -(AvatarWalkSpeed * Time.deltaTime);
        }

        private void _avatarMovement()
        {
            switch (AvatarStateMachine.AvatarMoveState)
            {
                case AvatarStateMachine.AvatarMove.Jumping:
                    _moveDirection.y = AvatarJumpSpeed;
                    break;
                case AvatarStateMachine.AvatarMove.Ducking:
                    _moveDirection.y = -0.5f;
                    break;
            }
        }

        private static void _setAvatarMoveStateToIdle()
        {
            AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Idle;
        }
    }
}
