using UnityEngine;
using System.Collections;

public class MouseSetup : MonoBehaviour {
	
	public GameObject player;
	public int character;
	public GameObject Zach;
	public GameObject Ana;
	public GameObject AnhChi;
	public GameObject Winnie; 
	public GameObject ui;
	public GUIText countdown;
	
	private Rect rect; 
	
	private Vector3 position; 
	private Quaternion rotation;
	
	//BED STUFF
	public Transform[] Spawners;
	public GameObject Bed;
	public GameObject BedObj;
	public int bedIndex;
	public bool bedSpawned;
	public bool startGame;
	public int ctdown;
	public bool countingDown;
	
	public bool setGMTrack;
	
	public GameObject bedTarget;

	public int numPlayers;
	public int playerCount;
	
	
	void Start() 
	{
		countdown = GameObject.Find ("Countdown").GetComponent<GUIText>();
		ctdown = 4;
		
		// CAMERA VIEWPORT
		rect.Set (0.0f, 0.0f, 1.0f, 1.0f);			// single player - full screen
		
		
		position = new Vector3 (-1.10f, 1.424898f, 3.941933f);
		
		rotation = new Quaternion (0, 0, 0, 0);
		
		// if PARTY mode, get the bed spawn locations
		if (GameManager.ins.mode == GameState.GameMode.Party){
			Spawners = new Transform[12];
			Spawners = GameObject.Find ("BedSpawner").GetComponentsInChildren<Transform>();
			bedSpawned = false;
		}
		else {
			bedSpawned = true;
		}
		
		// if TUTORIAL level, instantiate player at a different position
		if (GameManager.ins.status == GameState.GameStatus.Tutorial){
			position = new Vector3 (0, 1.424898f, -6);
		}
		playerCount = 0; 
		numPlayers = 1;
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * UPDATE:
	 * (0) if there are no players, get the setup variables from UniMoveSplash
	 * (1) if PARTY mode: spawn the bed (otherwise, bed is already in the level!)
	 * (2) create the players and assign the move controllers to the right players
	 * (3) check if same number of heads created as numPlayers && if there is a bed in the levle
	 * (4) set the UICams
	 * (5) countdown to begin the game (CALIBRATION?)
	 * (6) activate the player/head components (allow them to move, etc.)
	 * (7) reset the variables for UniMoveGame (this script)
	 * (8) disable the script while the game is played
	 * -------------------------------------------------------------------------------------------------------------------------- */
	void Update() 
	{
		if (!setGMTrack){
			if (GameManager.ins.track != 0){
				if (GameManager.ins.status != GameState.GameStatus.Tutorial){
					// if TUTORIAL level, instantiate player at a different position
					if (GameManager.ins.status == GameState.GameStatus.Tutorial){
						position = new Vector3 (0, 1.424898f, -6);
					}
				}
				else {
					position = new Vector3 (-1.10f, 1.424898f, 3.941933f);
				}
				setGMTrack = true;
			}
			else {
				GameManager.ins.track = 1;
				setGMTrack = true;
			}
		}
		

		if (GameManager.ins.mode == GameState.GameMode.Party){
			if (!bedSpawned){
				Spawners = GameObject.Find ("BedSpawner").GetComponentsInChildren<Transform>();
				setBed();
			}
		}
		else bedSpawned = true;
		if (GameObject.Find ("Head 1") == null){
			createPlayers();
		}
		if (StopManager ()){
			if (bedSpawned){
				if (!startGame){
					if (!countingDown){
						setUI();
						countingDown = true;
					}
					else {
						CountdownToGame();
					}
				}
				else {
					UniMoveActivateComponents();
					countdown.enabled = false;
					resetVariables ();
					this.enabled = false;
				}
			}
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * CREATE PLAYERS (HEADS)
	 * instantiate the player:
	 * (1) set up the viewport rect of the player cameras
	 * (2) set the appropriate ID to the player 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void createPlayers(){
		print ("create players!");
		while (playerCount < numPlayers){
			character = GameManager.ins.chosenChar;
			switch (character){
			case 1:
				player = Instantiate (Zach, position, rotation) as GameObject;
				break;
			case 2:
				player = Instantiate (Ana, position, rotation) as GameObject;
				break;
			case 3:
				player = Instantiate (AnhChi, position, rotation) as GameObject;
				break;
			case 4:
				player = Instantiate (Winnie, position, rotation) as GameObject;
				break;
			default: 
				break;
			}

			
			// SET UP THE ID OF THE HEAD AND COMPONENTS
			player.name = "Head " + (playerCount + 1);
			player.GetComponent<DrunkMovement> ().id = playerCount + 1;
			player.GetComponent<UniMoveDisplay> ().id = playerCount + 1; 
			
			LayerMask meLayer = LayerMask.NameToLayer("Me" + (playerCount + 1));
			player.layer = meLayer;
			
			Transform[] trans = player.GetComponentsInChildren<Transform>();
			foreach (Transform tran in trans){
				if (tran.gameObject.name.Equals ("Model")){
					GameObject model = tran.gameObject;
					model.layer = meLayer;
					foreach (Transform t in model.GetComponentsInChildren<Transform>()){
						t.gameObject.layer = meLayer;
					}
					break;
				}
			}
			
			Camera[] cams;
			
			cams = player.GetComponentsInChildren<Camera>();
			foreach (Camera cam in cams){
				cam.cullingMask &= ~(1 << meLayer);
			}
			
			
			
			
			
			// SET UP THE CAMERAS (ON HEAD'S CHILDREN)
			
			
			switch (playerCount) {

			case 0:
				cams = player.GetComponentsInChildren<Camera> ();
				foreach (Camera cam in cams){
					cam.rect = (rect);
				}
				playerCount++;
				break;
			default:
				break;
			}
		}
	}
	/* --------------------------------------------------------------------------------------------------------------------------
	 * COUNTDOWN TO GAME & COROUTINES 
	 * change out the guitext for the countdown and different delays for each 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	private void CountdownToGame(){
		if (ctdown == 4){
			StartCoroutine (beginCountdown());
		}
		else if (ctdown == 3){
			StartCoroutine (_nullto3());
		}
		else if (ctdown == 2){
			StartCoroutine (_3to2());
		}
		else if (ctdown == 1){
			StartCoroutine (_2to1());
		}
		else if (ctdown == 0){
			StartCoroutine (_1toGo());
		}
		else {
			startGame = true;
		}
	}
	IEnumerator beginCountdown(){
		yield return new WaitForSeconds(2.0f);
		for (int i = 1; i <= numPlayers; i++){
			GameObject introtext = GameObject.Find ("Intro " + i);
			//if (introtext != null){
			introtext.SetActive (false);
			//}
		}
		ctdown = 3;
	}
	IEnumerator _nullto3(){
		countdown.text = ctdown.ToString();
		countdown.enabled = true;
		yield return new WaitForSeconds(0.7f);
		ctdown = 2;
	}
	
	IEnumerator _3to2(){
		countdown.text = ctdown.ToString ();
		yield return new WaitForSeconds(0.7f);
		ctdown = 1;
	}
	IEnumerator _2to1(){
		countdown.text = ctdown.ToString ();
		yield return new WaitForSeconds(0.7f);
		ctdown = 0;
	}
	IEnumerator _1toGo(){
		countdown.text = "GO!";
		yield return new WaitForSeconds(0.7f);
		ctdown = -1;
		if (GameManager.ins.status == GameState.GameStatus.Tutorial){
			GameObject.Find ("Tutorial").GetComponent<Tutorial>().enabled = true;
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * SET UI
	 * set UICam viewports and assign the scripts the children's components call
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void setUI(){
		Camera ui;
		ui = GameObject.Find ("UICam 1").GetComponent<Camera>();
		ui.rect = rect;
		
		Eyelids eyelids = ui.GetComponentInChildren <Eyelids>();
		Compass comp = ui.GetComponentInChildren<Compass>();
		Ouch ouch = ui.GetComponentInChildren<Ouch>();
		Sounds sounds = ui.GetComponentInChildren<Sounds>();
		
		eyelids.me = GameObject.Find ("Head 1").GetComponent<DrunkMovement>();
		eyelids.sounds = sounds;
		comp.me =  GameObject.Find ("Head 1");
		comp.target = GameObject.Find ("BedTarget");
		comp.spriteScale = comp.target.transform;
		ouch.collision = GameObject.Find ("Head 1").GetComponent<Collision>();
		ouch.dm = GameObject.Find ("Head 1").GetComponent<DrunkMovement>();
		ouch.sounds = sounds;
		sounds.dm = ouch.dm;
		sounds.col = ouch.collision;
		sounds.eyelids = eyelids;
		sounds.ouch = ouch;
		
		sounds.dm.sounds = sounds;
		sounds.col.sounds = sounds;
		
		switch (character){
		case 1:
			sounds.clips_grunts = sounds.zach_grunts;
			sounds.clips_objects = sounds.zach_objects; 
			sounds.clips_furniture = sounds.zach_furniture;
			sounds.clips_wall =	sounds.zach_wall;
			sounds.clips_drowsy = sounds.zach_drowsy;
			sounds.clips_fall = sounds.zach_fall;
			sounds.clips_struggle = sounds.zach_struggle;
			sounds.clips_getup = sounds.zach_getup;
			sounds.clips_giveup = sounds.zach_giveup;
			sounds.clips_bed = sounds.zach_bed;
			break;
		case 2:
			sounds.clips_grunts = sounds.ana_grunts;
			sounds.clips_objects = sounds.ana_objects; 
			sounds.clips_furniture = sounds.ana_furniture;
			sounds.clips_wall =	sounds.ana_wall;
			sounds.clips_drowsy = sounds.ana_drowsy;
			sounds.clips_fall = sounds.ana_fall;
			sounds.clips_struggle = sounds.ana_struggle;
			sounds.clips_getup = sounds.ana_getup;
			sounds.clips_giveup = sounds.ana_giveup;
			sounds.clips_bed = sounds.ana_bed;
			break;
		case 3:
			sounds.clips_grunts = sounds.acb_grunts;
			sounds.clips_objects = sounds.acb_objects; 
			sounds.clips_furniture = sounds.acb_furniture;
			sounds.clips_wall =	sounds.acb_wall;
			sounds.clips_drowsy = sounds.acb_drowsy;
			sounds.clips_fall = sounds.acb_fall;
			sounds.clips_struggle = sounds.acb_struggle;
			sounds.clips_getup = sounds.acb_getup;
			sounds.clips_giveup = sounds.acb_giveup;
			sounds.clips_bed = sounds.acb_bed;
			break;
		case 4:
			sounds.clips_grunts = sounds.winnie_grunts;
			sounds.clips_objects = sounds.winnie_objects; 
			sounds.clips_furniture = sounds.winnie_furniture;
			sounds.clips_wall =	sounds.winnie_wall;
			sounds.clips_drowsy = sounds.winnie_drowsy;
			sounds.clips_fall = sounds.winnie_fall;
			sounds.clips_struggle = sounds.winnie_struggle;
			sounds.clips_getup = sounds.winnie_getup;
			sounds.clips_giveup = sounds.winnie_giveup;
			sounds.clips_bed = sounds.winnie_bed;
			break;
		}
		
		AudioManager.ins.gameObject.GetComponent<AudioSource>().volume = 0.3f;
		eyelids.enabled = true;
		comp.enabled = true;
		ouch.enabled = true;
		sounds.enabled = true;
		
		foreach (SpriteRenderer sprite in ui.GetComponentsInChildren<SpriteRenderer>()){
			sprite.enabled = true;
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * SET BED: spawn the bed at a random spawner position
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void setBed(){
		if (Spawners != null){
			bedIndex = Random.Range (0,Spawners.Length);
			
			
			if (BedObj == null){
				BedObj = Instantiate (Bed, Spawners [bedIndex].position, Spawners[bedIndex].rotation) as GameObject;
				bedTarget = GameObject.Find("BedTarget");
				bedTarget.transform.position = BedObj.transform.position;
				//BedObj.name = "BedObj";
			} else {
				//Debug.Log("DESTROY BED SPAWNER");
				Destroy(GameObject.Find ("BedSpawner"));
				bedSpawned = true;
			}
			
		}
		else {
			Spawners = new Transform[12];
			Spawners = GameObject.Find ("BedSpawner").GetComponentsInChildren<Transform>();
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * ACTIVATE HEAD/PLAYER COMPONENTS
	 * when all the players have been set, activate the player components
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void UniMoveActivateComponents(){
		print ("activate components!");
		if (playerCount == numPlayers) {
			DrunkMovement 	dm 	= player.GetComponent<DrunkMovement> (); 
			Rotation 		rot = player.GetComponent<Rotation> ();
			DrunkForce 		df 	= player.GetComponent<DrunkForce> ();
			Collision 		col = player.GetComponent<Collision> ();
			
			dm.enabled = true;
			rot.enabled = true;
			df.enabled = true;
			col.enabled = true;
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * STOP MANAGER: if there as many heads created as there are moves/players joined in the game
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private bool StopManager(){
		if (playerCount >= numPlayers){
			//print ("all heads created");
			return true;
		}
		return false; 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * RESET VARIABLES: of the script in case loop back to menu
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void resetVariables(){
		startGame = false;
		countingDown = false;
		bedSpawned = false;
		
		playerCount = 0;
		ctdown = 4;
		bedIndex = 0;
	}
}
