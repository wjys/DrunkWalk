using UnityEngine;
using System.Collections;

public class Collision : MonoBehaviour {

	//Overall Score
	public static int score;

	//Hurt Sound
	public GameObject[] ouch;
	public Animator ouchAnim;

	private static bool yelling = false;

	// sound stuff 
	public AudioClip[] clips; 
	public AudioClip[] hitclips; 
	public AudioClip hitit;
	public AudioSource source;
	private bool soundPlayed; 
	private bool reachedBed; 

	public float currentSoundTime = 0.0f; 
	public float delaySound = 1.0f; 

	public float currentCollTime = 0.0f;
	public float delayCollision = 1.0f; 

	public float currentScoreTime = 0.0f;
	public float delayScore = 1.0f; 

	private bool collided;
	public bool colliding;

	//RECOIL STUFF
	public int recoilDir; 
	public bool recoiled; 
	private enum Dir { forward, right, left, back }; 
	public float recoilF; 

	// TO PREVENT CAM WOBBLE WHEN HIT A WALL
	public DrunkMovement dm;
	public DrunkForce df; 
	public Rigidbody rhead; 

	//RUMBLE
	public float rumbleAmt;

	// Use this for initialization
	void Start () {
		colliding = false;
		source.volume = 0.5f;
		score = 10000;

		soundPlayed = false; 
		reachedBed = false; 
		collided = false; 
		recoiled = false; 
	}
	
	// Update is called once per frame
	void Update () {	
		if (score <= 0) {
			Application.LoadLevel ("Lost"); 
		}
	}

	void FixedUpdate(){
		currentSoundTime += Time.deltaTime;
		if (currentSoundTime >= delaySound){
			soundPlayed = false; 
			currentSoundTime = 0.0f; 
		}

		currentCollTime += Time.deltaTime;
		if (currentCollTime >= delayCollision){
			collided = false; 
			currentCollTime = 0.0f; 
		}

		currentScoreTime += Time.deltaTime;
		if (currentScoreTime >= delayScore){
			score -= 5; 
			currentScoreTime = 0.0f; 
		}
	}

	void OnGUI () {
		//GUI.Box(new Rect(10,10,100,23), "Score: " + score);
		// if (GameOver){
		// 	GUI.Box(new Rect(670,300,100,25), "GAME OVER");
		// 	if (Input.GetKeyDown ("space")) {  
  //   			Application.LoadLevel (0);  
  // 			}  
		// }
	}

	void OnCollisionEnter (Collider col){
		if (!recoiled){
			print ("RECOILING");
			setRecoilDir(col.ClosestPointOnBounds(transform.position), transform.position);  
		}
	}

	//When colliding with something:
	void OnTriggerEnter(Collider col) {

		colliding = true;

		if (col.tag != "Trigger"){
			df.stopWobble = true; 
			if (!collided){
				//Yell();
				audio.PlayOneShot (hitit);

				dm.hitRumble = rumbleAmt;

				rhead.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;

				Debug.Log("Collision");
				//ouchAnim.SetTrigger("Ouch");
				
				//If collision is against a wall:
				if (col.tag == "Wall") {
					score -= 100;
					Debug.Log("Wall Collision - " + score);
					//df.stopWobble = true; 
				}
				else if (col.tag == "Box"){
					score -= 200;
					Debug.Log("Box Collision - " + score);
				}
				else if (col.tag == "Cabinet"){
					score -= 200;
					Debug.Log("Cabinet Collision - " + score);
				}
				else if (col.tag == "Cat"){
					score -= 250;
					Debug.Log("Cat Collision - " + score);
				}
				else if (col.tag == "Table"){
					score -= 300;
					Debug.Log("Table Collision - " + score);
				}
				else if (col.tag == "Chair"){
					score -= 100;
					Debug.Log("Chair Collision - " + score);
				}
				else if (col.tag == "Bed"){ // WIN STATE
					Application.LoadLevel("Won"); 
					reachedBed = true; 
					soundPlayed = true; 
					audio.PlayOneShot (clips[Random.Range(5, 8)]); 
				}
		/*		else if (col.tag == "Floor"){
					score -= 500;
					Debug.Log("Floor Collision - " + score);
				}*/

				//If not currently yelling:
				if (yelling == false){
					Debug.Log ("YELL");
					//Yell();
				} else if (yelling == true){
					StopAllCoroutines();
				}
				
				if (!soundPlayed && !reachedBed){
					playGrunt (clips[Random.Range(0, 5)]); 
					soundPlayed = true; 
				}
				collided = true; 
			}
		}
	}
	//When not:
	void OnTriggerExit(Collider col) {
		colliding = false;
		dm.hitRumble = 0.0f;
		Debug.Log("No Longer Colliding");
		soundPlayed = false; 
		recoiled = false; 
		df.stopWobble = false; 
	}

	private void playGrunt(AudioClip clip){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clip); 
	}

	private void setRecoilDir(Vector3 colPos, Vector3 playerPos){
		recoiled = true; 
		//print ("collision pos " + colPos + ", player pos    " + playerPos); 
		if (colPos.x > playerPos.x) {
			//print ("higher x, recoil left"); 
			recoilForce ((int) Dir.left);
		}
		else if (colPos.x < playerPos.x){
			//print ("lower x, recoil right"); 
			recoilForce ((int) Dir.right); 
		}
		else if (colPos.z > playerPos.z) {
			//print ("higher z, recoil back"); 
			recoilForce ((int) Dir.back);
		}
		else {
			//print ("lower z, recoil forward"); 
			recoilForce ((int) Dir.forward); 
		}
	}

	private void recoilForce(int direction){
		df.recoiled = true; 
		switch (direction) {
			
		case (int) Dir.forward:				//print ("moving head forward");
			rhead.AddForce (recoilF*transform.forward);  
			break;
			
		case (int) Dir.right:				//print ("moving head to the right");
			rhead.AddForce (recoilF*1.5f*transform.right); 
			break;
			
		case (int) Dir.left:				//print ("moving head to the left");
			rhead.AddForce (-recoilF*1.5f*transform.right); 
			break;
			
		case (int) Dir.back:				//print ("stopping head movement");
			rhead.AddForce (-recoilF*transform.forward); 
			//rhead.position = new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z); 
			break; 
			
		default:
			break; 
		}
		print ("RECOILED!"); 
	}

	/*
	//Instantiate a hurt sound
	 public void Yell () {
		GameObject newOuch = Instantiate(ouch[0], transform.position + (0.8f * Vector3.up) + (2.5f * Vector3.forward) + (0.5f * Vector3.right), Quaternion.identity) as GameObject;
		this.StartCoroutine(this.DestroyYell(newOuch, 1.0f));
	}

	//Destroy the hurt sound
	IEnumerator DestroyYell(GameObject _yell, float delay) {
	 	yield return new WaitForSeconds(delay);
	 	Destroy(_yell);
	 	yelling = true;
	 }
*/
}
