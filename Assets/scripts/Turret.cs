using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

	public GameObject bullet;
	public Transform bulletSpawn;

	public bool facingRight=true;
	public float minPlayerDistance=10f;

	bool shooting=false;
	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate () {
		Vector2 direction = Vector2.right;
		if (!facingRight) {
			direction = Vector2.left;
		}
		RaycastHit2D playerRaycast = Physics2D.Raycast (bulletSpawn.position, direction);

		if (!shooting && playerRaycast.collider != null) {
			if (playerRaycast.collider.CompareTag ("Player") && playerRaycast.distance <= minPlayerDistance) {
				shooting = true;
				StartShooting ();
			}
		}
	}

	void StartShooting(){
		StartCoroutine (Shoot ());
	}

	IEnumerator Shoot(){
		while (shooting) {
			Debug.Log ("Instantiating bullet");

				
			GameObject gobj=(GameObject)Instantiate (bullet, new Vector2(bulletSpawn.position.x,bulletSpawn.position.y), Quaternion.identity);

			Bullet bulletObj = gobj.GetComponent <Bullet> ();
			Vector2 scale = gobj.transform.localScale;

			if (!facingRight) {
				bulletObj.speed *= -1;
				scale.x *= -1;
				gobj.transform.localScale = scale;
			}
			yield return new WaitForSeconds (1f);

		}
	}

	void Die(){
		Destroy (gameObject);
	}

	void OnCollisionEnter2D(Collision2D other){
		if (other.gameObject.CompareTag ("bullet")) {
			Die ();
		}
	}

}
