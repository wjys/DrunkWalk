using UnityEngine;
using System;
using System.Collections.Generic;

public class UniMoveManager : MonoBehaviour 
{
	// We save a list of Move controllers.
	public List<UniMoveController> moves = new List<UniMoveController>();
	public GameObject[] players;
	public GameObject player; 
	public int moveCount; 
	public int numPlayers;
	private bool createPlayer;
	private bool setPlayer;

	private Rect[] rects; 

	private Vector3[] positions; 
	private Quaternion rotations;
	
	void Start() 
	{
		players = new GameObject[4];

		// CAMERA VIEWPORT
		rects = new Rect[7];

		rects[0].Set (0, 0, 0.5f, 0.5f);		// multiplayer - p1
		rects[1].Set (0.5f, 0, 0.5f, 0.5f);		// multiplayer - p2
		rects[2].Set (0, 0.5f, 0.5f, 0.5f);		// multiplayer - p3
		rects[3].Set (0.5f, 0.5f, 0.5f, 0.5f);	// multiplayer - p4

		rects[4].Set (0, 0, 0.5f, 1f);			// two players - p1
		rects[5].Set (0.5f, 0, 0.5f, 1f);		// two players - p2

		rects[6].Set (0, 0, 1f, 1f);			// single player - full screen


		positions = new Vector3[4] { 	new Vector3 (-0.03585815f, 1.424898f, 3.941933f), 
										new Vector3 (2.383401f, 1.424898f, 3.366474f),
										new Vector3 (-0.03585815f, 1.424898f, 3.941933f),
										new Vector3 (-0.03585815f, 1.424898f, 3.941933f)};

		rotations = new Quaternion (0, 0, 0, 0);

		moveCount = 0; 
		UniMoveInit (); 
		setPlayer = false; 

	}
	
	
	void Update() 
	{
		if (StopManager () == false) {
			UniMoveSetID ();
			if (createPlayer)
				createPlayers();
			if (setPlayer)
				UniMoveSetPlayers ();
		} 
		else {
			setUI ();
			UniMoveActivateComponents();
			this.enabled = false; 
		}
	}
	
	void HandleControllerDisconnected (object sender, EventArgs e)
	{
		// TODO: Remove this disconnected controller from the list and maybe give an update to the player
	}



	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. NO RETURN.
	 * API Code to initialize the move controllers within this manager 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void UniMoveInit(){
		Time.maximumDeltaTime = 0.05f;
		
		int count = UniMoveController.GetNumConnected();
		
		// Iterate through all connections (USB and Bluetooth)
		for (int i = 0; i < count; i++) 
		{	
			UniMoveController move = gameObject.AddComponent<UniMoveController>();	// It's a MonoBehaviour, so we can't just call a constructor

			// Remember to initialize!
			if (!move.Init(i)) 
			{	
				Destroy(move);	// If it failed to initialize, destroy and continue on
				continue;
			}
			
			// This example program only uses Bluetooth-connected controllers
			PSMoveConnectionType conn = move.ConnectionType;
			if (conn == PSMoveConnectionType.Unknown || conn == PSMoveConnectionType.USB) 
			{
				Destroy(move);
			}
			else 
			{
				moves.Add(move);
				
				move.OnControllerDisconnected += HandleControllerDisconnected;

			}
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. NO RETURN.
	 * when a player taps the Move button on their controller, set the ID on the move controller to the first available player
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void UniMoveSetID(){
		foreach (UniMoveController move in moves){
			print (move.id);
			if (move.id == 0){
				if (move.GetButtonUp(PSMoveButton.Move)){
					switch (moveCount){
					case 0:
						move.SetLED (Color.cyan);
						move.id = 1;
						createPlayer = true;
						return;
					case 1:
						if (move.id == 0){
							move.SetLED (Color.red);
							move.id = 2;
							createPlayer = true;
						}
						return;
					case 2:
						if (move.id == 0){
							move.SetLED (Color.yellow);
							move.id = 3;
							createPlayer = true;
						}
						return;
					case 3:
						if (move.id == 0){
							move.SetLED (Color.magenta);
							move.id = 4; 
							createPlayer = true;
						}
						return;
					default:
						return;
					}
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
		players [moveCount] = Instantiate (player, positions [moveCount], rotations) as GameObject;

		// SET UP THE ID OF THE HEAD AND COMPONENTS
		players [moveCount].name = "Head " + (moveCount + 1);
		players [moveCount].GetComponent<DrunkMovement> ().id = moveCount + 1;
		players [moveCount].GetComponent<UniMoveDisplay> ().id = moveCount + 1; 

		// SET UP THE CAMERAS (ON HEAD'S CHILDREN)
		Camera[] cams;

		switch (moveCount) {
		case 0:
			cams = players [moveCount].GetComponentsInChildren<Camera> ();
			foreach (Camera cam in cams){
				cam.rect = (rects[6]);
			}
			break;
		case 1:
			for (int i = 0; i < moveCount; i++){
				cams = players[i].GetComponentsInChildren<Camera>();
				foreach (Camera cam in cams){
					cam.rect = (rects [i+4]);
				}
			}
			break;
		case 2:
			for (int i = 0; i < moveCount; i++){
				cams = players[i].GetComponentsInChildren<Camera>();
				foreach (Camera cam in cams){
					cam.rect = (rects [i]);
				}
			}
			break;
		case 3:
			cams = players [moveCount].GetComponentsInChildren<Camera> ();
			foreach (Camera cam in cams){
				cam.rect = (rects[3]);
			}
			break;
		default:
			break;
		}
		setPlayer = true;
		createPlayer = false; 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. NO RETURN.
	 * set the UniMove controller to the player (add component to the player)
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void UniMoveSetPlayers(){ 
		for(int i = 0; i < moves.Count; i++){
			UniMoveController mv; 

			if (moves[i].id > 0){
				mv = players[moveCount].GetComponent<UniMoveController>();
				if (mv == null){
					UniMoveDisplay display = players[moveCount].GetComponent<UniMoveDisplay>();
					if (moves[i].id == display.id){
						print ("ADD UNIMOVE CONTROLLER TO PLAYER");
						mv = players[moveCount].AddComponent<UniMoveController>() as UniMoveController;
						mv.Init (i); 
						mv.id = display.id;
					}
					setPlayer = false;
					moveCount++;
					break;
				}
			}
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. NO RETURN.
	 * set UICam viewports and assign the scripts the children's components call
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void setUI(){
		Camera ui;
		
		// SINGLE PLAYER MODE
		if (numPlayers == 1) {
			ui = GameObject.Find ("UICam 1").camera;
			ui.rect = rects[6];
			ui.GetComponentInChildren <Eyelids>().me = GameObject.Find ("Head 1").GetComponent<DrunkMovement>();
			ui.GetComponentInChildren<Compass>().me =  GameObject.Find ("Head 1");
			ui.GetComponentInChildren<Ouch>().collision = GameObject.Find ("Head 1").GetComponent<Collision>();
			ui.gameObject.SetActive(true);
		}
		
		// 2 PLAYER MODE
		else if (numPlayers == 2) {
			for (int i = 1; i < numPlayers+1; i++){
				ui = GameObject.Find ("UICam " + i).camera;
				ui.rect = rects[i+3];
				ui.GetComponentInChildren <Eyelids>().me = GameObject.Find ("Head " + i).GetComponent<DrunkMovement>();
				ui.GetComponentInChildren<Compass>().me =  GameObject.Find ("Head " + i);
				ui.GetComponentInChildren<Ouch>().collision = GameObject.Find ("Head " + i).GetComponent<Collision>();
				ui.gameObject.SetActive(true);
			}
		}
		
		// MULTIPLAYER (3+) MODE
		else if (numPlayers >= 3){
			for (int i = 1; i < numPlayers+1; i++){
				ui = GameObject.Find ("UICam " + i).camera;
				ui.rect = rects[i-1];
				ui.GetComponentInChildren <Eyelids>().me = GameObject.Find ("Head " + i).GetComponent<DrunkMovement>();
				ui.GetComponentInChildren<Compass>().me =  GameObject.Find ("Head " + i);
				ui.GetComponentInChildren<Ouch>().collision = GameObject.Find ("Head " + i).GetComponent<Collision>();
				ui.gameObject.SetActive(true);
			}
		}
	}


	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. NO RETURN.
	 * when all the players have been set, activate the player components
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void UniMoveActivateComponents(){
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
		if (moveCount == numPlayers){
			return true;
		}
		return false; 
	}
}
