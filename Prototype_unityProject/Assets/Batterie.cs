﻿using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Batterie : MonoBehaviour
{

    private GameObject _avatar;
    private bool _isTriggered;

    // Use this for initialization
    void Start()
    {
        _avatar = GameObject.Find("Avatar");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right * 50 * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit) && col.transform.gameObject.Equals(_avatar))
        {

            Fuel.FillFuel();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.transform.gameObject.Equals(_avatar))
        {
            Destroy(gameObject);
        }
    }
}
