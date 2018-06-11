using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LevelController : MonoBehaviour {

	public static DateTime prevTime;
	public static TimeSpan totalTime;

	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll ();

		if (!PlayerPrefs.HasKey ("time1")) {
			PlayerPrefs.SetFloat ("time1",Mathf.Infinity);
			PlayerPrefs.SetString ("name1","");
		}
		if (!PlayerPrefs.HasKey ("time2")) {
			PlayerPrefs.SetFloat ("time2",Mathf.Infinity);
			PlayerPrefs.SetString ("name2","");
		}
		if (!PlayerPrefs.HasKey ("time3")) {
			PlayerPrefs.SetFloat ("time3",Mathf.Infinity);
			PlayerPrefs.SetString ("name3","");
		}

		PlayerPrefs.Save ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Restart(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void GoHome(){
		SceneManager.LoadScene ("MainMenu");
	}

	public void SaveScore(){

	}
}
