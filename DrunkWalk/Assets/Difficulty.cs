using UnityEngine;
using System.Collections;

public class Difficulty : MonoBehaviour {

	//Game Manager
	public GameManager GM;

	//Different Drink Objs
	public GameObject[] drinks;

	//Individual drink levels
	public int drinkID;
	//public float smooth;
	//public Vector3 newDrinkPos;
	//public Vector3 oldDrinkPos;
	public static int JnC, Beer, Whiskey, Sangria;

	//Total drunk level
	public int totalDrunk;


	// Use this for initialization
	void Start () {
		drinkID = 0;
		JnC = Beer = Whiskey = Sangria = 0;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate () {
		
	}
}
