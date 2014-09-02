using UnityEngine;
using System.Collections;

public class Difficulty : MonoBehaviour {

	//Game Manager
	public GameManager GM;

	//Different Drink Objs
	public GameObject[] drinks;

	//Individual drink levels
	public int currDrink;
	public int[] drinkID;
	public string[] drinkName;

	//Total drunk level
	public int totalDrunk;
	public SpriteRenderer[] drunkLv;
	public float smooth;

	//Winner
	public int winner;
	public int numOfPlayers;


	// Use this for initialization
	void Start () {
		totalDrunk = 0;
		currDrink = 0;
		drinkID = new int[4];
		//Jack & Coke
		drinkID[0] = 0;
		//Beer
		drinkID[1] = 0;
		//Whiskey
		drinkID[2] = 0;
		//Sangria
		drinkID[3] = 0;

		drinkName = new string[4] {"Jack & Coke", "Beer", "Whiskey", "Sangria"};

		if (GM == null){
			GM = GameObject.Find ("_GameManager").GetComponent<GameManager>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < drinkID.Length; i++){
			drinks[i].GetComponentInChildren<TextMesh>().text = drinkName[i] + ": " + drinkID[i];
		}

		for (int d = 0; d < totalDrunk; d++) {
						drunkLv [d - 1].enabled = true;
				}

		if (totalDrunk == 0) {
				for (int r = 0; r < drunkLv.Length; r++) {
				drunkLv[r].color = Color.red;
				}
		}
	}

	void FixedUpdate () {
		//tDBar.transform.localScale = Vector3.Lerp(tDBar.transform.localScale, tDBarScale, smooth * Time.deltaTime);
	}
}
