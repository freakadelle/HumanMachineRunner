using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.View
{
    public class PlayerView : MonoBehaviour
    {

        private GameObject _player;
        private PlayerModel _playerModel;

        public float PlayerMoveSpeed;
        public float PlayerJumpSpeedPerFrame;
        public float PlayerMaxJumpHeight; // TODO: Set Gravitation in Level Model
        public float PlayerHeight;

        // Use this for initialization
        public void Start ()
        {
            _playerModel = Application.GetPlayerModel();
            _playerModel.PlayerForwardMove = PlayerMoveSpeed;
            _playerModel.PlayerJumpSpeedPerFrame = PlayerJumpSpeedPerFrame;
            _playerModel.PlayerMaxJumpHeight = PlayerMaxJumpHeight;
            _playerModel.CurrentPlayerState = PlayerModel.STATE.STATE_STANDING;
            // _playerModel.PlayerPositionTransform.position = Vector3.zero;
            _playerModel.PlayerHeight = PlayerHeight;
         //   _playerModel.PlayerCharacterController = GetComponent<CharacterController>();
        }
	
        // Update is called once per frame
        public void Update ()
        {
            // update position
            transform.position = _playerModel.PlayerPositionTransform.position;
           
        }
    }
}
