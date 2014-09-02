using UnityEngine;
using System.Collections;

//THE COLLISION MANAGER
//ALSO MANAGES SCORING SYSTEM FOR SINGLE PLAYER SCOREATTACK & STEALTH

public class Collision : MonoBehaviour {

	////////////////
	//SCORE ATTACK
	////////////////

	// Overall Score
	public int score;

	// Score Delay
	private float currentScoreTime = 0.0f;
	public float delayScore = 1.0f; 

	///////////
	//STEALTH
	///////////
	
	// Parents Awareness Level 
	public static int parents;

	// Stealth Bar Gameobject
	public StealthBar stealthBar;

	// Delay before parents decrease
	private float currentPTime = 0.0f;
	public float delayP = 2.0f;

	//Lerp the Stealth Bar Level
	public float smooth;

	///////////////////////////////////////////////////////////////////	

	// Player Components
	public DrunkMovement dm;
	public DrunkForce df; 
	public Rigidbody rhead; 
	public UniMoveController move;

	// Sounds
	public Sounds sounds;


	public AudioClip[] hitclips; 
	public AudioClip hitit;
	public AudioSource source;
	private bool soundPlayed; 
	public bool reachedBed; 
	public bool reachedCouch;
	public bool reachedTub;

	// Sound Delay
	private float currentSoundTime = 0.0f; 
	public float delaySound = 1.0f;

	// Collision Bools
	private bool collided;
	public bool colliding;

	// Collision Delay
	private float currentCollTime = 0.0f;
	public float delayCollision = 1.0f; 

	//Recoil
	public int recoilDir; 
	public bool recoiled; 
	private enum Dir { forward, right, left, back }; 
	public float recoilF; 

	// Rumble
	public float rumbleAmt;

	//NOT USED//////////////////////////////////////////
	// Hurt Sound Obj
	public GameObject[] ouch;
	public Animator ouchAnim;
	private static bool yelling = false;


	void Start () {
		if (GameManager.ins.controller == GameState.GameController.move)
			move = dm.GetComponent<UniMoveController>();

		//NOT COLLIDING
		colliding = false;
		collided = false; 

		//NOT RECOILING
		recoiled = false; 

		//SOUND SETUP
		source.volume = 0.5f;
		soundPlayed = false; 

		//INITIAL SCORE
		score = 10000;

		//INITIAL PARENTS LEVEL
		parents = 0;

		//REACHED BED
		reachedBed = false; 

		//GET STEALTH BAR
		stealthBar = GameObject.Find ("UICam " + dm.id).GetComponentInChildren<StealthBar> ();


		if (GameManager.ins.mode == GameState.GameMode.Stealth) {
			GameObject.Find ("StealthBar").GetComponentInChildren<SpriteRenderer>().enabled = true;
				} else {
			GameObject.Find ("StealthBar").GetComponentInChildren<SpriteRenderer>().enabled = false;
				}

		rhead.maxAngularVelocity = 10;
	}
	
	// Update is called once per frame
	void Update () {
		GameManager.ins.score = score;

		if (score <= 0){
			score = 0;
		}

		Debug.Log ("" + parents);
		//SCORE ATTACK
		if (GameManager.ins.mode == GameState.GameMode.ScoreAttack){
			if (score <= 0) {
				//LOSE
				Debug.Log ("SCORE = 0");
			}
		}
		//STEALTH
		else if (GameManager.ins.mode == GameState.GameMode.Stealth){
			if (parents >= 100){
				//LOSE
				Debug.Log ("PARENTS WOKE UP");
			} else if (parents < 100 && parents > 0 && !collided){
				currentScoreTime += Time.deltaTime;
				if (currentScoreTime >= delayScore){
					parents --;
					currentScoreTime = 0.0f;
				}
			}
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

		score --;

		if (GameManager.ins.mode == GameState.GameMode.Stealth){
			stealthBar.StealthIcons[2].transform.position = Vector3.Lerp (stealthBar.StealthIcons[2].transform.position, 
			                                                              new Vector3 (stealthBar.StealthIcons[2].transform.position.x, (parents * 0.0625f)-3.05f, stealthBar.StealthIcons[2].transform.position.z), smooth * Time.deltaTime);
			if (stealthBar.StealthIcons[2].transform.position.y >= 2.72f){
				stealthBar.StealthIcons[2].transform.position = new Vector3(stealthBar.StealthIcons[2].transform.position.x, 2.72f, stealthBar.StealthIcons[2].transform.position.z);
			}
		}

	}

	void OnGUI () {
		if (GameManager.ins.mode == GameState.GameMode.ScoreAttack){
			GUI.Box(new Rect(10,10,100,23), "Score: " + score);
		}
	}

	//When colliding with something:
	void OnTriggerEnter(Collider col) {

		colliding = true;
		dm.colliding = true;
		if (GameManager.ins.controller == GameState.GameController.move)
			move.SetLED(Color.red);

		//IF THE THING IS SOLID
		if (col.tag != "Trigger"){

			//RECOIL
			if (!recoiled){

				//IF THE THING IS A WALL OR A BIG OBJECT
				if (col.tag == "Wall" || col.tag == "Furniture"){
					print ("RECOIL");
					setRecoilDir(col.ClosestPointOnBounds(transform.position), transform.position);
					recoiled = true;
				}

				if (col.tag == "Furniture"){
					sounds.furnitureCollision = true;
				}

				if (col.tag == "Wall"){
					sounds.wallCollision = true;
				}
            }

			//STOP WOBBLE
			df.stopWobble = true; 

			//COLLIDED
			if (!collided){

				//PLAY HITTING SOUND
				audio.PlayOneShot (hitit);

				//RUMBLE
				dm.hitRumble = rumbleAmt;

				//CONSTRAIN RIGIDBODY
				rhead.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;

				//WIN STATE
				if (col.tag == "Bed"){
					//Application.LoadLevel("Won");
					Debug.Log ("REACHING BED?");
					reachedBed = true; 
					soundPlayed = true; 
					//audio.PlayOneShot (clips[Random.Range(5, 8)]); 
                }

				if (col.tag == "Couch"){
					reachedCouch = true;
				}

				if (col.tag == "Tub"){
					reachedCouch = true;
				}

				if (GameManager.ins.mode == GameState.GameMode.Stealth){
					stealthBar.StealthIcons[1].GetComponent<SpriteRenderer>().sprite = stealthBar.noiseIcons[1];
				}

				//If collision is against a wall:
				if (col.tag == "Wall") {
					score -= 100;
					parents += 50;
					//TAKE RECOIL DIR, FIND THE OPPOSITE, THEN PLAY ARM ANIMATION ACCORDINGLY
					//Debug.Log("Wall Collision - " + score);
				}
				else if (col.tag == "Box"){
					score -= 200;
					//Debug.Log("Box Collision - " + score);
				}
				else if (col.tag == "Cabinet"){
					score -= 200;
					//Debug.Log("Cabinet Collision - " + score);
				}
				else if (col.tag == "Cat"){
					score -= 250;
					//Debug.Log("Cat Collision - " + score);
				}
				else if (col.tag == "Table"){
					score -= 300;
					//Debug.Log("Table Collision - " + score);
				}
				else if (col.tag == "Chair"){
					score -= 100;
					//Debug.Log("Chair Collision - " + score);
				}

				//If not currently yelling:
				if (yelling == false){
					//Debug.Log ("YELL");
					//Yell();
				} else if (yelling == true){
					StopAllCoroutines();
				}
				
				if (!soundPlayed && !reachedBed){
					//playGrunt (clips[Random.Range(0, 5)]); 
					soundPlayed = true; 
				}
				collided = true; 
			}
		}
	}
	//When not:
	void OnTriggerExit(Collider col) {
		dm.colliding = false;
		colliding = false;
		dm.hitRumble = 0.0f;
		soundPlayed = false; 
		recoiled = false; 
		df.stopWobble = false; 

		reachedBed = false;
		reachedCouch = false;
		reachedTub = false;


		if (GameManager.ins.mode == GameState.GameMode.Stealth){
			stealthBar.StealthIcons[1].GetComponent<SpriteRenderer>().sprite = stealthBar.noiseIcons[0];
        }
    }
    
	//PLAY GRUNT
	private void playGrunt(AudioClip clip){
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clip); 
	}

	//SET THE DIRECTION OF RECOIL
	private void setRecoilDir(Vector3 colPos, Vector3 playerPos){ 
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

	//SET THE FORCE WITH WHICH TO RECOIL
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
