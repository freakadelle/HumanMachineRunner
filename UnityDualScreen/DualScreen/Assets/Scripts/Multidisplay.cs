using UnityEngine;
using System.Collections;

public class Multidisplay : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log(Display.displays.Length);
        if (Display.displays.Length.Equals(2))
            Display.displays[1].Activate();
    }
}
	

