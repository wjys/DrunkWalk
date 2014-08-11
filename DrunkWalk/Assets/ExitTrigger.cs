using UnityEngine;
using System.Collections;

public class ExitTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter(Collider other){
		if (other.tag == "Me"){
			Application.LoadLevel ("Splash"); 
			Destroy (GameObject.Find ("GameState"));
			Destroy (GameObject.Find ("GameManager"));
		}
	}
}
