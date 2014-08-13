using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//This
	static public GameManager ins;

	//Global Variables
	public GameState.GameStatus status;
	public GameState.GameMode mode;

	//Menu Assets
	public GUISkin skin;

	public GameObject pauseMenu;
	public GameObject audioManager;
	public GameObject mainMenu;

	private GameObject pauseMenuIns;
	private GameObject mainMenuIns;
	private float lastRealtimeSinceStartup;

	// COMPONENTS IN MANAGER TO ENABLE/DISABLE
	public UniMoveSplash splashScript;
	public UniMoveGame gameScript;
	public EndScreen endScript;
	public UniMoveController[] moves;

	//Bools
	private bool paused = false;
	private bool menu = false;
	public bool game;
	private bool playing;

	//Character
	public int chosenChar;
	public int[] multiChosenChar;

	//Difficulty
	public GameObject DiffObj;
	public int diffInt;
	public int JncInt, BeerInt, VodkaInt, GinInt;

	//Level
	public static int levelInt;
	public int track;

	/*
	 * STUFF TO DO ABOUT LEVELS:
	 * Once we ENTER the game, WHICHEVER LEVEL IT IS depending on the MODE--
	 * 		STEALTH : Instantiate a prefab of extra objects
	 * 		PARTY : Instantiate bed in a random location
	 */

	//Winner
	public int score;

	public int winner;
	public int numOfPlayers;

	public int winnerIndex;
	public int loserIndex;
	public int[] winners;
	public int[] losers;

	public bool SingleWin;
	public bool SingleLose;
	public bool MultiGO;

	//Result
	public Sprite[] resultChars;

	void Awake () {
		//Setup instance
		DontDestroyOnLoad(this);

		if (GameObject.Find ("GameManager") != null){
			Destroy (this.gameObject);
		}


		ins = this;
		chosenChar = 0;

		//Set Game Status
		if (game){
			status = GameState.GameStatus.Game;
		}

		//Don't show mouse
		//Screen.showCursor = false;

		//Setup Audio Manager
		if (AudioManager.ins == null){
			(Instantiate(audioManager) as GameObject).SendMessage("Initialize");
		}

		//Menu Instances
		if (status == GameState.GameStatus.Splash){
			mainMenuIns = Instantiate(mainMenu) as GameObject;
			mainMenuIns.name = "MainMenu";
			//mainMenuIns.SetActive(false);
		}

		pauseMenuIns = Instantiate(pauseMenu) as GameObject;
		pauseMenuIns.name = "PauseMenu";
		pauseMenuIns.SetActive(false);

		splashScript 	= gameObject.GetComponent <UniMoveSplash>();
		gameScript 		= gameObject.GetComponent<UniMoveGame>();
		endScript 		= gameObject.GetComponent<EndScreen>();

		multiChosenChar = new int[4];
	}



	void Start () {

		DiffObj = GameObject.Find ("Difficulty");
	}


	void Update () {
		if (gameObject.GetComponent<UniMoveController>() != null){
			moves = gameObject.GetComponents<UniMoveController>();
			setMoveColors();
		}
		//print ("Scene: " + Application.loadedLevelName);

		//Pause Menu in Game, Main menu in Splash
		if (status == GameState.GameStatus.Scores){



			if (Application.loadedLevelName.Equals ("Scores")){
				ScoreManager scoreManager = GameObject.Find ("HighScores").GetComponent<ScoreManager>();
				scoreManager.score = score;
				scoreManager.enabled = true;
			}
			else {
				score = 0;
				if (Application.loadedLevelName.Equals ("Splash")){
					if (status != GameState.GameStatus.Splash){
						status = GameState.GameStatus.Splash;
					}
				}

				else if (Application.loadedLevelName.Equals ("Won") || Application.loadedLevelName.Equals("Lost")){
					if (status != GameState.GameStatus.End){
						status = GameState.GameStatus.End;
					}
				}

				else {
					if (status != GameState.GameStatus.Game){
						status = GameState.GameStatus.Game;
					}
				}
			}
		}
		else if (status == GameState.GameStatus.End){

			// switch to SPLASH status
			if (Application.loadedLevelName.Equals ("Splash")){
				if (status != GameState.GameStatus.Splash){
					status = GameState.GameStatus.Splash;
				}
			}

			else if (Application.loadedLevelName.Equals ("Scores")){
				if (status != GameState.GameStatus.Scores){
					status = GameState.GameStatus.Scores;
				}
			}

			else if (Application.loadedLevelName.Equals ("Won") || Application.loadedLevelName.Equals("Lost")){
				if (gameScript.enabled){
					gameScript.enabled = false;
				}
				if (splashScript.enabled){
					splashScript.enabled = false;
				}
				if (!endScript.enabled){
					endScript.enabled = true;
				}
				if (Application.loadedLevelName.Equals ("Won")){
					GUIText winText = GameObject.Find ("Winner").GetComponent<GUIText>();
					GUIText scoreText = GameObject.Find ("Score").GetComponent<GUIText>();
					SpriteRenderer winSprite = GameObject.Find ("ZZZ").GetComponent<SpriteRenderer>();
					if (SingleWin){
						winText.enabled = false;
						winSprite.enabled = true;
						scoreText.text = "you scored : " + score;
						scoreText.enabled = true;
					}
					else {
						GameObject.Find ("Winner").GetComponent<GUIText>().text = "PLAYER " + winner + " WINS";

						GameObject race, party;
						race = GameObject.Find ("RaceEnd");
						party = GameObject.Find ("PartyEnd");

						if (mode == GameState.GameMode.Race){
							//IF RACE RESULTS
							Destroy (party);

							//if 1 player is in the winner index, enable spriterenderer of Win1 with its Player & Character ID
							//	show Lose1, Lose2, Lose3 in the loser index with their IDs

							//if 2 players are in the winner index, show Win1, Win2 with their IDs
							//	show Lose1, Lose2 in the loser index with their IDs

							//if 3 players are in the winner index, show Win1, Win2, Win3 with their IDs
							// show Lose1 with its ID

						} else if (mode == GameState.GameMode.Party){
							//IF PARTY RESULTS
							Destroy (race);

							//Send appropriate Player ID and character ID to 1st, 2nd, 3rd, 4th place sprite renderer
						}
					}
				}
			}

			else {
				if (status != GameState.GameStatus.Game){
					status = GameState.GameStatus.Game;
				}
			}

			if (playing) playing = false;
		}
		else if (status == GameState.GameStatus.Game){

			if (Application.loadedLevelName.Equals ("Splash")){
				if (status != GameState.GameStatus.Splash){
					status = GameState.GameStatus.Splash;
				}
			}

			else if (Application.loadedLevelName.Equals ("Scores")){
				if (status != GameState.GameStatus.Scores){
					status = GameState.GameStatus.Scores;
				}
			}

			else if (Application.loadedLevelName.Equals ("Won") || Application.loadedLevelName.Equals("Lost")){
				if (status != GameState.GameStatus.End){
					status = GameState.GameStatus.End;
				}
			}

			else {
				if (!gameScript.enabled && !playing){
					gameScript.enabled = true;
					playing = true;
				}
				if (splashScript.enabled){
					splashScript.enabled = false;
				}
				if (endScript.enabled){
					endScript.enabled = false;
				}

				CheckWinLose ();

			 	if (Input.GetKeyDown("p")){
	          	 	if (!paused) {
						if (pauseMenuIns == null){
							pauseMenuIns = Instantiate (pauseMenu) as GameObject;
							pauseMenuIns.SetActive (true);
						}
	           	    	Pause();
	          	 	} else {
	           	    	UnPause();
	           		}
	           	}
			}

		} else if (status == GameState.GameStatus.Splash) {

			if (Application.loadedLevelName.Equals("Splash")){
				if (endScript.enabled || gameScript.enabled){
					endScript.enabled = false;
					gameScript.enabled = false;
				}

				if (!splashScript.enabled){
					splashScript.enabled = true;
				}

				if (track != 0){
					Destroy (GameObject.Find ("_GameManager"));
					Destroy (GameObject.Find ("_GameState"));
				}
				else {
					if (!gameObject.name.Equals("GameManager")){
						gameObject.name = "GameManager";
						GameObject.Find ("_GameState").name = "GameState";
					}
				}

				if (mainMenuIns == null){
					if (GameObject.Find ("MainMenu") != null){
						mainMenuIns = GameObject.Find ("MainMenu");
					}
					ins = this;
				}
			
				if (!mainMenuIns.activeSelf){
					mainMenuIns.SetActive(true);
				}
				
				/*if (Input.GetKeyDown("m")){
        		if (!menu) {
        			Menu();
        		} else {
        			UnMenu();
        		}
				Debug.Log ("MENU");
        	}*/
				//Menu ()
				if (DiffObj == null){
					DiffObj = GameObject.Find ("Difficulty");
				}
				else {
					JncInt = DiffObj.GetComponent<Difficulty>().drinkID[0];
					BeerInt = DiffObj.GetComponent<Difficulty>().drinkID[1];
					VodkaInt = DiffObj.GetComponent<Difficulty>().drinkID[2];
					GinInt = DiffObj.GetComponent<Difficulty>().drinkID[3];
					
					diffInt = DiffObj.GetComponent<Difficulty>().totalDrunk;
				}

				if (SingleWin){
					SingleWin = false;
				}

			}

			else if (Application.loadedLevelName.Equals ("Scores")){
				if (status != GameState.GameStatus.Scores){
					status = GameState.GameStatus.Scores;
				}
			}

			else if (Application.loadedLevelName.Equals ("Won") || Application.loadedLevel.Equals ("Lost")){
				status = GameState.GameStatus.End;
			}
			else {
				status = GameState.GameStatus.Game;
			}
        }
		else if (status == GameState.GameStatus.Tutorial){
			if (Application.loadedLevelName.Equals ("Splash")){
				if (status != GameState.GameStatus.Splash){
					status = GameState.GameStatus.Splash;
				}
			}
			
			else if (Application.loadedLevelName.Equals ("Won") || Application.loadedLevelName.Equals("Lost")){
				status = GameState.GameStatus.End;
			}
			
			else {
				if (!gameScript.enabled && !playing){
					gameScript.enabled = true;
					playing = true;
				}
				if (splashScript.enabled){
					splashScript.enabled = false;
				}
				if (endScript.enabled){
					endScript.enabled = false;
				}
			}
			CheckWinLose ();
		}
	}


	//Pause Menu Initialize
	public void Pause () {
		InGame[] objs = FindObjectsOfType(typeof(InGame)) as InGame[];
		foreach (var obj in objs) {
			obj.paused = true;
		}

		paused = true;
		pauseMenuIns.SetActive(paused);

	}

	public void UnPause () {
		InGame[] objs = FindObjectsOfType(typeof(InGame)) as InGame[];
		foreach (var obj in objs) {
			obj.paused = false;
		}

		paused = false;
		pauseMenuIns.SetActive(paused);

	}


	//Main Menu Initialize
	public void Menu() {
		menu = true;
		mainMenuIns.SetActive (menu);
	}

	public void UnMenu() {
		menu = false;
		mainMenuIns.SetActive (menu);
	}

	private void CheckWinLose(){
		if (mode == GameState.GameMode.ScoreAttack || mode == GameState.GameMode.Stealth){
			if (winner == 1){
				SingleWin = true;
				Application.LoadLevel ("Won");
			}
			else if (loserIndex != 0){
				Application.LoadLevel ("Lost");
			}
		}

		if (mode == GameState.GameMode.Party || mode == GameState.GameMode.Race){
			if (winnerIndex + loserIndex == numOfPlayers ){
				status = GameState.GameStatus.End;
				gameObject.GetComponent<EndScreen>().enabled = true;
				if (winnerIndex != 0){
					Application.LoadLevel ("Won");
				}
				else {
					Application.LoadLevel ("Lost");
				}
			}
		}

		if (status == GameState.GameStatus.Tutorial){
			if (loserIndex == 1){
				status = GameState.GameStatus.End;
				Application.LoadLevel ("Lost");
			}
		}
	}

	private void UpdateNumPlayers(){
		numOfPlayers = GetComponent<UniMoveSplash>().numPlayers;
		winnerIndex = 0;
		loserIndex = 0;
		winners = new int[numOfPlayers];
		losers = new int[numOfPlayers];
	}

	private void setMoveColors(){
		foreach (UniMoveController move in moves){
			if (move.id > 0){
				switch (move.id){
				case 1:
					move.SetLED(Color.cyan);
					break;
				case 2:
					move.SetLED(Color.magenta);
					break;
				case 3:
					move.SetLED(Color.yellow);
					break;
				case 4:
					move.SetLED(Color.green);
					break;
				default:
					break;
				}
			}
		}
	}
}