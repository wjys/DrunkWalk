using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// INITS THE MOVE CONTROLLERS AND MANAGES THEM
// GENERATES MULTI MARKERS WITH APPROPRIATE CONTROLLER ASSIGNED TO EACH

public class UniMoveSplash : MonoBehaviour 
{
	// We save a list of Move controllers.
	public List<UniMoveController> moves = new List<UniMoveController>();
	public int numPlayers;
	public MainMenu main;

	public GameObject multiMarker;
	public int multiMarkerCt;
	public bool allMultiMarkersMade;

	public Transform[] handle;
	private Vector3[] newHandle;

	public float smooth;
	public bool movePaired;

	public Color[] markerColors;

	void Start() 
	{
		numPlayers = 0; 
		UniMoveInit (); 
		handle = new Transform[4];
		newHandle = new Vector3[4];
		markerColors = new Color[4];
		//main = GameObject.Find ("MainMenu").GetComponent<MainMenu> ();
	}
	
	
	void Update() 
	{
		if (GameManager.ins.status == GameState.GameStatus.Splash){
			if (main == null){
				main = GameObject.Find ("MainMenu").GetComponent<MainMenu> ();
			}
			setMoveColour ();
			UniMoveSetID ();
			if (multiMarkerCt < numPlayers){
				allMultiMarkersMade = false;
				movePaired = false;
			}

			if (main.menuNumPublic == 1){
				print ("in main menu");
	            if (numPlayers > 1) {
					// DISPLAY MULTIPLAYER MENU OPTION
					print ("DISPLAY MULTIPLAYER OPTION");
				}
			}
			// MULTI CHARACTER SELECT
			if (main.menuNumPublic == 5) {
				TurnOnMarkerComponents();
			}
			if (UniMoveAllPlayersIn ()) {
				GameManager.ins.mode = GameState.GameMode.Race;
				GameManager.ins.status = GameState.GameStatus.Game;
				Application.LoadLevel ("WastedMulti");
				setGame ();
				this.enabled = false;
			}
		}
	}

	void FixedUpdate (){

		// LIL UNIMOVES POP UP
		foreach (UniMoveController move in moves){
			if (move.id !=0){
				handle[move.id-1] = GameObject.Find ("HANDLE"+move.id).transform;
				//newHandle[move.id-1] = new Vector3 (handle[move.id-1].position.x, -2.2f, handle[move.id-1].position.z);
				handle[move.id-1].position = Vector3.Lerp (handle[move.id-1].position, new Vector3 (handle[move.id-1].position.x, -2.2f, handle[move.id-1].position.z), smooth * Time.deltaTime);
            }
        }
    }
    
	/*void HandleControllerDisconnected (object sender, EventArgs e)
	{
		// TODO: Remove this disconnected controller from the list and maybe give an update to the player
	}*/
	
	
	
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
				
				//move.OnControllerDisconnected += HandleControllerDisconnected;
				
			}
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. NO RETURN.
	 * when a player taps the Move button on their controller, set the ID on the move controller to the first available player
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void UniMoveSetID(){
		foreach (UniMoveController move in moves){
			//print (move.id);
			if (move.id == 0){
				if (move.GetButtonUp(PSMoveButton.Move)){
					switch (numPlayers){
					case 0:
						move.SetLED (markerColors[0]);
						move.id = 1;
						numPlayers = 1;
						createMarker (move);
						return;
					case 1:
						move.SetLED (markerColors[1]);
						move.id = 2;
						numPlayers = 2;
						createMarker (move);
						return;
					case 2:
						move.SetLED (markerColors[2]);
						move.id = 3;
						numPlayers = 3;
						createMarker (move);
						return;
					case 3:
						move.SetLED (markerColors[4]);
						move.id = 4; 
						numPlayers = 4;
						createMarker (move);
						return;
					default:
						return;
					}
				}
			}
		}
	}

	private void createMarker(UniMoveController move){
		if (move.id > multiMarkerCt) {
			GameObject mark = Instantiate (multiMarker) as GameObject;
			mark.GetComponent<MultiMarker>().id = move.id;
			mark.GetComponent<MultiMarker>().name = "MultiMarker " + move.id;
			mark.GetComponent<MultiMarker>().UniMove = move;
			mark.GetComponent<MultiMarker>().spriteID = move.id-1;
			multiMarkerCt++;
			allMultiMarkersMade = true;
		}
	}

	private void TurnOnMarkerComponents(){
		for (int i = 1; i <= numPlayers; i++){
			GameObject marker = GameObject.Find ("MultiMarker " + i);
			if (!marker.GetComponent<SpriteRenderer>().enabled){
				marker.GetComponent<SpriteRenderer>().enabled = true;
			}
		}
		for (int i = 1; i <= numPlayers; i++){
			GameObject marker = GameObject.Find ("MultiMarker " + i); 
			if (!marker.GetComponent<MultiMarker>().enabled){
				marker.GetComponent<MultiMarker>().enabled = true;
			}
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. RETURN BOOL: true if ready to move on to level select
	 * if there are 4 players in or if all 2+ players are holding move button, move on to next game state/level select
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private bool UniMoveAllPlayersIn(){
		bool holdMove = true;
		if (numPlayers > 1){
			if (numPlayers == 4) {
				return true;
			}

			foreach (UniMoveController move in moves) {
				if (move.id != 0){
					if (!move.GetButton (PSMoveButton.Move)){
						holdMove = false;
					}
				}
			}
			if (holdMove) {
				return true;
			}
		}
		return false;

	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * SET MOVE CONTROLLER COLOUR
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void setMoveColour(){
		foreach (UniMoveController move in moves){
			switch (move.id) {
			case 0:
				break;
			case 1:
				move.SetLED (Color.cyan);
				break;
			case 2:
				move.SetLED (Color.magenta);
				break;
			case 3:
				move.SetLED (Color.yellow);
				break;
			case 4:
				move.SetLED (Color.green);
				break;
			default:
				break;

			}
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * SET NECESSARY VARIABLES FOR THE GAME IN THE UNIMOVEGAME MANAGER SCRIPT
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void setGame(){
		UniMoveGame gm = gameObject.GetComponent<UniMoveGame> ();
		gm.numPlayers = numPlayers;
		gm.players = new GameObject[numPlayers];

		gm.enabled = true;
	}
}