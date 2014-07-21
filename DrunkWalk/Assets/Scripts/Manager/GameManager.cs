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
	public GameObject audioManager;
	public GameObject mainMenu;
	private GameObject pauseMenuIns;
	private GameObject mainMenuIns;
	private float lastRealtimeSinceStartup;

	//Bools
	private bool paused = false;
	private bool menu = false;
	public bool game;


	void Awake () {
		//Setup instance
		DontDestroyOnLoad(this);
		ins = this;

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
		mainMenuIns = Instantiate(mainMenu) as GameObject;
		mainMenuIns.SetActive(false);

		pauseMenuIns = Instantiate(pauseMenu) as GameObject;
		pauseMenuIns.SetActive(false);
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
           	    	Pause();
          	 	} else {
           	    	UnPause();
           		}
           	}
		} else if (status == GameState.GameStatus.Splash) {
        	if (Input.anyKey){
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
}