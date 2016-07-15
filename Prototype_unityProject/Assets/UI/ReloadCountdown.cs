using UnityEngine;
using System.Collections;
using System.Globalization;
using UnityEngine.UI;

public class ReloadCountdown : MonoBehaviour
{

    private float _countdown;
    private Text _text;
    
    // Use this for initialization
    void Start()
    {
        _countdown = Time.time + 5;
        _text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        var timeLeft = _countdown - Time.time;
        timeLeft = (int) timeLeft;
        if (timeLeft < 0) timeLeft = 0;
        _text.text = timeLeft.ToString(CultureInfo.CurrentCulture);
    }
}
