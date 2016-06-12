using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class Score : MonoBehaviour
{

    private Text _score;
    private int _count;

    void Start()
    {
        _score = GetComponent<Text>();
    }

	// Update is called once per frame
	void Update ()
	{
        //Todo: if(collected)
	    _count += 1;
	    _score.text = _count.ToString();
	}
}
