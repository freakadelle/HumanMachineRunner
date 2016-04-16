using System;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class PlayerController : IController {

        private readonly PlayerModel _playerModel;
        private readonly KinectInputModel _kinectInputModel;

        public static bool IsJumping;

        double TOLERANCE = 0.01;

        public PlayerController(PlayerModel playerModel, KinectInputModel kinectInputModel)
        {
            _playerModel = playerModel;
            _kinectInputModel = kinectInputModel;
            
            IsJumping = false;
        }

        public void HandleInput()
        {
            switch (_kinectInputModel.Kinectmove)
            {
                // Moves
                case KinectInputModel.KINECTMOVE.KINNECTMOVE_IDLE:
                    _playerModel.CurrentPlayerState = PlayerModel.STATE.STATE_STANDING;
                    break;
                case KinectInputModel.KINECTMOVE.KINNECTMOVE_JUMPING:
                    _playerModel.CurrentPlayerState = PlayerModel.STATE.STATE_JUMPING;
                    break;
                case KinectInputModel.KINECTMOVE.KINNECTMOVE_DUCKING:
                    _playerModel.CurrentPlayerState = PlayerModel.STATE.STATE_DUCKING;
                    break;

                // Lanes
                case KinectInputModel.KINECTMOVE.KINECTMOVE_LANERIGHT:
                    if (_playerModel.CurrentPlayerLane != PlayerModel.LANE.LANE_RIGHT)
                    {
                        // Reset KinectMove
                        _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINNECTMOVE_IDLE;
                        _playerModel.CurrentPlayerLane++;
                    }
                    
                    break;
                case KinectInputModel.KINECTMOVE.KINNECTMOVE_LANELEFT:
                    if (_playerModel.CurrentPlayerLane != PlayerModel.LANE.LANE_LEFT)
                    {
                        // Reset KinectMove
                        _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINNECTMOVE_IDLE;
                        _playerModel.CurrentPlayerLane--;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("PlayerController " + "Wrong HandleInput value");
            }
        }
        
        // From Interface
        public void Update()
        {

            // Debug.Log(_playerModel.CurrentPlayerState);
            switch (_playerModel.CurrentPlayerState)
            {
                case PlayerModel.STATE.STATE_JUMPING:
                    StandUp();
                    MoveForward();
                    Jump();
                    break;
                case PlayerModel.STATE.STATE_DUCKING:
                    MoveForward();
                    Duck();
                    break;
                case PlayerModel.STATE.STATE_STANDING:
                    MoveForward();
                    StandUp();
                    break;
                case PlayerModel.STATE.STATE_DEAD:
                    break;
                default:
                    MoveForward();
                    break;
                //throw new ArgumentOutOfRangeException();
            }

            switch (_playerModel.CurrentPlayerLane)
            {
                case PlayerModel.LANE.LANE_LEFT:
                    
                    if(Math.Abs(_playerModel.PlayerPositionTransform.position.x - (-2f)) > TOLERANCE)
                        _playerModel.PlayerPositionTransform.Translate(Vector3.left * 1.5f * Time.deltaTime);
                    break;
                case PlayerModel.LANE.LANE_MIDDLE:
                    if (_playerModel.PlayerPositionTransform.position.x > 0)
                    {
                        if(Math.Abs(_playerModel.PlayerPositionTransform.position.x) > TOLERANCE)
                        _playerModel.PlayerPositionTransform.Translate(Vector3.left * 1.5f * Time.deltaTime);
                    }
                    if (_playerModel.PlayerPositionTransform.position.x < 0)
                    {
                        if(Math.Abs(_playerModel.PlayerPositionTransform.position.x) > TOLERANCE)
                        _playerModel.PlayerPositionTransform.Translate(Vector3.left * 1.5f * Time.deltaTime);
                    }
                    break;
                case PlayerModel.LANE.LANE_RIGHT:
                    if (_playerModel.PlayerPositionTransform.position.x < 2f)
                        _playerModel.PlayerPositionTransform.Translate(Vector3.right * 1.5f * Time.deltaTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Duck()
        {
            _playerModel.PlayerPositionTransform.transform.localScale = new Vector3(1f,0.3f,1);
            // throw new NotImplementedException();
        }

        private void StandUp()
        {
            _playerModel.PlayerPositionTransform.transform.localScale = new Vector3(1, 1, 1);
        }


        // TODO: Make this better :Q
        private void Jump()
        {
            if (IsJumping)
               _playerModel.PlayerPositionTransform.transform.position += new Vector3(0,_playerModel.PlayerJumpSpeedPerFrame, 0);
          
            if(_playerModel.PlayerPositionTransform.transform.position.y > _playerModel.PlayerMaxJumpHeight)
                IsJumping = false;

            if (!(_playerModel.PlayerPositionTransform.transform.position.y <= _playerModel.PlayerHeight) || IsJumping) return;

            // Reset Kinect State
            _kinectInputModel.Kinectmove = KinectInputModel.KINECTMOVE.KINNECTMOVE_IDLE;
        }
 
        private void MoveForward()
        {
            _playerModel.PlayerPositionTransform.Translate(Vector3.forward * _playerModel.PlayerForwardMove * Time.deltaTime);
        }
    }
}
