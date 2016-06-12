using UnityEngine;
using UnityEngine.UI;

public class WeightOverload : MonoBehaviour
{

    private Image _weightOverload;
    private GameObject _child;

    // Use this for initialization
    void Start()
    {
        _child = gameObject.transform.GetChild(0).gameObject;
        _child.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //ToDo: if(Kinect.Multiplesources && _child.activeSelf.Equals(false))
        if (Input.GetKeyDown(KeyCode.C) && _child.activeSelf.Equals(false))
        {
            _child.SetActive(true);
        }
        //ToDo: if(Kinect.Multiplesources && _child.activeSelf.Equals(true))
        else if (Input.GetKeyDown(KeyCode.C) && _child.activeSelf.Equals(true))
        {
            _child.SetActive(false);
        }
    }
}
