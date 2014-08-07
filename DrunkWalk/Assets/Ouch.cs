using UnityEngine;
using System.Collections;

public class Ouch : MonoBehaviour {

	public string[] ouches;
	public int ouchIndex;
	public GUIText ouchGui;
	public Collision collision;
	public bool displayed; 
	public SpriteRenderer win;

	public DrunkMovement dm;

	// Use this for initialization
	void Start () {
		displayed = false; 
		win = gameObject.GetComponentInChildren<SpriteRenderer> ();
		win.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		reachingBed ();
		reachingCouch ();
		reachingTub ();
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
			GameObject bed = GameObject.Find ("BedObj");
			BoxCollider bedTrigger = bed.GetComponent<BoxCollider>();
			
			if (GameManager.ins.mode == GameState.GameMode.Party){
				if (bedTrigger.enabled){
					bedTrigger.enabled = false;
					
					GameObject.Find ("CouchObj").GetComponent<BoxCollider>().enabled = true;
				}
				else return;
			}
			
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
			manager.winnerIndex++;
			
			this.enabled = false;
		}
	}

	private void reachingCouch(){
		if (GameManager.ins.mode == GameState.GameMode.Party){
			if (collision.reachedCouch){
				GameObject couch = GameObject.Find ("CouchObj");
				BoxCollider couchTrigger = couch.GetComponent<BoxCollider>();

				if (couchTrigger.enabled){
					//couchTrigger.enabled = false;

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
					manager.winnerIndex++;
					
					this.enabled = false;
				}
			}
		}
	}
		private void reachingTub(){
			if (GameManager.ins.mode == GameState.GameMode.Party){
				if (collision.reachedCouch){
					GameObject tub = GameObject.Find ("TubObj");
					BoxCollider tubTrigger = tub.GetComponent<BoxCollider>();
					
					if (tubTrigger.enabled){
						//couchTrigger.enabled = false;
						
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
						manager.winnerIndex++;
						
						this.enabled = false;
					}
				}
			}
		}
}
