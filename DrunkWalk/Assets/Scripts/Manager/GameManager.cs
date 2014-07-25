using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//This
	static public GameManager ins;

	//Game State
	public GameState.GameStatus status;
	public GameState.PlayerStatus playerStatus;

	//Menu Assets
	public GUISkin skin;

	public GameObject pauseMenu;
	public GameObject mainMenu;
	public GameObject audioManager;

	private GameObject pauseMenuIns;
	private GameObject mainMenuIns;
	private float lastRealtimeSinceStartup;

	//Bools
	public bool paused = false;
	private bool menu = false;
	public bool game;

	//Difficulty
	public static int chosenChar;


	void Awake () {
		//Setup instance
		DontDestroyOnLoad(this);
		ins = this;
		chosenChar = 0;

		//Set Game Status
		if (!game){
			status = GameState.GameStatus.Splash;
		} else {
			status = GameState.GameStatus.Game;
		}

		//Don't show mouse
		//Screen.showCursor = false;

		//Setup Audio Manager
		if (AudioManager.ins == null){
			(Instantiate(audioManager) as GameObject).SendMessage("Initialize");
		}

		//Menu Instances
		pauseMenuIns = Instantiate(pauseMenu) as GameObject;
		pauseMenuIns.SetActive(false);

		mainMenuIns = Instantiate(mainMenu) as GameObject;
		mainMenuIns.SetActive(false);
	}



	void Start () {
	}


	void Update () {
	
	if (playerStatus == GameState.PlayerStatus.Lost){
			//LOST
			Application.LoadLevel("Lost");
	}

		//Pause Menu in Game, Main menu in Splash
		if (status == GameState.GameStatus.Game){
		 	if (Input.GetKeyDown("p")){
          	 	if (!paused) {
					paused = true;
					pauseMenuIns.SetActive(true);

					InGame[] objs = FindObjectsOfType(typeof(InGame)) as InGame[];
					foreach (var obj in objs) {
						obj.paused = true;
					}

           	    	Pause();
					Debug.Log ("activate");
          	 	} else if (paused == true){
					paused = false;
					pauseMenuIns.SetActive(false);

					InGame[] objs = FindObjectsOfType(typeof(InGame)) as InGame[];
					foreach (var obj in objs) {
						obj.paused = false;
					}

           	    	UnPause();
					Debug.Log ("deactivate");
           		}
           	}
		} else if (status == GameState.GameStatus.Splash) {
			if (Input.GetKeyDown("m")){
        		if (!menu) {
        			Menu();
        		} else {
        			UnMenu();
        		}
				Debug.Log ("MENU");
        	}
        }
	}


	//Pause Menu Initialize
	public void Pause () {
		Debug.Log ("!!!!!");
		InGame[] objs = FindObjectsOfType(typeof(InGame)) as InGame[];
		foreach (var obj in objs) {
			obj.paused = true;
		}

		paused = true;
		pauseMenuIns.SetActive(paused);

	}

	public void UnPause () {
		Debug.Log ("?????");

		InGame[] objs = FindObjectsOfType(typeof(InGame)) as InGame[];
		foreach (var obj in objs) {
			obj.paused = false;
		}

		paused = false;
		pauseMenuIns.SetActive(false);

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
}