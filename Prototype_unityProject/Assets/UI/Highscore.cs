using UnityEngine;
using System.Collections;
using Assets.Scripts;
using UnityEngine.UI;

public class Highscore : MonoBehaviour {

    private Text _highScore;

    void OnLevelWasLoaded(int level)
    {
        if (!level.Equals(1)) return;
        _highScore = GetComponent<Text>();
        _highScore.text = Game.GetHighScore;
    }

    void Start()
    {
        _highScore = GetComponent<Text>();
        _highScore.text = Game.GetHighScore;
    }
}
