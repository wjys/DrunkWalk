using UnityEngine;
using System.Collections;

public class Collision : MonoBehaviour {

	//Overall Score
	public static int score;

	//Hurt Sound
	//public GameObject ouch;
	//public Animator ouchAnim;

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

	// TO PREVENT CAM WOBBLE WHEN HIT A WALL
	public TopplingForce tf; 

	// Use this for initialization
	void Start () {
		source.volume = 0.5f;
		score = 10000;

		soundPlayed = false; 
		reachedBed = false; 
		collided = false; 
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
		GUI.Box(new Rect(10,10,100,23), "Score: " + score);
		// if (GameOver){
		// 	GUI.Box(new Rect(670,300,100,25), "GAME OVER");
		// 	if (Input.GetKeyDown ("space")) {  
  //   			Application.LoadLevel (0);  
  // 			}  
		// }
	}

	//When colliding with something:
	void OnTriggerEnter(Collider col) {

		if (!collided){
			audio.PlayOneShot (hitit);

			Debug.Log("Collision");
			//ouchAnim.SetTrigger("Ouch");
			
			//If collision is against a wall:
			if (col.tag == "Wall") {
				score -= 100;
				Debug.Log("Wall Collision - " + score);
				tf.hitWall = true; 
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
				// Yell();
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
	//When not:
	void OnTriggerExit(Collider col) {
		Debug.Log("No Longer Colliding");
		soundPlayed = false; 
		tf.hitWall = false; 
	}

	private void playGrunt(AudioClip clip){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clip); 
		
	}

	//Instantiate a hurt sound
	// public void Yell () {
		//GameObject newOuch = Instantiate(ouch, transform.position + (0.8f * Vector3.up) + (2.5f * Vector3.forward) + (0.5f * Vector3.right), Quaternion.identity) as GameObject;
		//this.StartCoroutine(this.DestroyYell(newOuch, 1.0f));
	// }

	//Destroy the hurt sound
	//IEnumerator DestroyYell(GameObject _yell, float delay) {
	// 	yield return new WaitForSeconds(delay);
	// 	Destroy(_yell);
	// 	yelling = true;
	// }

}
