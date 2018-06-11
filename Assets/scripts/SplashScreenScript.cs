using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashScreenScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (WaitAndContinue ());
	}

	IEnumerator WaitAndContinue(){
		yield return new WaitForSeconds (2f);
		SceneManager.LoadScene ("MainMenu");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
