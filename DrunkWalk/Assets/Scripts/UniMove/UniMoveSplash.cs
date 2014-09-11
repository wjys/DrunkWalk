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
	public GameManager manager;
	public UniMoveController p1;
	public UniMoveController p2;
	public UniMoveController p3;
	public UniMoveController p4;

	// move input bounds
	public float moveBoundRight;
	public float moveBoundLeft;
	public float moveBoundFront;
	public float moveBoundBack;
	public float moveBoundFlipped;
	public bool joinedGame;

	public GameObject multiMarker;
	public int multiMarkerCt;
	public int selectedMarkers;

	public Transform[] handle;
	private Vector3[] newHandle;

	public float smooth;
	public bool remadeMarkers;

	public Color[] markerColors;

	/* --------------------------------------------------------------------------------------------------------------------------
	 * START: reset variables and get the moves paired with the computer
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void Start() 
	{ 
		manager = gameObject.GetComponent<GameManager>();
		if (manager.track == 0){

			numPlayers = 0;
			UniMoveInit ();

			if (moves.Count == 0){
				DestroyUniMove();
			}

			handle = new Transform[4];
			newHandle = new Vector3[4];
			markerColors = new Color[4];
			selectedMarkers = 0;
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * UPDATE:
	 * (0) only do things if we're in the splash scene 
	 * (1) get access to running MainMenu script
	 * (2) remake the markers if looping through menu again
	 * (3) constantly check if another player joins in if less than 4 players currently in the game
	 * (4) turn on markers if in multiplayer character select menu
	 * (5) if all moves/players have selected a character, move on to multiplayer mode select
	 * (6) also constantly read input from players to navigate through menu (menuActions)
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	void Update() 
	{
		if (GameManager.ins.status == GameState.GameStatus.Splash){

			if (main == null){
				main = GameObject.Find ("MainMenu").GetComponent<MainMenu> ();
			}

			// create move marker objects
			if (manager.track != 0 && !remadeMarkers){
				if (multiMarkerCt > 0){
					multiMarkerCt = 0;
				}
				foreach (UniMoveController move in moves){
					if (move.id > 0){
						GameState.GameController = GameState.GameController.move;
						print ("making marker " + move.id + " after looping back to menu");
						createMarker (move);
					}
				}
				remadeMarkers = true;
			}

			if (((main.menuNumPublic < 5 && main.menuNumPublic != 1) || main.menuNumPublic == 7) && numPlayers > 1){
				numPlayers = 1;
			}

			// keep checking if a new player joins the game
			if (p1 == null || ((main.menuNumPublic == 1 || (main.menuNumPublic >= 5 && main.menuNumPublic != 7)) && numPlayers < 4)){
				if (numPlayers == 1 && p2 != null){
					foreach (UniMoveController move in moves){
						if (move.id > 0 && move.id != 1){
							numPlayers++;
						}
					}
				}
				UniMoveSetID ();
			}

			// MULTI CHARACTER SELECT
			if (main.menuNumPublic == 5) {
				TurnOnMarkerComponents();

				if (selectedMarkers == numPlayers){
					//main.mcitems[0].command();
					//main.LerpCam();
				}
			}

			menuActions ();
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * FIXED UPDATE: lerp the lil move controllers to be displayed when a player joins in
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void FixedUpdate (){

		// LIL UNIMOVES POP UP
		if (GameManager.ins.status == GameState.GameStatus.Splash){
			foreach (UniMoveController move in moves){
				if (move.id != 0){
					handle[move.id-1] = GameObject.Find ("HANDLE"+move.id).transform;
					//newHandle[move.id-1] = new Vector3 (handle[move.id-1].position.x, -2.2f, handle[move.id-1].position.z);
					handle[move.id-1].position = Vector3.Lerp (handle[move.id-1].position, new Vector3 (handle[move.id-1].position.x, -2.2f, handle[move.id-1].position.z), smooth * Time.deltaTime);
	            }
	        }
		}
    }

	/* --------------------------------------------------------------------------------------------------------------------------
	 * REMOVE UNIMOVE: destroy gamemanager move scripts (use in OPTIONS/switch to mouse)
	 * -------------------------------------------------------------------------------------------------------------------------- */

	public void DestroyUniMove(){
		GameManager.ins.SendMessage ("NoMove");
		/*Destroy (this.gameObject.GetComponent<UniMoveGame>());
		Destroy (this.gameObject.GetComponent<UniMoveTest>());
		Destroy (this.gameObject.GetComponent<UniMoveManager>());
		Destroy (this);*/


		// maybe instead of destroying disable, for SWITCHING controllers ??
		this.gameObject.GetComponent<UniMoveGame>().enabled = false;
		this.gameObject.GetComponent<UniMoveTest>().enabled = false;
		this.gameObject.GetComponent<UniMoveManager>().enabled = false;
		this.enabled = false;
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * UNIMOVE INIT: API Code to initialize the move controllers within this manager 
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
	 * UNIMOVE SET ID: when a player taps the Move button, set the ID on the move controller to the first available ID
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
						p1 = move;
						StartCoroutine (startRead ());
						return;
					case 1:
						move.SetLED (markerColors[1]);
						move.id = 2;
						numPlayers = 2;
						createMarker (move);
						p2 = move;
						return;
					case 2:
						move.SetLED (markerColors[2]);
						move.id = 3;
						numPlayers = 3;
						createMarker (move);
						p3 = move;
						return;
					case 3:
						move.SetLED (markerColors[3]);
						move.id = 4; 
						numPlayers = 4;
						createMarker (move);
						p4 = move;
						return;
					default:
						return;
					}
				}
			}
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * CREATE MARKER: instantiate from marker prefab
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void createMarker(UniMoveController move){
		if (move.id > 0) {
			GameObject mark = Instantiate (multiMarker) as GameObject;
			mark.GetComponent<MultiMarker>().id = move.id;
			mark.GetComponent<MultiMarker>().name = "MultiMarker " + move.id;
			mark.GetComponent<MultiMarker>().UniMove = move;
			mark.GetComponent<MultiMarker>().spriteID = move.id-1;
			multiMarkerCt++;
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * TURN ON MARKER COMPONENTS: activate marker sprites (show the marker)
	 * -------------------------------------------------------------------------------------------------------------------------- */

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
	 * UNIMOVE ALL PLAYERS IN. RETURN BOOL: true if ready to move on to level select
	 * if there are 4 players in or if all 2+ players are holding move button, move on to next game state/level select
	 * -------------------------------------------------------------------------------------------------------------------------- */

	public bool UniMoveAllPlayersIn(){
		bool holdMove = true;
		if (numPlayers > 1){
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
	 * SET GAME: set variables for the game in UniMoveGame
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void setGame(){
		print ("reset game move settings");
		UniMoveGame gm = gameObject.GetComponent<UniMoveGame> ();
		gm.numPlayers = numPlayers;
		gm.players = new GameObject[numPlayers];
		gm.moveInitIDs = new int[numPlayers];
		for (int i = 0; i < numPlayers; i++){
			for (int j = 0; j < moves.Count; j++){
				if (moves[j].id == i+1){
					gm.moveInitIDs[i] = j;
					break;
				}
			}
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * SET NUM PLAYERS: when all players ready, reset #players, winners/losers and indeces in GameManager
	 * -------------------------------------------------------------------------------------------------------------------------- */

	public void setNumPlayers(){
		print ("reset players/winners/losers in GameManager");
		manager.numOfPlayers = numPlayers;
		manager.winnerIndex = 0;
		manager.loserIndex = 0;
		manager.winners = new int[numPlayers];
		manager.losers = new int[numPlayers];
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * MENU ACTIONS: inputs to navigate through the menu with the move controller
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void menuActions(){
		//if (main.menuNumPublic < 5){
			if (p1 != null){
				if (p1.ax > moveBoundLeft)	main.tiltL = true;
				else main.tiltL = false;

				if (p1.ax < moveBoundRight) main.tiltR = true;
				else main.tiltR = false;

				if (p1.az > moveBoundFront) main.tiltF = true;
				else main.tiltF = false;

				if (p1.az < moveBoundBack) main.tiltB = true;
				else main.tiltB = false;

				if (p1.ay < moveBoundFront && p1.az < moveBoundBack) main.flipped = true;
				else main.flipped = false;

				if (joinedGame){
					if (p1.GetButtonUp(PSMoveButton.Move)) main.movePressed = true;
					else main.movePressed = false;
				}

				if (p1.GetButtonUp(PSMoveButton.Circle) || p1.GetButtonUp (PSMoveButton.Cross) || p1.GetButtonUp(PSMoveButton.Square) || p1.GetButtonUp (PSMoveButton.Triangle))
					main.cancelSelection = true;
				else main.cancelSelection = false;

			}
		/*}
		else {
			switch (numPlayers){
			case 1:
				if (p1.ax > moveBoundLeft)	main.tiltL = true;
				else main.tiltL = false;
				
				if (p1.ax < moveBoundRight) main.tiltR = true;
				else main.tiltR = false;
				
				if (p1.az > moveBoundFront) main.tiltF = true;
				else main.tiltF = false;
				
				if (p1.az < moveBoundBack) main.tiltB = true;
				else main.tiltB = false;

				if (joinedGame){
					if (p1.GetButtonUp(PSMoveButton.Move)) main.movePressed = true;
					else main.movePressed = false;
				}
				
				if (p1.GetButtonUp(PSMoveButton.Circle) || p1.GetButtonUp (PSMoveButton.Cross) || p1.GetButtonUp(PSMoveButton.Square) || p1.GetButtonUp (PSMoveButton.Triangle))
					main.cancelSelection = true;
				else main.cancelSelection = false;
				break;
			case 2:
				if (p1.ax > moveBoundLeft || p2.ax > moveBoundLeft)	main.tiltL = true;
				else main.tiltL = false;
				
				if (p1.ax < moveBoundRight || p2.ax < moveBoundRight) main.tiltR = true;
				else main.tiltR = false;
				
				if (p1.az > moveBoundFront || p2.az > moveBoundFront) main.tiltF = true;
				else main.tiltF = false;
				
				if (p1.az < moveBoundBack || p2.az < moveBoundBack) main.tiltB = true;
				else main.tiltB = false;

				if (joinedGame){
					if ((p1.GetButtonUp(PSMoveButton.Move) && p2.GetButtonUp(PSMoveButton.Move)))
						main.movePressed = true;
					else main.movePressed = false;
				}
				
				if ((p1.GetButtonUp(PSMoveButton.Circle) || p1.GetButtonUp (PSMoveButton.Cross) || p1.GetButtonUp(PSMoveButton.Square) || p1.GetButtonUp (PSMoveButton.Triangle)) &&
				    (p2.GetButtonUp(PSMoveButton.Circle) || p2.GetButtonUp (PSMoveButton.Cross) || p2.GetButtonUp(PSMoveButton.Square) || p2.GetButtonUp (PSMoveButton.Triangle)))
					main.cancelSelection = true;
				else main.cancelSelection = false;
				break;
			case 3:
				if (p1.ax > moveBoundLeft || p2.ax > moveBoundLeft || p3.ax > moveBoundLeft)	main.tiltL = true;
				else main.tiltL = false;
				
				if (p1.ax < moveBoundRight || p2.ax < moveBoundRight || p3.ax < moveBoundRight) main.tiltR = true;
				else main.tiltR = false;
				
				if (p1.az > moveBoundFront || p2.az > moveBoundFront || p3.az > moveBoundFront) main.tiltF = true;
				else main.tiltF = false;
				
				if (p1.az < moveBoundBack || p2.az < moveBoundBack || p3.az < moveBoundBack) main.tiltB = true;
				else main.tiltB = false;
				
				if (joinedGame){
					if (p1.GetButtonUp(PSMoveButton.Move) && p2.GetButtonUp(PSMoveButton.Move) && p3.GetButtonUp (PSMoveButton.Move)) 
							main.movePressed = true;
					else main.movePressed = false;
				}
				
				if ((p1.GetButtonUp(PSMoveButton.Circle) || p1.GetButtonUp (PSMoveButton.Cross) || p1.GetButtonUp(PSMoveButton.Square) || p1.GetButtonUp (PSMoveButton.Triangle)) &&
				    (p2.GetButtonUp(PSMoveButton.Circle) || p2.GetButtonUp (PSMoveButton.Cross) || p2.GetButtonUp(PSMoveButton.Square) || p2.GetButtonUp (PSMoveButton.Triangle)) && 
				    (p3.GetButtonUp(PSMoveButton.Circle) || p3.GetButtonUp (PSMoveButton.Cross) || p3.GetButtonUp(PSMoveButton.Square) || p3.GetButtonUp (PSMoveButton.Triangle)))
					main.cancelSelection = true;
				else main.cancelSelection = false;
				break;

			case 4:
				if (p1.ax > moveBoundRight || p2.ax > moveBoundLeft || p3.ax > moveBoundLeft || p4.ax > moveBoundLeft)	main.tiltL = true;
				else main.tiltL = false;
				
				if (p1.ax < moveBoundRight || p2.ax < moveBoundRight || p3.ax < moveBoundRight || p4.ax < moveBoundRight) main.tiltR = true;
				else main.tiltR = false;
				
				if (p1.az > moveBoundFront || p2.az > moveBoundFront || p3.az > moveBoundFront || p4.az > moveBoundFront) main.tiltF = true;
				else main.tiltF = false;
				
				if (p1.az < moveBoundBack || p2.az < moveBoundBack || p3.az < moveBoundBack || p4.az < moveBoundBack) main.tiltB = true;
				else main.tiltB = false;
				
				if (joinedGame){
					if (p1.GetButtonUp(PSMoveButton.Move) && p2.GetButtonUp(PSMoveButton.Move) && p3.GetButtonUp (PSMoveButton.Move) && p4.GetButtonUp (PSMoveButton.Move)) 
						main.movePressed = true;
					else main.movePressed = false;
				}

				if ((p1.GetButtonUp(PSMoveButton.Circle) || p1.GetButtonUp (PSMoveButton.Cross) || p1.GetButtonUp(PSMoveButton.Square) || p1.GetButtonUp (PSMoveButton.Triangle)) &&
				    (p2.GetButtonUp(PSMoveButton.Circle) || p2.GetButtonUp (PSMoveButton.Cross) || p2.GetButtonUp(PSMoveButton.Square) || p2.GetButtonUp (PSMoveButton.Triangle)) && 
				    (p3.GetButtonUp(PSMoveButton.Circle) || p3.GetButtonUp (PSMoveButton.Cross) || p3.GetButtonUp(PSMoveButton.Square) || p3.GetButtonUp (PSMoveButton.Triangle)) &&
				    (p4.GetButtonUp(PSMoveButton.Circle) || p4.GetButtonUp (PSMoveButton.Cross) || p4.GetButtonUp(PSMoveButton.Square) || p4.GetButtonUp (PSMoveButton.Triangle)))
					main.cancelSelection = true;
				else main.cancelSelection = false;
				break;
			}
		}*/
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * START READ COROUTINE: when a player joins in, don't let move confirm in menu (DELAY)
	 * -------------------------------------------------------------------------------------------------------------------------- */

	IEnumerator startRead(){
		yield return new WaitForSeconds (0.5f);
		joinedGame = true;
	}
}