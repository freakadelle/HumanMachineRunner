using UnityEngine;
using System.Collections;

public class RotateBat : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.right *50*Time.deltaTime);
    }
}
