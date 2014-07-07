using UnityEngine;
using System.Collections;

public class ScoreTexture : MonoBehaviour {

	public Texture2D[] digitSprites; 
	public Collision collScript; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void calculatorScore (int score){
		// 5 digits to get to 10000
		Texture2D[] scoreImages = new Texture2D[5];

		int ones = score % 10; 


	}
}
