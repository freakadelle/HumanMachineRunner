using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Model
{
    public class PlayerModel
    {
        public enum STATE
        {
            STATE_STANDING,
            STATE_JUMPING,
            STATE_DUCKING,
            STATE_DEAD
        }

        public enum LANE
        {
            LANE_LEFT,
            LANE_MIDDLE,
            LANE_RIGHT
        }
        
        public PlayerModel(STATE currentPlayerState, LANE currentPlayerLane, Vector3 playerPositionVector3)
        {
            CurrentPlayerState = currentPlayerState;
            CurrentPlayerLane = currentPlayerLane;
            PlayerPositionVector3 = playerPositionVector3;
        }

        public STATE CurrentPlayerState { get; set; }
        public LANE CurrentPlayerLane { get; set; }

        public Vector3 PlayerPositionVector3 { get; set; }

        public float PlayerForwardMove { get; set; }
        public float PlayerJumpSpeed { get; set; }

    }
}
