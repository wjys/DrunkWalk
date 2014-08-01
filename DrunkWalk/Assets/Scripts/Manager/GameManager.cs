using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//This
	static public GameManager ins;

	//Game State
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

	//Bools
	private bool paused = false;
	private bool menu = false;
	public bool game;

	//Character
	public static int chosenChar;

	//Difficulty
	public GameObject DiffObj;
	public int diffInt;
	public int JncInt, BeerInt, WhiskeyInt, SangriaInt;

	//Level
	public static int levelInt;

	//Winner
	public int winner;
	public int numOfPlayers;

	public int winnerIndex;
	public int loserIndex;
	public int[] winners;
	public int[] losers;

	public bool SingleWin;
	public bool SingleLose;
	public bool MultiGO;

	void Awake () {
		//Setup instance
		DontDestroyOnLoad(this);
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
			mainMenuIns.SetActive(false);
		}

		pauseMenuIns = Instantiate(pauseMenu) as GameObject;
		pauseMenuIns.name = "PauseMenu";
		pauseMenuIns.SetActive(false);
	}



	void Start () {
		numOfPlayers = GetComponent<UniMoveManager>().numPlayers;
		winnerIndex = 0;
		loserIndex = 0;
		winners = new int[numOfPlayers];
		losers = new int[numOfPlayers];

		DiffObj = GameObject.Find ("Difficulty");
	}


	void Update () {

		CheckWinLose ();

		//Pause Menu in Game, Main menu in Splash
		if (status == GameState.GameStatus.Game){
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







		} else if (status == GameState.GameStatus.Splash) {
        	/*if (Input.GetKeyDown("m")){
        		if (!menu) {
        			Menu();
        		} else {
        			UnMenu();
        		}
				Debug.Log ("MENU");
        	}*/
			Menu ();

			JncInt = DiffObj.GetComponent<Difficulty>().drinkID[0];
			BeerInt = DiffObj.GetComponent<Difficulty>().drinkID[1];
			WhiskeyInt = DiffObj.GetComponent<Difficulty>().drinkID[2];
			SangriaInt = DiffObj.GetComponent<Difficulty>().drinkID[3];

			diffInt = DiffObj.GetComponent<Difficulty>().totalDrunk;
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
			}
		}

		if (mode == GameState.GameMode.Party || mode == GameState.GameMode.Race){
			if (winnerIndex + loserIndex == numOfPlayers-1) {
				if (winnerIndex >= loserIndex){
					Application.LoadLevel ("Won");
				}
				else {
					Application.LoadLevel ("Lost");
				}
			}
		}
	}
}