using UnityEngine;
using System.Collections;

public class ReturnGame : MonoBehaviour {

	// sound stuff 
	public AudioClip[] clips;  

	// Use this for initialization
	void Start () {
		if (Application.loadedLevelName.Equals("Lost")) {
			audio.PlayOneShot(clips[Random.Range (0,4)]);
		}
		if (Application.loadedLevelName.Equals ("Won")) {
			audio.PlayOneShot(clips[Random.Range (4,7)]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey)
			Application.LoadLevel ("WastedNight"); 
	}
}
