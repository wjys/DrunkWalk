using UnityEngine;
using System.Collections;

// TURN THIS ON WHEN IN GAME

public class UniMoveGame : MonoBehaviour {

	public GameObject[] players;
	public GameObject player; 
	public UniMoveController[] moves;
	public int[] moveInitIDs;
	public int playerCount; 
	public int numPlayers;
	private bool createPlayer;
	
	private Rect[] rects; 
	
	private Vector3[] positions; 
	private Quaternion rotations;

	//BED STUFF
	public Transform[] Spawners;
	public GameObject Bed;
	public GameObject BedObj;
	public int bedIndex;
	public bool bedSpawned;

	public GameObject bedTarget;

	
	void Start() 
	{
		createPlayer = false;
		playerCount = 0;
		//moves = gameObject.GetComponents<UniMoveController> ();
		/*if (numPlayers <= 4) {
			for (int num = 1; num <= 4; num++){
				GameObject.Find ("UICam " + num).SetActive(false);
			}
		}*/
		// CAMERA VIEWPORT
		rects = new Rect[7];
		
		rects[0].Set (-0.002f, 0.504f, 0.5f, 0.5f);	// multiplayer - p1
		rects[1].Set (0.502f, 0.504f, 0.5f, 0.5f);	// multiplayer - p2
		rects[2].Set (-0.002f, -0.004f, 0.5f, 0.5f);	// multiplayer - p3
		rects[3].Set (0.502f, -0.004f, 0.5f, 0.5f);	// multiplayer - p4
		
		rects[4].Set (-0.002f, 0.0f, 0.5f, 1.0f);	// two players - p1
		rects[5].Set (0.502f, 0.0f, 0.5f, 1.0f);	// two players - p2
		
		rects[6].Set (0.0f, 0.0f, 1.0f, 1.0f);	// single player - full screen
		
		
		positions = new Vector3[4] { 	new Vector3 (-1.10f, 1.424898f, 3.941933f),
										new Vector3 (-0.03585815f, 1.424898f, 3.941933f),
										new Vector3 (1.16f, 1.424898f, 3.941933f), 
										new Vector3 (2.383401f, 1.424898f, 3.366474f)};
		
		rotations = new Quaternion (0, 0, 0, 0);
		createPlayer = true;


		Spawners = new Transform[12];
		Spawners = GameObject.Find ("BedSpawner").GetComponentsInChildren<Transform>();
		bedSpawned = false;

//		if (GameManager.ins.status == GameState.GameStatus.Tutorial){
//			positions = new Vector3[1] { new Vector3 (0, 1.424898f, -6) };
//		}
	}
	
	
	void Update() 
	{
		if (!createPlayer){
		}
		else {
			if (GameManager.ins.mode == GameState.GameMode.Party){
				setBed();
			}

			if (StopManager() == false){
				createPlayers ();
				UniMoveSetPlayers();
			}
			if (StopManager ()){
				if (bedSpawned == true){
					setUI();
					UniMoveActivateComponents();
					this.enabled = false;
				}
			}
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. NO RETURN.
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
	 * NO ARG. NO RETURN.
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
				comp.bed = GameObject.Find ("BedObj");
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

	/*--------
	 * SET RANDOM BED LOCATION
	 * --------*/
	
	private void setBed(){
		if (!bedSpawned){
			bedIndex = Random.Range (0,Spawners.Length);
			
			if (BedObj == null){
				BedObj = Instantiate (Bed, Spawners [bedIndex].position, Spawners[bedIndex].rotation) as GameObject;
				bedTarget = GameObject.Find("BedTarget");
				bedTarget.transform.position = BedObj.transform.position;
				//BedObj.name = "BedObj";
			} else {
				Debug.Log("DESTROY BED SPAWNER");
				Destroy(GameObject.Find ("BedSpawner"));
				bedSpawned = true;
			}
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. NO RETURN.
	 * set the UniMove controller to the player (add component to the player)
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void UniMoveSetPlayers(){ 
		//print ("entered UniMoveSetPlayers()");
		/*int moveCount = 0;
		for(int i = 0; i < moves.Length; i++){
			UniMoveController mv; 
			//print ("checking move id" + moves[i].id);


			if (moves[i].id > 0){
				mv = players[moveCount].GetComponent<UniMoveController>();
				if (mv == null){
					UniMoveDisplay display = players[moveCount].GetComponent<UniMoveDisplay>();
					if (moves[i].id == display.id){
						//print ("adding move " + moves[i].id + " to player " + display.id);
						mv = players[moveCount].AddComponent<UniMoveController>() as UniMoveController;
						mv.Init (i); 
						mv.id = display.id;
						//createPlayer = false;
						moveCount++; 
					}
					//print ("finished adding move to player");
				}
			}
		}*/
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
	 * NO ARG. NO RETURN.
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
	
	private bool StopManager(){
		if (playerCount >= numPlayers){
			print ("all heads created");
			return true;
		}
		return false; 
	}
}
