using UnityEngine;
using System.Collections;

public class EnterGame : MonoBehaviour {

	// sound stuff 
	public AudioClip[] clips;  

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey)
			Application.LoadLevel ("WastedMove"); 
	}
}
