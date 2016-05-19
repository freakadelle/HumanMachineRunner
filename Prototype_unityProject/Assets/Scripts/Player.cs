using UnityEngine;
using System.Collections;

public class Player
{

    public double highscore;
    public float fuel;

    public Player()
    {
        fuel = 100.0f;
        highscore = 0;
    }

    public void score(double _amount)
    {
        highscore += _amount;
    }

    public void useFuel(float _amount)
    {
        fuel += _amount;
    }
}
