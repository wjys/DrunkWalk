using UnityEngine;
using System.Collections;

public class EndScreen : MonoBehaviour {

	// sound stuff 
	public AudioClip[] clips;  
	private bool soundPlayed; 
	public bool getMoves;
	public UniMoveController[] moves;
	public UniMoveController[] pairedMoves;
	public Sprite[] winSprites;
	public Sprite[] loseSprites;
	public Sprite[] numSprites;

	public bool spritesSet;
	
	// Use this for initialization
	void Start () {
		soundPlayed = false; 
		spritesSet = false;
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
		if (GameManager.ins.mode == GameState.GameMode.ScoreAttack || GameManager.ins.mode == GameState.GameMode.Stealth){
			Destroy(GameObject.Find ("RaceEnd"));
			Destroy(GameObject.Find ("PartyEnd"));

			if (Input.anyKey){
				Application.LoadLevel ("Scores");
				this.enabled = false;
			}
		}
		else if (GameManager.ins.mode == GameState.GameMode.Party || GameManager.ins.mode == GameState.GameMode.Race){

			//if (!spritesSet){
				setSprites();
			//}

			if (Input.anyKey){
				Destroy(GameObject.Find("GameState"));
				Application.LoadLevel ("Splash");
				Destroy (this.gameObject);
				this.enabled = false;
			}
		}
	}

	private void findMoves(){
		moves = gameObject.GetComponents<UniMoveController>();
		getMoves = true;
	}

	private void setSprites(){
		GameObject race, party;
		race = GameObject.Find ("RaceEnd");
		party = GameObject.Find ("PartyEnd");
		
		if (GameManager.ins.mode == GameState.GameMode.Race){
			//IF RACE RESULTS
			if (party != null){
				Destroy (party);
			}
			race.SetActive (true);

			getRaceWinSprites();
			getRaceLoseSprites();
			
		} else if (GameManager.ins.mode == GameState.GameMode.Party){
			//IF PARTY RESULTS
			if (race != null){
				Destroy (race);
			}
			party.SetActive (true);

			getPartyWinSprites ();
			getPartyLoseSprites();
		}
		spritesSet = true;
	}
	private void getPartyWinSprites(){
		for (int i = 0; i < GameManager.ins.winnerIndex; i++){
			SpriteRenderer[] sprites = GameObject.Find ("Win" + (i+1)).GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spr in sprites){
				if (spr.gameObject.name.Equals ("PlayerNum")){
					switch (GameManager.ins.winners [i]){
					case 1:
						spr.sprite = numSprites[0];
						break;
					case 2:
						spr.sprite = numSprites[1];
						break;
					case 3:
						spr.sprite = numSprites[2];
						break;
					case 4:
						spr.sprite = numSprites[3];
						break;
					}
					spr.enabled = true;
					spr.gameObject.GetComponent<PlayerNum>().enabled = true;
				}
				else {
					switch (GameManager.ins.multiChosenChar [GameManager.ins.winners [i]-1]){
					case 1:
						spr.sprite = winSprites[0];
						break;
					case 2:
						spr.sprite = winSprites[1];
						break;
					case 3:
						spr.sprite = winSprites[2];
						break;
					case 4:
						spr.sprite = winSprites[3];
						break;
					}
					spr.enabled = true;
				}
			}
		}
	}

	private void getPartyLoseSprites(){
		for (int i = 0; i < GameManager.ins.loserIndex; i++){
			SpriteRenderer[] sprites = GameObject.Find ("Lose" + (i+1)).GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spr in sprites){
				if (spr.gameObject.name.Equals ("PlayerNum")){
					switch (GameManager.ins.losers [i]){
					case 1:
						spr.sprite = numSprites[0];
						break;
					case 2:
						spr.sprite = numSprites[1];
						break;
					case 3:
						spr.sprite = numSprites[2];
						break;
					case 4:
						spr.sprite = numSprites[3];
						break;
					}
					spr.enabled = true;
					spr.gameObject.GetComponent<PlayerNum>().enabled = true;
				}
				else {
					switch (GameManager.ins.multiChosenChar [GameManager.ins.losers [i]-1]){
					case 1:
						spr.sprite = loseSprites[0];
						break;
					case 2:
						spr.sprite = loseSprites[1];
						break;
					case 3:
						spr.sprite = loseSprites[2];
						break;
					case 4:
						spr.sprite = loseSprites[3];
						break;
					}
					spr.enabled = true;
				}
			}
		}
	}

	private void getRaceWinSprites(){
		for (int i = 0; i < GameManager.ins.winnerIndex; i++){
			SpriteRenderer[] sprites = GameObject.Find ("RWin" + (i+1)).GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spr in sprites){
				if (spr.gameObject.name.Equals ("PlayerNum")){
					switch (GameManager.ins.winners [i]){
					case 1:
						spr.sprite = numSprites[0];
						break;
					case 2:
						spr.sprite = numSprites[1];
						break;
					case 3:
						spr.sprite = numSprites[2];
						break;
					case 4:
						spr.sprite = numSprites[3];
						break;
					}
					spr.enabled = true;
					spr.gameObject.GetComponent<PlayerNum>().enabled = true;
				}
				else if (spr.gameObject.name.Equals ("EndRoute")){
					spr.enabled = true;
				}
				else {
					switch (GameManager.ins.multiChosenChar [GameManager.ins.winners [i]-1]){
					case 1:
						spr.sprite = winSprites[0];
						break;
					case 2:
						spr.sprite = winSprites[1];
						break;
					case 3:
						spr.sprite = winSprites[2];
						break;
					case 4:
						spr.sprite = winSprites[3];
						break;
					}
					spr.enabled = true;
				}
			}
		}
	}
	
	private void getRaceLoseSprites(){
		for (int i = 0; i < GameManager.ins.loserIndex; i++){
			SpriteRenderer[] sprites = GameObject.Find ("RLose" + (i+1)).GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spr in sprites){
				if (spr.gameObject.name.Equals ("PlayerNum")){
					switch (GameManager.ins.losers [i]){
					case 1:
						spr.sprite = numSprites[0];
						break;
					case 2:
						spr.sprite = numSprites[1];
						break;
					case 3:
						spr.sprite = numSprites[2];
						break;
					case 4:
						spr.sprite = numSprites[3];
						break;
					}
					spr.enabled = true;
					spr.gameObject.GetComponent<PlayerNum>().enabled = true;
				}
				else if (spr.gameObject.name.Equals ("EndRoute")){
					spr.enabled = true;
				}
				else {
					switch (GameManager.ins.multiChosenChar [GameManager.ins.losers [i]-1]){
					case 1:
						spr.sprite = loseSprites[0];
						break;
					case 2:
						spr.sprite = loseSprites[1];
						break;
					case 3:
						spr.sprite = loseSprites[2];
						break;
					case 4:
						spr.sprite = loseSprites[3];
						break;
					}
					spr.enabled = true;
				}
			}
		}
	}

}
