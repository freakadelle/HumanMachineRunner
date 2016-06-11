using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class AvatarController : MonoBehaviour
    {

        public float AvatarJumpSpeed = 2f;
        public float AvatarWalkSpeed = 10f;
        public float LevelGravity = 20.0f;
        public float AvatarTurnSpeed = 20.0f;
        public float Score { get; set; }
        private float _rotationY;

        public static CharacterController CharacterController { get; private set; }
        private Vector3 _moveDirection = Vector3.zero;

        public void Start()
        {
            CharacterController = GetComponent<CharacterController>();
            // Set inital states
            _setAvatarMoveStateToIdle();
            _rotationY = 0;
         
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

                _moveDirection = transform.TransformDirection(_moveDirection);
                _moveDirection *= AvatarWalkSpeed;
            }

            if(AvatarStateMachine.AvatarMoveState != AvatarStateMachine.AvatarMove.Left && AvatarStateMachine.AvatarMoveState != AvatarStateMachine.AvatarMove.Right)
               _setAvatarMoveStateToIdle();

            _moveDirection.y -= LevelGravity * Time.deltaTime;

            CharacterController.Move(_moveDirection * Time.deltaTime);
        }

        private bool flag = true;
        
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
                    float angle = 0;
                    float currentAngle = 0;

                    if (flag)
                    {
                        flag = false;
                        currentAngle = transform.eulerAngles.y;
                    }
                    angle = Mathf.LerpAngle(currentAngle, currentAngle + 45, Time.time);
                    transform.eulerAngles = new Vector3(0, angle, 0);
                    
                    
                    Debug.Log(transform.eulerAngles.y);

                    if ((int) transform.eulerAngles.y == (int) currentAngle + 45)
                    {
                        flag = true;
                        _setAvatarMoveStateToIdle();
                    }
                       

                    break;
                case AvatarStateMachine.AvatarMove.Right:
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

     


        private void _setAvatarMoveStateToIdle()
        {
            AvatarStateMachine.AvatarMoveState = AvatarStateMachine.AvatarMove.Idle;
          

        }
    }
}
