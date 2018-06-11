using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Player : MonoBehaviour {

	public float repulsableDistance=10f;
	public float repulsableBulletDistance=10f;
	public float minMoveBoxDistance=2f;
	public float repulseSpeed=10f;
	public float repulseForce=10f;
	public float playerSpeed=10f;
	public float playerJumpForce=10f;
	public float delayBetweenPulses=500f;
	public bool facingLeft = true;

	public GameObject repulseEmitterPoint, forwardPoint;
	public GameObject groundCheck, groundCheck1;
	public GameObject floorCheck, floorCheck1;
	public GameObject greenFlash, repulseWaves;
	public GameObject ScoreSubmitScreen;

	public SpriteRenderer teleportAnimation;

	public Slider slider;
	public Text elapsedTimeText, scoreText, currentHighScore;
	public InputField nameField;

	public LevelController levelController;

	Rigidbody2D rb2dPlayer;
	Animator animator;
	GameObject repulsiveWaveObj=null;

	bool grounded=true, grounded1=true, jumping=false, unmoveable=false, teleporting=false;

	bool alive=true, overrideShowWaves=false, won=false, started=false;

	int lastFireTime=0;
	int repulseLevel=100;
	int playerFinalTime=0;

	int receivedScore=0;


	// Use this for initialization
	void Start () {
		rb2dPlayer = GetComponent <Rigidbody2D> ();
		animator = GetComponent <Animator> ();


		Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("Ignore Raycast"));

		teleporting = true;
		StartCoroutine (ReleaseTeleportingAfterDelay ());

		float best = PlayerPrefs.GetFloat ("time1");
		Debug.Log ("Top: "+best);
		if(best!=Mathf.Infinity)
			currentHighScore.text = "Current Best Time: " + best;
		else
			currentHighScore.text = "No Current Best Time";
	}

	IEnumerator ReleaseTeleportingAfterDelay(){
		yield return new WaitForSeconds (5.75f/2);
		teleporting = false;
		started = true;
		LevelController.prevTime = DateTime.Now;
	}

	int getTotalSeconds(TimeSpan interval){
		return interval.Days * 24 * 60 * 60 + interval.Hours * 60 * 60 + interval.Minutes * 60 + interval.Seconds;
	}

	void FixedUpdate () {

		if (!won && started) {
			LevelController.totalTime += (DateTime.Now.Subtract(LevelController.prevTime));
			LevelController.prevTime = DateTime.Now;
		}
		//elapsedTimeText.text = getTotalSeconds(LevelController.totalTime).ToString (); 
		playerFinalTime = getTotalSeconds (LevelController.totalTime);
		elapsedTimeText.text = playerFinalTime.ToString (); 

		if (!alive)
			return;
		
		float hor = Input.GetAxis ("Horizontal");
		bool fireBtnPressed = Input.GetButton ("Fire1");

		if (fireBtnPressed) {
			repulseLevel -= 3;
			if (repulseLevel <= 0) {
				repulseLevel = 0;
				fireBtnPressed = false;
			}
		}

		if (repulseLevel < 100) {
			repulseLevel+=2;
			if (repulseLevel > 100)
				repulseLevel = 100;
		}

		if (fireBtnPressed || overrideShowWaves) {
			animator.SetBool ("repulsing", true);
			unmoveable = true;
			StartCoroutine(releaseUnmoveableAfterDelay ());
		} else {
			animator.SetBool ("repulsing", false);
			unmoveable = false;
		}


		bool jumpBtnPressed = Input.GetButton ("Jump");

		grounded = Physics2D.Linecast (transform.position, groundCheck.transform.position, 1 << LayerMask.NameToLayer ("Ground"));
		grounded1 = Physics2D.Linecast (transform.position, groundCheck1.transform.position, 1 << LayerMask.NameToLayer ("Ground"));

		Vector2 direction=Vector2.right;
		if (facingLeft) {
			direction = Vector2.left;
		}

		Vector2 playerVelocity = rb2dPlayer.velocity;
		Vector2 playerPosition = rb2dPlayer.transform.position;

		RaycastHit2D repulseRaycast = Physics2D.Raycast (repulseEmitterPoint.transform.position, direction);
		RaycastHit2D forwardRaycast = Physics2D.Raycast (forwardPoint.transform.position, direction);

		float areaHDist = repulsableBulletDistance;
		if (facingLeft) {
			areaHDist *= -1;
		}

		Collider2D[] collidersInRadius = Physics2D.OverlapAreaAll (new Vector2(transform.position.x + areaHDist,transform.position.y+repulsableBulletDistance), new Vector2(transform.position.x,transform.position.y-repulsableBulletDistance));
		

		Vector2 GCNewPos = floorCheck.transform.position;
		Vector2 GC1NewPos = floorCheck1.transform.position;
		RaycastHit2D floorCheckRaycast = Physics2D.Raycast (GCNewPos, Vector2.down);
		RaycastHit2D floorCheck1Raycast = Physics2D.Raycast (GC1NewPos,  Vector2.down);

		CheckAndFlip (hor);
		MovePlayer (hor, playerVelocity, forwardRaycast, fireBtnPressed);
		CheckAndJump (jumpBtnPressed);

		if (teleporting)
			return;
		
		CheckAndShowRepulseWaves (fireBtnPressed);
		CheckAndRepulse (fireBtnPressed, repulseRaycast, playerVelocity);
		CheckAndDeflectBullet (fireBtnPressed, collidersInRadius);
		CheckAndDrown (floorCheckRaycast, floorCheck1Raycast, playerVelocity);
		UpdateRepulseBar ();

		//Debug.Log ("Repulse Level: "+repulseLevel);
	}

	void UpdateRepulseBar(){
		slider.value = repulseLevel;
	}

	IEnumerator releaseUnmoveableAfterDelay(){
		unmoveable = false;
		yield return new WaitForSeconds (0.11f);
	}

	void MovePlayer(float hor,Vector2 playerVelocity, RaycastHit2D forwardRaycast, bool fireBtnPressed){
		playerVelocity.x = hor * playerSpeed;

		if (forwardRaycast.collider != null) {
			if (forwardRaycast.collider.gameObject.CompareTag ("moveableBox")) {
				if(!fireBtnPressed){
					if (forwardRaycast.distance <= minMoveBoxDistance) {
						playerVelocity.x = 0f;
					}
				}
			}
		}

		if (unmoveable || teleporting)
			playerVelocity.x = 0f;

		rb2dPlayer.velocity = playerVelocity;

		if (playerVelocity.x != 0 && grounded) {
			animator.SetBool ("walking",true);
		} else {
			animator.SetBool ("walking",false);
		}
	}

	void CheckAndShowRepulseWaves(bool fireBtnPressed){
		if (fireBtnPressed) {
			if (repulsiveWaveObj == null) {
				Vector2 pos = repulseEmitterPoint.transform.position;
				pos.x -= 0.4f;
				pos.y += 0.5f;
				repulsiveWaveObj = (GameObject)Instantiate (repulseWaves, repulseEmitterPoint.transform, false);
			}
				Vector2 pos1 = repulsiveWaveObj.transform.localPosition;
				pos1.x = -0.3f;
				pos1.y = 1.5f;
				repulsiveWaveObj.transform.localPosition = pos1;
		} else if(!overrideShowWaves){
			if (repulsiveWaveObj != null) {
				Destroy (repulsiveWaveObj);
				repulsiveWaveObj = null;
			}
		}
	}

	void CheckAndJump(bool jumpBtnPressed){
		if (jumpBtnPressed && (grounded || grounded1) && !teleporting) {
			rb2dPlayer.AddForce (new Vector2 (0, playerJumpForce));
			jumping = true;

			animator.SetBool ("jumping",true);
			Instantiate (greenFlash,new Vector2(groundCheck1.transform.position.x, groundCheck1.transform.position.y+0.58f),Quaternion.identity);
		} else if (grounded || grounded1) {
			jumping = false;
			animator.SetBool ("jumping",false);
		}
	}

	void CheckAndFlip(float hor){
		if (((facingLeft && hor > 0) || (!facingLeft && hor < 0)) && !teleporting) {
			Flip ();
		}
	}

	void Flip(){
		facingLeft = !facingLeft;
		Vector2 scale=gameObject.transform.localScale;
		scale.x *= -1;
		gameObject.transform.localScale = scale;
	}

	bool CheckFloorRaycast(RaycastHit2D floorCheckRaycast){
		if (floorCheckRaycast.collider != null) {
			if (floorCheckRaycast.collider.gameObject.CompareTag ("floorBottom") && !jumping) {
				return true;
			}
		}

		return false;
	}

	void CheckAndDrown(RaycastHit2D floorCheckRaycast, RaycastHit2D floorCheck1Raycast, Vector2 playerVelocity){
		if (CheckFloorRaycast (floorCheckRaycast) && CheckFloorRaycast (floorCheck1Raycast)) {
			Debug.Log ("Drown");
			playerVelocity.x = 0f;
			rb2dPlayer.velocity = playerVelocity;
		}
	}

	void CheckAndDeflectBullet(bool fireBtnPressed, Collider2D[] colliders){
		int i=0;
		if(fireBtnPressed && repulseLevel>50)
		while (i < colliders.Length) {
			if (colliders [i].CompareTag ("bullet")) {
				Rigidbody2D rb2d = colliders [i].GetComponent <Rigidbody2D> ();
				Vector2 vel = rb2d.velocity;
				Vector2 scale=rb2d.transform.localScale;
				vel.x *= -1;
				rb2d.velocity = vel;
				scale.x *= -1;
				rb2d.transform.localScale = scale;
				StartCoroutine(OverrideShowWaves ());
				repulseLevel = 0;
			}
			i++;
		}
	}

	IEnumerator OverrideShowWaves(){
		overrideShowWaves = true;
		yield return new WaitForSeconds (2.167f/3);
		overrideShowWaves = false;
	}

	void CheckAndRepulse(bool fireBtnPressed, RaycastHit2D repulseRaycast, Vector2 playerVelocity){
		if (repulseRaycast.collider != null) {
			//Debug.Log ("Hit Something: "+raycastHit2D.collider.gameObject.tag);
			if(repulseRaycast.collider.gameObject.CompareTag ("moveableBox")){
				//Debug.Log ("Hit the moveable box");
				if (fireBtnPressed) {
					//Debug.Log ("Fire Pressed");
					if (repulseRaycast.distance <= repulsableDistance) {
						GameObject moveableBox = repulseRaycast.collider.gameObject;
						Rigidbody2D rb2d = moveableBox.GetComponent <Rigidbody2D> ();
						float forceVal = repulseForce;
						if (facingLeft) {
							forceVal *= -1;
						}
						rb2d.AddForce (new Vector2 (forceVal, 0f));
					}
				} else {
					if (repulseRaycast.distance <= minMoveBoxDistance) {
						playerVelocity.x = 0f;
						rb2dPlayer.velocity = playerVelocity;
					}
				}
			}
		}
	}

	public void Die(){
		animator.SetBool ("dead",true);
		Debug.Log ("Player Dead");
		alive = false;
		started = false;
		StartCoroutine (TeleportOutAfterDelay (4f));
	}

	IEnumerator RestartAfterDelay(float sec){
		yield return new WaitForSeconds (sec);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	void OnCollisionEnter2D(Collision2D other){
		if(other.gameObject.CompareTag ("bullet") || other.gameObject.CompareTag ("spikes") ){
			Die();
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.CompareTag ("teleporterPlatform")){
			started = false;
			TeleportOut (true);
		}
	}

	IEnumerator TeleportOutAfterDelay(float sec){
		yield return new WaitForSeconds (sec);
		TeleportOut ();
	}

	void TeleportOut(bool dontGo=false){
		teleporting = true;
		animator.SetBool ("teleporting",true);
		teleportAnimation.enabled = true;
		if (!dontGo) {
			StartCoroutine (RestartAfterDelay (5.75f / 2));
			FinishedGame ();
		}else{
			StartCoroutine(ShowNameScreen ());
		}
	}

	void FinishedGame(){
		started = false;
		LevelController.totalTime = DateTime.Now.Subtract (DateTime.Now);
	}

	IEnumerator ShowNameScreen(){
		yield return new WaitForSeconds (5.75f / 2);
		scoreText.text = playerFinalTime.ToString ();
		receivedScore = playerFinalTime;
		ScoreSubmitScreen.SetActive (true);
		FinishedGame ();
	}

	public void SubmitScore(){
		Debug.Log ("Clicked");
		string yourName= nameField.text;
		float yourScore = (float)receivedScore;

		float time1 = PlayerPrefs.GetFloat ("time1");
		float time2 = PlayerPrefs.GetFloat ("time2");
		float time3 = PlayerPrefs.GetFloat ("time3");

		float[] times = new float[] {time1,time2,time3,yourScore};
		Array.Sort <float>(times);

		string name1= PlayerPrefs.GetString ("name1");
		string name2= PlayerPrefs.GetString ("name2");
		string name3= PlayerPrefs.GetString ("name3");

		Debug.Log ("Your Score: " + yourScore);
		Debug.Log ("Time 1: " + time1);
		Debug.Log ("Time 2: " + time2);
		Debug.Log ("Time 3: " + time3);


		if (times [0] == yourScore) {
			Debug.Log ("Top 1");
			time3 = time2;
			time2 = time1;
			time1 = yourScore;
		
			name3 = name2;
			name2 = name1;
			name1 = yourName;

		} else if (times [1] == yourScore) {
			Debug.Log ("Top 2");
			time3 = time2;
			time2 = yourScore;

			name3 = name2;
			name2 = yourName;

		} else if (times [2] == yourScore) {
			Debug.Log ("Top 3");
			time3 = yourScore;
			name3 = yourName;
		} else {
			Debug.Log ("Top None");
		
		}

		PlayerPrefs.SetString ("name1",name1);
		PlayerPrefs.SetString ("name2",name2);
		PlayerPrefs.SetString ("name3",name3);
		PlayerPrefs.SetFloat ("time1",time1);
		PlayerPrefs.SetFloat ("time2",time2);
		PlayerPrefs.SetFloat ("time3",time3);
		PlayerPrefs.Save ();

		Debug.Log ("Time best :"+ PlayerPrefs.GetInt ("time1"));
		Debug.Log ("Actual Time best :"+ time1);

		Time.timeScale = 1;
		playerFinalTime = 0;
		ScoreSubmitScreen.SetActive (false);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

}
