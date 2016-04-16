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
        
        public PlayerModel(STATE currentPlayerState, LANE currentPlayerLane, Transform playerPositionTransform)
        {
            CurrentPlayerState = currentPlayerState;
            CurrentPlayerLane = currentPlayerLane;
            PlayerPositionTransform = playerPositionTransform;
        }

        public STATE CurrentPlayerState { get; set; }
        public LANE CurrentPlayerLane { get; set; }

        public Transform PlayerPositionTransform { get; set; }
        public float PlayerForwardMove { get; set; }
        public float PlayerJumpSpeedPerFrame { get; set; }
        public float PlayerMaxJumpHeight { get; set; }
        public float PlayerHeight { get; set; }
        
      
    }
}
