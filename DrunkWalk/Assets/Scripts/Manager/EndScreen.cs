using UnityEngine;
using System.Collections;

public class EndScreen : MonoBehaviour {

	// sound stuff 
	public AudioClip[] clips;  
	private bool soundPlayed; 
	
	// Use this for initialization
	void Start () {
		soundPlayed = false; 
	}
	
	// Update is called once per frame
	void Update () {
		if (!soundPlayed) {
			if (Application.loadedLevelName.Equals("Lost")) {
				audio.clip = clips[Random.Range (0,4)];
				audio.Play();
			}
			if (Application.loadedLevelName.Equals ("Won")) {
				audio.clip = clips[Random.Range (4,7)];
				audio.Play();
			}
			soundPlayed = true; 
		}
		
		if (Input.anyKey)
			GameManager.ins.status = GameState.GameStatus.Splash;
			Application.LoadLevel ("Splash"); 
	}
}
