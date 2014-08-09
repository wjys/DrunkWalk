using UnityEngine;
using System.Collections;

public class EndScreen : MonoBehaviour {

	// sound stuff 
	public AudioClip[] clips;  
	private bool soundPlayed; 
	public bool getMoves;
	public UniMoveController[] moves;
	public UniMoveController[] pairedMoves;
	
	// Use this for initialization
	void Start () {
		soundPlayed = false; 
	}
	
	// Update is called once per frame
	void Update () {
	
		if (GameManager.ins.status != GameState.GameStatus.End){
			this.enabled = false;
		}
		/*if (!soundPlayed) {
			if (Application.loadedLevelName.Equals("Lost")) {
				audio.clip = clips[Random.Range (0,4)];
				audio.Play();
			}
			if (Application.loadedLevelName.Equals ("Won")) {
				audio.clip = clips[Random.Range (4,7)];
				audio.Play();
			}
			soundPlayed = true; 
		}*/

		if (Input.anyKey){
			Application.LoadLevel ("Splash"); 
			//Destroy (GameObject.Find ("GameState"));
			//Destroy (this.gameObject);
			this.enabled = false;
		}
	}

	private void findMoves(){
		moves = gameObject.GetComponents<UniMoveController>();
		getMoves = true;
	}
}
