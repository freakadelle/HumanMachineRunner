using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelReload : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    StartCoroutine("Reload");
	}

    IEnumerator Reload()
    {
       yield return new WaitForSeconds(5);
       SceneManager.LoadScene(0);
    }

}
