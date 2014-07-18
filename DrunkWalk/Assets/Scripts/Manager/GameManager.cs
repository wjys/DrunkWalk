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
		if (!game){
			status = GameState.GameStatus.Splash;
		} else {
			status = GameState.GameStatus.Game;
		}

		DontDestroyOnLoad(this);
		ins = this;

		//Screen.showCursor = false;

		if (AudioManager.ins == null){
			(Instantiate(audioManager) as GameObject).SendMessage("Initialize");
		}

		mainMenuIns = Instantiate(mainMenu) as GameObject;
		mainMenuIns.SetActive(false);

		pauseMenuIns = Instantiate(pauseMenu) as GameObject;
		pauseMenuIns.SetActive(false);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
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