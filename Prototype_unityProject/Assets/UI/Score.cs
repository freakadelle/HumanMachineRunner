using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class Score : MonoBehaviour
{

    private Text _score;
    private float _time;

    void Start()
    {
        _score = GetComponent<Text>();
    }

    void Update()
    {
        _score.text = StopWatch();
    }

    private string StopWatch()
    {
        _time += Time.deltaTime;

        var minutes = _time / 60;       //Divide the guiTime by sixty to get the minutes.
        var seconds = _time % 60;       //Use the euclidean division for the seconds.
        var fraction = (_time * 100) % 100;

        var timePassed = string.Format("{0:00} : {1:00} : {2:000}", minutes, seconds, fraction);
        return timePassed;
    }
}
