using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public GameObject explosion;

	public float speed=3f;

	Rigidbody2D rb2d;
	// Use this for initialization
	void Start () {
		Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Bullet"), LayerMask.NameToLayer ("Ignore Raycast"));

		rb2d = GetComponent <Rigidbody2D> ();
		Vector2 vel=rb2d.velocity;
		vel.x = speed;
		vel.y = 0f;
		rb2d.velocity = vel;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D other){
		Debug.Log ("Collision Detected");
		Destroy (gameObject);
		Instantiate (explosion, transform.position, Quaternion.identity);
		/*if (other.gameObject.CompareTag ("Player")) {
			Player player = other.gameObject.GetComponent <Player> ();
			player.Die ();
		}*/
	}
}
