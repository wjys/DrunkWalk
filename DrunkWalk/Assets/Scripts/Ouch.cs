using UnityEngine;
using System.Collections;

public class Ouch : MonoBehaviour {

	public GameManager gm;
	public string[] ouches;
	public int ouchIndex;
	public GUIText ouchGui;
	public Collision collision;
	public bool displayed; 
	public SpriteRenderer win;

	public DrunkMovement dm;
	public Sounds sounds;

	// Use this for initialization
	void Start () {
		displayed = false;
		gm = GameObject.Find ("GameManager").GetComponent<GameManager>();
		win = gameObject.GetComponentInChildren<SpriteRenderer> ();
		win.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if ((gm.winnerIndex == 0 && GameManager.ins.mode == GameState.GameMode.Party)|| GameManager.ins.mode == GameState.GameMode.Race || GameManager.ins.mode == GameState.GameMode.ScoreAttack || GameManager.ins.mode == GameState.GameMode.Stealth){
			reachingBed ();

		}
		else if (gm.winnerIndex == 1 && GameManager.ins.mode == GameState.GameMode.Party){
			reachingCouch ();
		}
		else if (gm.winnerIndex == 2 && GameManager.ins.mode == GameState.GameMode.Party){
			reachingTub ();
		}
		else if (gm.winnerIndex == 3 && GameManager.ins.mode == GameState.GameMode.Party){
			GameManager.ins.losers[GameManager.ins.loserIndex] = dm.id;
			GameManager.ins.loserIndex++;
			GameManager.ins.status = GameState.GameStatus.End;
		}
		if (!dm.fallen){
			if (collision.recoiled && !collision.reachedBed){
				if (!displayed){
					gameObject.guiText.enabled = true;
					ouchIndex = Random.Range (0,ouches.Length);
					ouchGui.text = ouches[ouchIndex];
					displayed = true; 
				}
			} else {
				gameObject.guiText.enabled = false;
				displayed = false; 
			}
		}
		else {
			gameObject.guiText.enabled = false;
			displayed = false;
		}
	}

	private void reachingBed(){
		if (collision.reachedBed) {
			
			GameObject.Find ("UICam " + dm.id).GetComponentInChildren<CameraBG>().enabled = true;
			SpriteRenderer[] sprites = GameObject.Find ("UICam " + dm.id).GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer sprite in sprites){
				sprite.enabled = false;
			}
			GameObject.Find ("UICam " + dm.id).camera.clearFlags = CameraClearFlags.Skybox;
			gameObject.guiText.enabled = false;
			win.enabled = true;
			dm.gameObject.SetActive(false);
			
			GameObject gm = GameObject.Find ("GameManager");
			GameManager manager = gm.GetComponent<GameManager>();
			manager.winners[manager.winnerIndex] = dm.id;
			if (manager.winnerIndex == 0){
				manager.winner = dm.id;
				manager.score = collision.score;
			}
			GameObject.Find ("Inst " + dm.id).SetActive(false);
			manager.winnerIndex++;
			
			this.enabled = false;
		}
	}

	private void reachingCouch(){
		if (GameManager.ins.mode == GameState.GameMode.Party){
			if (collision.reachedCouch){
				GameObject.Find ("UICam " + dm.id).GetComponentInChildren<CameraBG>().enabled = true;
				SpriteRenderer[] sprites = GameObject.Find ("UICam " + dm.id).GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer sprite in sprites){
					sprite.enabled = false;
				}
				GameObject.Find ("UICam " + dm.id).camera.clearFlags = CameraClearFlags.Skybox;
				gameObject.guiText.enabled = false;
				win.enabled = true;
				dm.gameObject.SetActive(false);
				
				GameObject gm = GameObject.Find ("GameManager");
				GameManager manager = gm.GetComponent<GameManager>();
				manager.winners[manager.winnerIndex] = dm.id;
				if (manager.winnerIndex == 0){
					manager.winner = dm.id;
				}
				GameObject.Find ("Inst " + dm.id).SetActive(false);
				manager.winnerIndex++;
				
				this.enabled = false;
			}
		}
	}
	private void reachingTub(){
		if (GameManager.ins.mode == GameState.GameMode.Party){
			if (collision.reachedCouch){
				GameObject.Find ("UICam " + dm.id).GetComponentInChildren<CameraBG>().enabled = true;
				SpriteRenderer[] sprites = GameObject.Find ("UICam " + dm.id).GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer sprite in sprites){
					sprite.enabled = false;
				}
				GameObject.Find ("UICam " + dm.id).camera.clearFlags = CameraClearFlags.Skybox;
				gameObject.guiText.enabled = false;
				win.enabled = true;
				dm.gameObject.SetActive(false);
				
				GameObject gm = GameObject.Find ("GameManager");
				GameManager manager = gm.GetComponent<GameManager>();
				manager.winners[manager.winnerIndex] = dm.id;
				if (manager.winnerIndex == 0){
					manager.winner = dm.id;
				}
				GameObject.Find ("Inst " + dm.id).SetActive(false);
				manager.winnerIndex++;
				
				this.enabled = false;
			}
		}
	}
}
