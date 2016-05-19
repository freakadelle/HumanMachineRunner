using UnityEngine;

// TODO: Delete this - avatar exists as prefab!
// TODO: Needed vars can be re factored into avatar script
public class Player
{

    public double highscore;
    public float fuel;

    public Transform playTransform;

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
