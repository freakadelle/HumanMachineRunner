using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeightOverload : MonoBehaviour {

    private Text _weightOverload;

    // Use this for initialization
    void Start () {
        _weightOverload = GetComponent<Text>();
        _weightOverload.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.C) && _weightOverload.enabled.Equals(false))
        {
            _weightOverload.enabled = true;
	    }
        else if (Input.GetKey(KeyCode.C) && _weightOverload.enabled.Equals(true))
        {
            _weightOverload.enabled = false;
        }
	}
}
