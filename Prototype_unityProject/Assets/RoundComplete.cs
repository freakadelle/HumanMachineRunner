using UnityEngine;
using Assets.Scripts;

public class RoundComplete : MonoBehaviour
{

    private GameObject _avatar;

    void Start()
    {
        _avatar = GameObject.Find("Avatar");
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.gameObject.Equals(_avatar))
        {
            RespawnBatteries();
        }
    }

    private static void RespawnBatteries()
    {
        Destroy(GameObject.Find("Batteries"));
        Instantiate(Resources.Load("Batteries"));
        Debug.Log("Round complete");
    }

}
