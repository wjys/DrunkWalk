using UnityEngine;
using System.Collections;

// TURN THIS ON WHEN IN GAME

public class UniMoveGame : MonoBehaviour {

	public GameObject[] players;
	public GameObject player; 
	public GameObject[] ui;
	public GUIText countdown;
	public UniMoveController[] moves;
	public int[] moveInitIDs;
	public int playerCount; 
	public int numPlayers;
	
	private Rect[] rects; 
	
	private Vector3[] positions; 
	private Quaternion rotations;

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

	
	void Start() 
	{
		countdown = GameObject.Find ("Countdown").guiText;
		playerCount = 0;
		ctdown = 4;

		// CAMERA VIEWPORT
		rects = new Rect[7];
		
		rects[0].Set (-0.002f, 0.504f, 0.5f, 0.5f);		// multiplayer - p1
		rects[1].Set (0.502f, 0.504f, 0.5f, 0.5f);		// multiplayer - p2
		rects[2].Set (-0.002f, -0.004f, 0.5f, 0.5f);	// multiplayer - p3
		rects[3].Set (0.502f, -0.004f, 0.5f, 0.5f);		// multiplayer - p4
		
		rects[4].Set (-0.002f, 0.0f, 0.5f, 1.0f);		// two players - p1
		rects[5].Set (0.502f, 0.0f, 0.5f, 1.0f);		// two players - p2
		
		rects[6].Set (0.0f, 0.0f, 1.0f, 1.0f);			// single player - full screen
		
		
		positions = new Vector3[4] { 	new Vector3 (-1.10f, 1.424898f, 3.941933f),
										new Vector3 (-0.03585815f, 1.424898f, 3.941933f),
										new Vector3 (1.16f, 1.424898f, 3.941933f), 
										new Vector3 (2.383401f, 1.424898f, 3.366474f)};
		
		rotations = new Quaternion (0, 0, 0, 0);
	
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
			positions = new Vector3[1] { new Vector3 (0, 1.424898f, -6) };
		}
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
						positions = new Vector3[1] { new Vector3 (0, 1.424898f, -6) };
					}
				}
				else {
					positions = new Vector3[4] { 	new Vector3 (-1.10f, 1.424898f, 3.941933f),
													new Vector3 (-0.03585815f, 1.424898f, 3.941933f),
													new Vector3 (1.16f, 1.424898f, 3.941933f), 
													new Vector3 (2.383401f, 1.424898f, 3.366474f)};
				}
				setGMTrack = true;
			}
			else {
				GameManager.ins.track = 1;
				setGMTrack = true;
			}
		}

		if (numPlayers == 0){
			getVariables();
		}
		else {
			if (GameManager.ins.mode == GameState.GameMode.Party){
				if (!bedSpawned){
					Spawners = GameObject.Find ("BedSpawner").GetComponentsInChildren<Transform>();
					setBed();
				}
			}
			else bedSpawned = true;

			if (StopManager() == false){
				createPlayers ();
				UniMoveSetPlayers();
			}
			if (StopManager ()){
				if (bedSpawned == true){
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
			players [playerCount] = Instantiate (player, positions [playerCount], rotations) as GameObject;
			
			// SET UP THE ID OF THE HEAD AND COMPONENTS
			players [playerCount].name = "Head " + (playerCount + 1);
			players [playerCount].GetComponent<DrunkMovement> ().id = playerCount + 1;
			players [playerCount].GetComponent<UniMoveDisplay> ().id = playerCount + 1; 



			// SET UP THE CAMERAS (ON HEAD'S CHILDREN)
			Camera[] cams;
			
			switch (playerCount) {
				
			// 1 player
			case 0:
				cams = players [playerCount].GetComponentsInChildren<Camera> ();
				foreach (Camera cam in cams){
					cam.rect = (rects[6]);
				}
				playerCount++;
				break;
				
				// 2 players
			case 1:
				for (int i = 0; i <= playerCount; i++){
					cams = players[i].GetComponentsInChildren<Camera>();
					foreach (Camera cam in cams){
						cam.rect = (rects [i+4]);
						//print (i + " " + cam.rect);
					}
				}
				playerCount++;
				break;
				
				// 3 players
			case 2:
				for (int i = 0; i <= playerCount; i++){
					cams = players[i].GetComponentsInChildren<Camera>();
					foreach (Camera cam in cams){
						cam.rect = (rects [i]);
					}
				}
				playerCount++;
				break;
				
				// 4 players
			case 3:
				cams = players [playerCount].GetComponentsInChildren<Camera> ();
				foreach (Camera cam in cams){
					cam.rect = (rects[3]);
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
			introtext.SetActive (false);
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
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * SET UI
	 * set UICam viewports and assign the scripts the children's components call
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void setUI(){
		print ("set ui!");
		Camera ui;
		
		// SINGLE PLAYER MODE
		if (numPlayers == 1) {
			ui = GameObject.Find ("UICam 1").camera;
			ui.rect = rects[6];
			
			Eyelids eyelids = ui.GetComponentInChildren <Eyelids>();
			Compass comp = ui.GetComponentInChildren<Compass>();
			Ouch ouch = ui.GetComponentInChildren<Ouch>();
			
			eyelids.me = GameObject.Find ("Head 1").GetComponent<DrunkMovement>();
			comp.me =  GameObject.Find ("Head 1");
			comp.bed = GameObject.Find ("BedTarget");
			comp.bedSpriteScale = comp.bed.transform;
			ouch.collision = GameObject.Find ("Head 1").GetComponent<Collision>();
			ouch.dm = GameObject.Find ("Head 1").GetComponent<DrunkMovement>();
			
			eyelids.enabled = true;
			comp.enabled = true;
			ouch.enabled = true;
			
			foreach (SpriteRenderer sprite in ui.GetComponentsInChildren<SpriteRenderer>()){
				sprite.enabled = true;
			}
		}
		
		// 2 PLAYER MODE
		else if (numPlayers == 2) {
			for (int i = 1; i < numPlayers+1; i++){
				ui = GameObject.Find ("UICam " + i).camera;
				ui.rect = rects[i+3];
				
				Eyelids eyelids = ui.GetComponentInChildren <Eyelids>();
				Compass comp = ui.GetComponentInChildren<Compass>();
				Ouch ouch = ui.GetComponentInChildren<Ouch>();
				
				eyelids.me = GameObject.Find ("Head " + i).GetComponent<DrunkMovement>();
				comp.me =  GameObject.Find ("Head " + i);
				comp.bed = GameObject.Find ("BedTarget");
				comp.bedSpriteScale = comp.bed.transform;
				ouch.collision = GameObject.Find ("Head " + i).GetComponent<Collision>();
				ouch.dm = GameObject.Find ("Head " + i).GetComponent<DrunkMovement>();
				
				eyelids.enabled = true;
				comp.enabled = true;
				ouch.enabled = true;
				
				foreach (SpriteRenderer sprite in ui.GetComponentsInChildren<SpriteRenderer>()){
					sprite.enabled = true;
				}
			}
		}
		
		// MULTIPLAYER (3+) MODE
		else if (numPlayers >= 3){
			for (int i = 1; i < numPlayers+1; i++){
				ui = GameObject.Find ("UICam " + i).camera;
				ui.rect = rects[i-1];
				
				Eyelids eyelids = ui.GetComponentInChildren <Eyelids>();
				Compass comp = ui.GetComponentInChildren<Compass>();
				Ouch ouch = ui.GetComponentInChildren<Ouch>();
				
				eyelids.me = GameObject.Find ("Head " + i).GetComponent<DrunkMovement>();
				comp.me =  GameObject.Find ("Head " + i);
				comp.bed = GameObject.Find ("BedTarget");
				comp.bedSpriteScale = comp.bed.transform;
				ouch.collision = GameObject.Find ("Head " + i).GetComponent<Collision>();
				ouch.dm = GameObject.Find ("Head " + i).GetComponent<DrunkMovement>();
				
				eyelids.enabled = true;
				comp.enabled = true;
				ouch.enabled = true;
				
				foreach (SpriteRenderer sprite in ui.GetComponentsInChildren<SpriteRenderer>()){
					sprite.enabled = true;
				}
				
				ui.gameObject.SetActive(true);
			}
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
	 * UNIMOVE SET PLAYERS
	 * set the UniMove controller to the player (add component to the player and initialize it)
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void UniMoveSetPlayers(){ 
		for (int i = 0; i < numPlayers; i++){
			UniMoveController mv = players[i].GetComponent<UniMoveController>();
			if (mv == null){
				UniMoveDisplay disp = players[i].GetComponent<UniMoveDisplay>();
				mv = players[i].AddComponent<UniMoveController>() as UniMoveController;
				mv.Init (moveInitIDs[i]);
				mv.id = disp.id;
			}
		}
		//print ("exiting UniMoveSetPlayers()");
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * ACTIVATE HEAD/PLAYER COMPONENTS
	 * when all the players have been set, activate the player components
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void UniMoveActivateComponents(){
		print ("activate components!");
		for (int i = 0; i < numPlayers; i++) {
			UniMoveController mv = players [i].GetComponent<UniMoveController> ();
			
			if (mv != null) {
				DrunkMovement dm = players [i].GetComponent<DrunkMovement> (); 
				Rotation rot = players [i].GetComponent<Rotation> ();
				DrunkForce df = players [i].GetComponent<DrunkForce> ();
				Collision col = players [i].GetComponent<Collision> ();
				
				dm.enabled = true;
				rot.enabled = true;
				df.enabled = true;
				col.enabled = true;
			}
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

	/* --------------------------------------------------------------------------------------------------------------------------
	 * GET VARIABLES: if numPlayers 0, get variables from UniMoveSplash
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void getVariables(){
		print ("reset game move settings");
		UniMoveSplash splash = gameObject.GetComponent<UniMoveSplash> ();
		numPlayers = numPlayers;
		players = new GameObject[numPlayers];
		moveInitIDs = new int[numPlayers];
		for (int i = 0; i < numPlayers; i++){
			for (int j = 0; j < splash.moves.Count; j++){
				if (moves[j].id == i+1){
					moveInitIDs[i] = j;
					break;
				}
			}
		}
	}
}
