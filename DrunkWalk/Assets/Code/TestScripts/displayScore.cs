using UnityEngine;
using System.Collections;

public class displayScore : MonoBehaviour {

	public Collision collScript;
	public GUIText scoreDisplay; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		showScore (); 
	}

	private void showScore(){
	//	scoreDisplay.text = "Score: " + collScript.score; 
	}
}
