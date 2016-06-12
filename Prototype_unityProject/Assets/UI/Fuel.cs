using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fuel : MonoBehaviour
{

    private Scrollbar _scrollbar;
    private float _time;
    private bool _isCoroutineStarted = false;
    private int _timeToLooseFuel;

    // Use this for initialization
    void Start()
    {
        _scrollbar = GetComponent<Scrollbar>();
        _scrollbar.size = 1;
        
        StartCoroutine(ReduceFuel());
    }

    //Game restart purpose
    void OnLevelWasLoaded(int level)
    {
        if (level.Equals(1))
        {
            _scrollbar = GetComponent<Scrollbar>();
            _scrollbar.size = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Todo: if(Avatar.CollectedFuel.Equals(true))
        FillFuel();

        
    }

    private void FillFuel()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            var scrollbarSize = _scrollbar.size + 0.1f;
            _scrollbar.size = scrollbarSize;
        }
    }


    IEnumerator ReduceFuel()
    {
        _isCoroutineStarted = true;
        while (_scrollbar.size >= 0)
        {
            yield return new WaitForSeconds(5);
            var scrollbarSize = _scrollbar.size - 0.1f;
            _scrollbar.size = scrollbarSize;
        }
    }
    
}
