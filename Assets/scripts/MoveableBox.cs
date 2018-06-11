using UnityEngine;
using System.Collections;

public class MoveableBox : MonoBehaviour {

	bool buried=false, buriedFixed=false;
	public float buryYPosition=-4f;

	public Transform GroundCheck;

	Rigidbody2D rb2d;
	// Use this for initialization
	void Start () {
		rb2d = GetComponent <Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		RaycastHit2D repulseRaycast = Physics2D.Raycast (GroundCheck.position, Vector2.down);

		if (buried) {
			Bury (repulseRaycast);
		}
	}

	void Bury(RaycastHit2D repulseRaycast){
		if (repulseRaycast.collider == null || repulseRaycast.distance > 0) {
			Vector2 newPosition = gameObject.transform.position;
			newPosition.y -= 0.1f;
			gameObject.transform.position = newPosition;
		} else {
			Vector2 newPosition = gameObject.transform.position;
			newPosition.y = Mathf.Round (newPosition.y);
			gameObject.transform.position = newPosition;

			rb2d.constraints = RigidbodyConstraints2D.FreezePositionY;
			Vector2 vel = rb2d.velocity;
			vel.x = 0;
			rb2d.velocity = vel;
		}
	}

	void OnCollisionEnter2D(Collision2D other){
		if(other.collider.CompareTag ("moveableBoxBlocker")){
			//Vector2 newPosition = gameObject.transform.position;
			//newPosition.y = -4;

			buried = true;


			//gameObject.transform.position = newPosition;
			//Vector2.Lerp (gameObject.transform.position,newPosition,2f);
		} else if(other.collider.CompareTag ("Player")){
				Vector2 velocity = rb2d.velocity;
				velocity.x = 0;
				rb2d.velocity = velocity;
		}
	}
}
