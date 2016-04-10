using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.View
{
    public class PlayerView : MonoBehaviour
    {

        private GameObject _player;
        private PlayerModel _playerModel;

        public float PlayerMoveSpeed;
        public float PlayerJumpSpeed;

        // Use this for initialization
        public void Start ()
        {
            _playerModel = Application.GetPlayerModel();
            _player = GameObject.Find("ExamplePlayer");
            _playerModel.PlayerForwardMove = 0.3f;
            _playerModel.PlayerJumpSpeed = 9.81f;
        }
	
        // Update is called once per frame
        public void Update ()
        {
            // Always RUN
            _player.transform.position = _playerModel.PlayerPositionVector3;
        }
    }
}
