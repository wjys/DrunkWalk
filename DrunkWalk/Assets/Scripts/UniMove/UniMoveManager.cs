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
	private Rect rect1a;
	private Rect rect2a;
	private Rect rect1b;
	private Rect rect2b;
	private Rect rect3;
	private Rect rect4;

	private Vector3[] positions; 
	private Quaternion rotations;
	
	void Start() 
	{
		players = new GameObject[4];
		rects = new Rect[7];
		rects[0].Set (0, 0, 1f, 1f);
		rects[1].Set (0.5f, 0, 0.5f, 1f);
		rects[2].Set (0, 0.5f, 0.5f, 0.5f);
		rects[3].Set (0.5f, 0.5f, 0.5f, 0.5f);

		// players 1-2 if 3 or more players
		rects[4].Set (0, 0, 0.5f, 1f);
		rects[5].Set (0, 0, 0.5f, 0.5f);
		rects[6].Set (0.5f, 0, 0.5f, 0.5f);

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


	// NOT USED - simple button checks
	private void UniMoveButtons(){
		foreach (UniMoveController move in moves) 
		{
			// Instead of this somewhat kludge-y check, we'd probably want to remove/destroy
			// the now-defunct controller in the disconnected event handler below.
			if (move.Disconnected) continue;
			
			// Button events. Works like Unity's Input.GetButton
			if (move.GetButtonDown(PSMoveButton.Circle)){
				Debug.Log("Circle Down");
			}
			if (move.GetButtonUp(PSMoveButton.Circle)){
				Debug.Log("Circle UP");
			}
			
			// Change the colors of the LEDs based on which button has just been pressed:
			if (move.GetButtonDown(PSMoveButton.Circle)) 		move.SetLED(Color.cyan);
			else if(move.GetButtonDown(PSMoveButton.Cross)) 	move.SetLED(Color.red);
			else if(move.GetButtonDown(PSMoveButton.Square)) 	move.SetLED(Color.yellow);
			else if(move.GetButtonDown(PSMoveButton.Triangle)) 	move.SetLED(Color.magenta);
			else if(move.GetButtonDown(PSMoveButton.Move)) 		move.SetLED(Color.black);
			
			// Set the rumble based on how much the trigger is down
			move.SetRumble(move.Trigger);
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
		players [moveCount].name = "Head " + (moveCount + 1);
		Camera[] cams = players [moveCount].GetComponentsInChildren<Camera> ();
		foreach (Camera cam in cams){
			cam.rect = (rects [moveCount]);
		}
		players [moveCount].GetComponent<DrunkMovement> ().id = moveCount + 1;
		players [moveCount].GetComponent<UniMoveDisplay> ().id = moveCount + 1; 
		if (moveCount == 1) {
			cams = players[0].GetComponentsInChildren<Camera> ();
			foreach (Camera cam in cams){
				cam.rect = (rects[4]);
			}
		}
		/*if (moveCount >= 1) {
		GameObject ui;
			foreach (Camera cam in cams){
				if (cam.name.Equals("UICam")){
					ui = cam.gameObject;
					ui.layer = 7+moveCount;
					foreach (Transform trans in ui.GetComponentsInChildren<Transform>()){
						trans.gameObject.layer = 7+moveCount;
					}
					break;
				}
			}
		}*/
		if (moveCount >= 2) {
			cams = players[0].GetComponentsInChildren<Camera> ();
			foreach (Camera cam in cams){
				cam.rect = (rects[5]);
			}
			cams = players[1].GetComponentsInChildren<Camera> ();
			foreach (Camera cam in cams){
				cam.rect = (rects[6]);
			}
		}
		setPlayer = true;
		createPlayer = false; 
	}
	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. NO RETURN.
	 * set UICam layers and culling mask
	 * -------------------------------------------------------------------------------------------------------------------------- */
	private void setUI(){
		for (int i = 1; i < numPlayers; i++) {
			GameObject ui = GameObject.Find ("/Head " + (i+1) + "/UICam");
			ui.layer = 7+i;
			foreach (Transform trans in ui.GetComponentsInChildren<Transform>()){
				trans.gameObject.layer = 7+i;
			}
			Camera cam = ui.GetComponent<Camera>();
			cam.cullingMask = 7;
		}
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
	 * when all the players have been set, activate the player components
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void UniMoveActivateComponents(){
		print ("ENABLE PLAYER COMPONENTS!");
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
