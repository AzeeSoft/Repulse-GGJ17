using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public GameObject player;

	float xDiff=0f;
	float yDiff=0f;

	Rigidbody2D rb2dPlayer;

	// Use this for initialization
	void Start () {
		rb2dPlayer = player.GetComponent <Rigidbody2D> ();
		xDiff = transform.position.x - rb2dPlayer.transform.position.x;
		yDiff = transform.position.y - rb2dPlayer.transform.position.y;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 copyPosition = transform.position;
		copyPosition.x = rb2dPlayer.transform.position.x;
		transform.position = copyPosition;
	}
}
