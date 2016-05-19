using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class Game {

    public static bool isRunning = false;
    //private static bool isPaused = false;
    private static bool isPreparing = false;
    private static bool isInitialized = false;
    public static int secondsUntilGameStart = 4;
    private static DateTime startTime;
    private static DateTime startPrepareTime;
    private static DateTime actTime;

    private static Text elapsedTimeView = GameObject.Find("ElapsedTime").GetComponent<Text>();
    private static Text highscoreView = GameObject.Find("Highscore").GetComponent<Text>();
    private static Text warningView = GameObject.Find("Init").GetComponent<Text>();

    public static Player player;

    static Game()
    {
        Debug.Log("-- GAME PRE-INITIALIZE --");
        warningView.enabled = true;
        elapsedTimeView.enabled = false;
        highscoreView.enabled = false;

        warningView.text = "Waiting for player!";
    }

    public static void update()
    {
        actTime = DateTime.Now;

        if (isRunning)
        {
            TimeSpan t = actTime - startTime;
            player.score(13);

            elapsedTimeView.text = t.Seconds.ToString("##") + ":" + t.Milliseconds.ToString("##");
            highscoreView.text = player.highscore + " Pt.";
        }
        else if (isPreparing)
        {
            int t = secondsUntilGameStart - (actTime.Second - startPrepareTime.Second);

            if (t <= 0)
            {
                warningView.enabled = false;
                elapsedTimeView.enabled = true;
                highscoreView.enabled = true;
                isPreparing = false;
                isRunning = true;

                startTime = DateTime.Now.AddSeconds(-1);
            }
            else
            {
                warningView.text = t.ToString();
            }
        }
    }

    public static void init()
    {
        //Todo: Method implementation. Player positioned correctly. General resets. etc.

        player = new Player();

        isInitialized = true;
        Debug.Log("-- GAME INITIALIZED --");
    }

    private static void prepare(bool _value)
    {
        isPreparing = _value;
        startPrepareTime = DateTime.Now;
    }

    public static void pause(bool _value, bool _prepare = true)
    {
        prepare(_prepare);
        isRunning = !_value && !_prepare;
        //isPaused = _value;
        Debug.Log("-- GAME PAUSED: " + _value + " --");
    }

    public static void start(bool _prepare = true)
    {
        if (isInitialized)
        {
            pause(false);
            prepare(_prepare);
        }
        else if (!isInitialized)
        {
            init();
            startTime = DateTime.Now;
            prepare(_prepare);
            isRunning = !_prepare;
            Debug.Log("-- GAME STARTED --");
        }
        else
        {
            Debug.Log("-- GAME UNABLE TO START. NOT YET INITIALIZED --");
        }
    }

    public static void stop()
    {
        startTime = new DateTime();
        actTime = new DateTime();
        isRunning = false;
        isInitialized = false;

        Debug.Log("-- GAME STOPPED --");
    }

    public static void restart()
    {
        stop();
        //TODO: Method implementation
    }

    public static void loose()
    {
        stop();
        Debug.Log("-- GAME LOOSE --");
        //TODO: Method implementation    
    }

    public static void win()
    {
        stop();
        Debug.Log("-- GAME WON --");
        //TODO: Method implementation    
    }

}
