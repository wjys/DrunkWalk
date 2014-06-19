using UnityEngine;
using System.Collections;

public class Collision : MonoBehaviour {

	//Overall Score
	public static int score;

	//Hurt Sound
	public GameObject ouch;
	public Animator ouchAnim;

	private static bool yelling = false;

	// sound stuff
	private AudioSource source; 
	private AudioClip[] clips; 
	public int numClips = 1; 
	public string[] sound; 
	public float soundDelay; 
	public float soundVolume = 1.0f;
	private bool soundPlayed; 


	// Use this for initialization
	void Start () {
		score = 10000;


		source = GetComponent<AudioSource>(); 
		
		clips = new AudioClip[numClips];
		for (int i = 0; i < numClips; i++) {
			clips [i] = (AudioClip)Resources.Load ("Sounds/SFX/Insults" + sound [i]); 
		}
		source.volume = soundVolume;
		source.loop = false; 
		soundPlayed = false; 
	}
	
	// Update is called once per frame
	void Update () {
		score--; 


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

		// play pain sound
		switchGrunt ();
		if (!soundPlayed){
			source.Play (); 
			soundPlayed = true; 
		}
		StartCoroutine (stopSound ()); 
	
		Debug.Log("Collision");
		ouchAnim.SetTrigger("Ouch");
		
		//If collision is against a wall:
		if (col.tag == "Wall") {
			score -= 100;
			Debug.Log("Wall Collision - " + score);
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
		else if (col.tag == "Bed"){
			Application.LoadLevel (Application.loadedLevel); 
		}
/*		else if (col.tag == "Floor"){
			score -= 500;
			Debug.Log("Floor Collision - " + score);
		}*/

		//If not currently yelling:
		if (yelling == false){
			Yell();
		} else if (yelling == true){
			StopAllCoroutines();
		}

	}
	//When not:
	void OnTriggerExit(Collider col) {
		Debug.Log("No Longer Colliding");
	}

	private void switchGrunt(){
		int index = Random.Range (0, numClips); 
		source.clip = clips [index];
	}

	//Instantiate a hurt sound
	public void Yell () {
		GameObject newOuch = Instantiate(ouch, transform.position + (0.8f * Vector3.up) + (2.5f * Vector3.forward) + (0.5f * Vector3.right), Quaternion.identity) as GameObject;
		this.StartCoroutine(this.DestroyYell(newOuch, 1.0f));
	}

	//Destroy the hurt sound
	IEnumerator DestroyYell(GameObject _yell, float delay) {
		yield return new WaitForSeconds(delay);
		Destroy(_yell);
		yelling = true;
	}

	IEnumerator stopSound(){
		yield return new WaitForSeconds(soundDelay);
		soundPlayed = false; 
	}
}
