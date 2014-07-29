﻿using UnityEngine;
using System.Collections;

public class MainMenu : Menu {

	static public GameManager ins;

	public Texture2D backgroundTexture;
	public Color backgroundColor;
	public GUISkin skin;

	private int idx = 0;
	private float timer = 0;

	private bool wasDown = false;
	private bool wasUp = false;
	private bool wasRight = false;
	private bool wasLeft = false;

	//Which menu?
	public static bool menuSet;
	public static int menuNum;

	//THE MENU ITEM VISUALIZATION
	private GameObject mMenuIns;
	public GameObject mMenu;
	public GameObject cube;

	//Camera
	public GameObject camObj;
	public float smooth;
	public static Vector3 newPos;
	public static Quaternion newRot;

	public bool lerping;

	//Character Reel
	public CharacterReel CR;

	//Main
	private Item[] items= new Item[] {
		new Item("START", delegate () { ChooseCharacter (); }),
		new Item("MULTIPLAYER", delegate () { Difficulty (-1); }),
		new Item("SET UP", delegate () { Settings (); }),
		new Item("CALIBRATE", delegate () { Calibrate (); }),
		new Item("EXIT", delegate () { Application.Quit(); })
	};

	//Settings
	private int sidx = 0;

	private Item[] sitems = new Item[] {
		new Item("SOUNDFX", delegate () { Debug.Log ("SOUNDFX"); }),
		new Item("MUSIC", delegate () { Debug.Log ("MUSIC"); }),
		new Item("CONTROLLER", delegate () { Debug.Log ("CONTROLLER"); })
	};

	//Characters
	private int cidx = 0;
	public int charID;

	private Item[] citems = new Item[] {
		new Item("THAT GUY", delegate () { Difficulty (1); }),
		new Item("ANADROID", delegate () { Difficulty (2); }),
		new Item("ANCHOVY", delegate () { Difficulty (3); }),
		new Item("WOLF", delegate () { Difficulty (4); }),
	};

	//Difficulty
	private int didx = 0;

	private Item[] ditems = new Item[] {
		new Item("JACK & COKE", delegate () { ChooseLevel (); }),
		new Item("BEER", delegate () { ChooseLevel (); }),
		new Item("WHISKEY", delegate () { ChooseLevel (); }),
		new Item("SANGRIA", delegate () { ChooseLevel (); }),
		new Item("RESET", delegate () { ChooseLevel (); }),
		new Item("LEAVE BAR", delegate () { ChooseLevel (); }),
	};

	//Level
	private int lidx = 0;

	private Item[] litems = new Item[] {
		new Item("LEVEL STUFF", delegate () { StartGame(); })
	};

	///////////////////////////////////////////////////////////////////


	void Start() {
		//If there's no instance of this, make one
		if (mMenuIns == null){
			mMenuIns = Instantiate(mMenu) as GameObject;
			mMenuIns.SetActive(true);
		}

		//Find Camera
		camObj = GameObject.FindGameObjectWithTag("MainCamera");

		//New Camera Pos and Rot
		newPos = new Vector3 (0,0,0);
		newRot = new Quaternion (0,0,0,0);

		//Current Menu is at the first one
		menuNum = 1;
		menuSet = false;

		//Character is at the Guy
		charID = 0;

		Menu1 ();

		//Get character reel
		CR = GameObject.Find ("Characters").GetComponent<CharacterReel>();
	}

	/////////////////////////////////////////////////////////////////////////////////////
	/////////////////
	//SHOW LEVEL GUIS
	/////////////////

	void OnGUI () {
		GUI.skin = skin;
		GUI.color = backgroundColor;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);
		GUI.color = Color.white;
		if (menuNum == 1) {
			GUIMenu(idx, 200, 80, items, timer);}
		else if (menuSet == true) {
			GUIMenu(sidx, 200, 80, sitems, timer);}
		//else if (menuNum == 2) {
		//	GUIMenu (cidx, 200, 80, citems, timer);}
		else if (menuNum == 3) {
			GUIMenu (didx, 200, 80, ditems, timer);}
		else if (menuNum == 4) {
			GUIMenu (lidx, 200, 80, litems, timer);}
	}


	/////////////////////////////////////////////////////////////////////////////////////
	////////////
	//START GAME
	////////////

	public static void StartGame(){
		GameManager.ins.game = true;
		GameManager.ins.status = GameState.GameStatus.Game;
		Application.LoadLevel ("WastedMouse");
	}

	public static void Menu1 (){
		menuNum = 1;
		menuSet = false;
		newPos = new Vector3 (0,0,0);
		newRot = new Quaternion(0,0,0,0);
	}

	//////////
	//SETTINGS
	//////////
	
	public static void Settings(){
		menuNum = 0;
		menuSet = true;

		newPos = new Vector3 (25,0,0);
		newRot = new Quaternion(0,0,0,0);
	}

	///////////////////////
	///CUSTOMIZE CHARACTER
	//////////////////////

	public static void ChooseCharacter(){


		menuSet = false;
		menuNum = 2;
	}

	/////////////
	///DIFFICULTY
	/////////////

	public static void Difficulty(int characterName){
		GameManager.chosenChar = characterName;

		menuSet = false;
		menuNum = 3;
	}

	///////////////
	///CHOOSE LEVEL
	///////////////
	
	public static void ChooseLevel(){

		menuSet = false;
		menuNum = 4;
	}


	/////////////
	///CALIBRATE
	/////////////

	public static void Calibrate(){

	}


	void FixedUpdate() {
		camObj.transform.position = Vector3.Lerp(camObj.transform.position, newPos, smooth * Time.deltaTime);
		camObj.transform.rotation = Quaternion.Lerp(camObj.transform.rotation, newRot, smooth * Time.deltaTime);
	}

	void Update () {

		timer += Time.deltaTime;

		//GET INPUT
		bool isUp = Input.GetAxis("Vertical") > 0.8f,
			 isDown = Input.GetAxis("Vertical") < -0.8f,
		     isRight = Input.GetAxis ("Horizontal") > 0.8f,
			 isLeft = Input.GetAxis ("Horizontal") < -0.8f;

		bool justUp = isUp && !wasUp,
			 justDown = isDown && !wasDown,
			 justRight = isRight && !wasRight,
			 justLeft = isLeft && !wasLeft;




		if (Input.GetButtonDown("Down") || justDown) {
			//WENT DOWN
			if (menuNum == 1) {
				//DOWN IN MAINMENU
				idx += 1;
				idx %= items.Length;
			} else if (menuSet) {
				//DOWN IN SETTINGS
				sidx += 1;
				sidx %= sitems.Length;
			} else if (menuNum == 3){
				//DOWN IN DIFF
			}

			timer = 0;
		}

		if (Input.GetButtonDown("Up") || justUp) {
			//WENT UP
			if (menuNum == 1) {
				//UP IN MAINMENU
				idx += items.Length - 1;
				idx %= items.Length;
			} else if (menuSet) {
				//UP IN SETTINGS
				sidx += sitems.Length - 1;
				sidx %= sitems.Length;
			} else if (menuNum == 3) {
				//UP IN DIFF
			}

			timer = 0;
		}

		if (Input.GetButtonDown ("Right") || justRight) {
			//WENT RIGHT
			if (menuNum == 2){
				//IN CHARACTERS, GO RIGHT TO CHOOSE CHARACTER
				cidx += 1;
				cidx %= citems.Length;
				//HIGHLIGHTED CHARACTER
				CR.charID = cidx;
			} else if (menuNum == 3){
				//RIGHT IN DIFF
			}

			if (menuSet){
				//IF IN SETTINGS, GO RIGHT TO RAISE VOLUME
				if (sidx == 0){
					AudioManager.ins.GetComponent<AudioSource>().volume += 0.1f;
				} else if (sidx == 1){
					AudioManager.ins.GetComponent<AudioSource>().volume += 0.1f;
				}
			}
		}

		if (Input.GetButtonDown ("Left") || justRight) {
			//WENT LEFT
			if (menuNum == 2){
				//IN CHARACTERS, GO LEFT TO CHOOSE CHARACTER
				cidx += citems.Length - 1;
				cidx %= citems.Length;
				//HIGHLIGHTED CHARACTER
				CR.charID = cidx;
			} else if (menuNum == 3){
				//LEFT IN DIFF
			}

			if (menuSet){
				//IF IN SETTINGS, GO LEFT TO LOWER VOLUME
				if (sidx == 0){
					AudioManager.ins.GetComponent<AudioSource>().volume -= 0.1f;
				} else if (sidx == 1){
					AudioManager.ins.GetComponent<AudioSource>().volume -= 0.1f;
				}
			}
		}

		if (Input.GetButtonDown("Confirm")) {
			//CONFIRMED
			if (menuNum == 1) {
				items[idx].command();
			} else if (menuSet){
				sitems[sidx].command();
			} else if (menuNum == 2){
				citems[cidx].command();
			} else if (menuNum == 3){
				ditems[didx].command();
			} else if (menuNum == 4){
				litems[lidx].command();
			}

			if (!lerping){
				lerping = true;
				StartCoroutine("LerpCam");
			}
		}

		if (Input.GetButtonDown("Cancel")) {
			//CANCELLED
			if (menuNum == 1){
				GameManager.ins.UnMenu();
			} else if (menuNum == 4){
				menuNum = 3;
			} else if (menuNum == 3){
				menuNum = 2;
			} else {
				menuNum = 1;
				menuSet = false;
				newPos = new Vector3 (0,0,0);
			}

			if (!lerping){
				lerping = true;
				StartCoroutine("LerpCam");
			}
		}

		wasUp = isUp;
		wasDown = isDown;
		wasRight = isRight;
		wasLeft = isLeft;


		//HIGHLIGHTED OBJS
		if (idx == 0){
			mMenuIns.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
		} else if (idx == 1){
			mMenuIns.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
		} else if (idx == 2){
			mMenuIns.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
		} else if (idx == 3){
			mMenuIns.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
		}
	}

	public IEnumerator LerpCam() {
		if (menuNum == 4){
			newPos = new Vector3(50, 25, 0);
		}

		if (menuNum == 3){
			newPos = new Vector3(25,25,0);
			newRot = new Quaternion (0, 0, 0, 0);
		}

		if (menuNum == 2){
			newPos = new Vector3(0,25,0);
			newRot = new Quaternion (0, 0, 0, 0);
		}

		if (menuNum == 1){
			newPos = new Vector3(0,0,0);
			newRot = new Quaternion (0, 0, 0, 0);
		}

		if (menuNum == 0){
			newPos = new Vector3(0,0,0);
			newRot = new Quaternion (0, 0, 0, 0);
		}

		if (menuSet){
			newPos = new Vector3(25,0,0);
		}

		yield return new WaitForSeconds(0.001f);
		lerping = false;
	}
}