using UnityEngine;
using System.Collections;

// TODO: Remove this class, is it really necessary? 
public class GameControlPanel : MonoBehaviour
{

    public bool isRunning;

    [SerializeField(), Range(0f, 10f)]
    public int secondsUntilGameStart;

    [SerializeField(), Range(10f, 1000f)]
    public float playerFuel;


    //void Start()
    //{
    //    Game.secondsUntilGameStart = secondsUntilGameStart;
    //    isRunning = Game.isRunning;
    //    playerFuel = Game.player.fuel;
    //}

}
