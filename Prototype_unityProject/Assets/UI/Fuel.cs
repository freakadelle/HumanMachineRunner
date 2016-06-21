using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fuel : MonoBehaviour
{

    private Scrollbar _scrollbar;
    private float _time;
    private bool _isCoroutineStarted = false;
    private int _timeToLooseFuel;
    public static float GetFuel { get; private set; }
    // Use this for initialization
    void Start()
    {
        _scrollbar = GetComponent<Scrollbar>();
        _scrollbar.size = 1f;
        GetFuel = _scrollbar.size;
        StartCoroutine(ReduceFuel());
    }

    //Game restart purpose
    void OnLevelWasLoaded(int level)
    {
        if (level.Equals(1))
        {
            _scrollbar = GetComponent<Scrollbar>();
            GetFuel = _scrollbar.size;
            _scrollbar.size = 1;
        }
    }

    public static void FillFuel()
    {
        var scrollbar = GameObject.Find("Fuelbar").GetComponent<Scrollbar>();
        var scrollbarSize = scrollbar.size + 0.1f;
        scrollbar.size = scrollbarSize;
    }

    IEnumerator ReduceFuel()
    {
        _isCoroutineStarted = true;
        while (_scrollbar.size >= 0)
        {
            yield return new WaitForSeconds(3);
            var scrollbarSize = _scrollbar.size - 0.1f;
            _scrollbar.size = scrollbarSize;
            GetFuel = _scrollbar.size;
        }
    }

}
