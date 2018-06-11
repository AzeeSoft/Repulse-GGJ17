using UnityEngine;
using System.Collections;

public class DieAfterSeconds : MonoBehaviour {
	public float aliveSeconds=1.05f;
	// Use this for initialization
	void Start () {
		StartCoroutine (WaitAndDie());
	}

	IEnumerator WaitAndDie(){
		yield return new WaitForSeconds (aliveSeconds);
		Destroy (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
