using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Highscore : MonoBehaviour {

    private Text _highScore;

    void OnLevelWasLoaded(int level)
    {
        if (level.Equals(1))
        {
            _highScore = GetComponent<Text>();
            //TODO: _highScore.text = Avatar.GetHighScore();
        }
    }

    void Start()
    {
        _highScore = GetComponent<Text>();
        //TODO._highScore.text = Avatar.GetHighScore();
    }
}
