﻿using UnityEngine;
using System.Collections;

public class MainMenu : Menu {

	static public GameManager ins;

	public Texture2D backgroundTexture;
	public Color backgroundColor;
	public GUISkin skin;
	
	private float timer = 0;

	private bool wasDown = false;
	private bool wasUp = false;
	private bool wasRight = false;
	private bool wasLeft = false;

	public static bool multi;

	//Which menu?
	public static bool menuSet;
	public static int menuNum;
	public int menuNumPublic;

	//THE MENU ITEM VISUALIZATION
	public GameObject mMenu;

	//Camera
	public GameObject camObj;
	public float smooth;
	public static Vector3 newPos;
	public static Quaternion newRot;

	public bool lerping;

	//Character Reel
	public CharacterReel CR;

	//Difficulty
	public Difficulty Diff;

	//Levels
	public GameObject Lev;
	public GameObject Mod;
	public GameObject Choice;
	public GameObject CellPhone;
	public bool lerpPhone;
	public Vector3 originalPhone;
	public Vector3 newPhonePos;
	public Vector3 phoneDownPos;

	public GameObject[] Markers;
	public Vector3 unselectedMarkerScale;
	public Vector3 selectedMarkerScale;
	public float[] originalYs;
	public float[] shiftedYs;

	public Sprite[] levSprite;

	//Multiplayer Modes
	private SpriteRenderer phoneScreen, reply;
	private GameObject bubbles;
	public Sprite[] screens, bubbleSpr, replySpr;

	//Main (1)
	private int idx = 0;

	private Item[] items= new Item[] {
		new Item("START", delegate () { ChooseCharacter (); }),
		new Item("MULTIPLAYER", delegate () { MultiplayerChar (); }),
		//new Item("HOW TO PLAY", delegate () { HTP(); }),
		new Item("EXIT", delegate () { Application.Quit(); })
	};

	//Settings (set)
	/*
	private int sidx = 0;

	private Item[] sitems = new Item[] {
		new Item("SOUNDFX", delegate () { Debug.Log ("SOUNDFX"); }),
		new Item("MUSIC", delegate () { Debug.Log ("MUSIC"); }),
		new Item("CONTROLLER", delegate () { Debug.Log ("CONTROLLER"); })
	};*/

	//Characters (2)
	private int cidx = 0;

	private Item[] citems = new Item[] {
		new Item("THAT GUY", delegate () { Difficulty (1); }),
		new Item("ANADROID", delegate () { Difficulty (2); }),
		new Item("ANCHOVY", delegate () { Difficulty (3); }),
		new Item("WOLF", delegate () { Difficulty (4); }),
	};

	//Difficulty (3)
	private int didx = 0;

	private Item[] ditems = new Item[] {
		new Item("JACK & COKE", delegate () { ChooseLevel (); }),
		new Item("BEER", delegate () { Debug.Log ("Beer"); }),
		new Item("VODKA", delegate () { Debug.Log ("Vodka"); }),
		new Item("GIN", delegate () { Debug.Log ("Gin"); }),
		new Item("RESET", delegate () { Debug.Log ("Reset Drunk"); }),
		new Item("LEAVE BAR", delegate () { ChooseLevel (); }),
	};

	//SingleLevel (4)
	private int lidx = 0;

	private Item[] litems = new Item[] {
		new Item("TUTORIAL", delegate () { StartTutorial(); }),
		new Item("EASY", delegate () { Modes (1); }),
		new Item("MEDIUM", delegate () { Modes (2); }),
		new Item("HARD", delegate () { Modes (3); })
	};

	//SingleMode (7)
	private int midx = 0;

	private Item[] mitems = new Item[] {
		new Item("SCOREATTACK", delegate () { StartGame (0); }),
		new Item("STEALTH", delegate () { StartGame (1); })
	};

	///////////////////
	//MULTIPLAYER MENUS
	///////////////////
	
	//MultiCharacter (5)
	public int mcidx = 0;
	
	public Item[] mcitems = new Item[] {
		new Item("ZACH", delegate () {MultiplayerLevel(); }),
		new Item("ANA", delegate () {Debug.Log ("ANNA"); }),
		new Item("ANH CHI", delegate () {Debug.Log ("ANHCHI"); }),
		new Item("WINNIE", delegate () {Debug.Log ("WINNIE"); }),
		new Item("BACK TO MULTI CHAR", delegate () { MultiplayerChar (); }),
	};
	
	//MultiMode (6)
	private int mlidx = 0;
	
	private Item[] mlitems = new Item[] {
		new Item("RACE", delegate () { StartGame (2); }),
		new Item("PARTY", delegate () { StartGame (3); })
	};

	// for navigating through using the move controllers
	public bool cancelSelection;
	public bool movePressed;
	public bool tiltL;
	public bool tiltR;
	public bool tiltF;
	public bool tiltB;
	public bool flipped;

	public bool stopMove;
	public bool stopConfirm;
	public float corDelay;

	///////////////////////////////////////////////////////////////////


	void Start() {
		//If there's no instance of this, make one

		//Find Camera
		camObj = GameObject.Find("SplashCam");

		//New Camera Pos and Rot
		newPos = new Vector3 (0,0,0);
		newRot = new Quaternion (0,0,0,0);

		//Current Menu is at the first one
		menuNum = 1;
		menuSet = false;
		Menu1 ();

		//Get World Components
		CR = GameObject.Find ("Characters").GetComponent<CharacterReel>();
		Diff = GameObject.Find ("Difficulty").GetComponent<Difficulty>();
		Lev = GameObject.Find ("Level");

		//Set multiplayer to false
		multi = false;

		Mod = GameObject.Find ("ModeUI");
		Choice = GameObject.Find("ChoiceBox");
		Mod.SetActive (false);

		CellPhone = GameObject.Find ("Phone");
		originalPhone = CellPhone.transform.position;
		phoneDownPos = new Vector3 (CellPhone.transform.position.x, -3.61f, CellPhone.transform.position.z);

		//Get Level Markers
		Markers = new GameObject[4];

		Markers[0] = GameObject.Find ("MarkerT");
		Markers[1] = GameObject.Find ("MarkerE");
		Markers[2] = GameObject.Find ("MarkerM");
		Markers[3] = GameObject.Find ("MarkerH");

		selectedMarkerScale = new Vector3(0.08f, 0.08f, 0.08f);
		unselectedMarkerScale = new Vector3(0.03f, 0.03f, 0.03f);

		originalYs = new float[Markers.Length];
		shiftedYs = new float[Markers.Length];

		for (int i = 0; i < Markers.Length; i++){
			originalYs[i] = Markers[i].transform.position.y;
			shiftedYs[i] = originalYs[i]+0.2f;
		}

		//Get Phone screen and set it to map
		phoneScreen = GameObject.Find ("Screen").GetComponent<SpriteRenderer> ();
		phoneScreen.sprite = screens [0];
		bubbles = GameObject.Find ("Bubbles");
		reply = GameObject.Find ("Reply").GetComponent<SpriteRenderer>();
	}

	/////////////////////////////////////////////////////////////////////////////////////
	/////////////////
	//SHOW MENU GUIS
	/////////////////

	void OnGUI () {
		GUI.skin = skin;
		GUI.color = backgroundColor;
		//GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);
		GUI.color = Color.white;
		if (menuNum == 1) {
			//GUIMenu(idx, 200, 80, items, timer);
			for (int i = 0; i < Markers.Length; i++){
				Markers[i].GetComponent<SpriteRenderer>().enabled = true;
			}
			bubbles.GetComponent<SpriteRenderer>().enabled = false;
			reply.enabled = false;
		}
		//else if (menuSet == true) {
		//	GUIMenu(sidx, 200, 80, sitems, timer);}
		//else if (menuNum == 2) {
		//	GUIMenu (cidx, 200, 80, citems, timer);}
		//else if (menuNum == 3) {
		//	GUIMenu (didx, 200, 80, ditems, timer);}
		//else if (menuNum == 4) {
		//	GUIMenu (lidx, 200, 80, litems, timer);}
		//else if (menuNum == 5){
		//	GUIMenu (mcidx, 200, 80, mcitems, timer);}
		else if (menuNum == 6){
		//	GUIMenu (mlidx, 200, 80, mlitems, timer);
			for (int i = 0; i < Markers.Length; i++){
				Markers[i].GetComponent<SpriteRenderer>().enabled = false;
			}
			bubbles.GetComponent<SpriteRenderer>().enabled = true;
			reply.enabled = true;
		}
		//else if (menuNum == 7){

		//	GUIMenu (midx, 200, 80, mitems, timer);}
	}

	/////////////////////////////////////////////////////////////////////////////////////
	////////////
	//START GAME    1
	////////////

	public static void StartGame(int mode){
		GameManager.ins.game = true;

		//TUTORIAL PORTAL
		if (mode == 4){
			Application.LoadLevel ("WastedTut");
			GameManager.ins.status = GameState.GameStatus.Tutorial;
		}
		//SCORE PORTAL
		else if (mode == 0){
			Application.LoadLevel ("WastedEasy");
			GameManager.ins.status = GameState.GameStatus.Game;
			GameManager.ins.mode = GameState.GameMode.ScoreAttack;
		}
		//STEALTH PORTAL
		else if (mode == 1){
			//Application.LoadLevel (GameManager.chosenLevel);
			Application.LoadLevel ("WastedEasy");
			GameManager.ins.status = GameState.GameStatus.Game;
			GameManager.ins.mode = GameState.GameMode.ScoreAttack;
		}
		//RACE PORTAL
		else if (mode == 2){
			Application.LoadLevel ("WastedMulti");
			//Application.LoadLevel("WastedParty");
			GameManager.ins.status = GameState.GameStatus.Game;
			GameManager.ins.mode = GameState.GameMode.Race;
			//GameManager.ins.mode = GameState.GameMode.Party;
		}
		//PARTY PORTAL
		else if (mode == 3){
			Application.LoadLevel("WastedParty");
			GameManager.ins.status = GameState.GameStatus.Game;
			GameManager.ins.mode = GameState.GameMode.Party;
		}
	}

	public static void StartTutorial(){
		Application.LoadLevel ("WastedTut");
		GameManager.ins.status = GameState.GameStatus.Tutorial;
	}

	public static void Menu1 (){
		menuNum = 1;
		menuSet = false;
	}

	//////////
	//SETTINGS    set
	//////////
	/*
	public static void Settings(){
		menuNum = 0;
		menuSet = true;

		newPos = new Vector3 (25,0,0);
		newRot = new Quaternion(0,0,0,0);
	}*/

	///////////////////////
	///CUSTOMIZE CHARACTER    2
	//////////////////////

	public static void ChooseCharacter(){


		menuSet = false;
		menuNum = 2;

		multi = false;
	}

	/////////////
	///DIFFICULTY    3
	/////////////

	public static void Difficulty(int characterName){
		GameManager.ins.chosenChar = characterName;

		menuSet = false;
		menuNum = 3;

		multi = false;
	}

	///////////////
	///CHOOSE LEVEL   4
	///////////////
	
	public static void ChooseLevel(){
		GameManager.ins.JncInt = 0;
		GameManager.ins.BeerInt = 0;
		GameManager.ins.VodkaInt = 0;
		GameManager.ins.GinInt = 0;

		menuSet = false;
		menuNum = 4;
	}

	/////////////
	///MULTI CHAR   5
	/////////////

	public static void MultiplayerChar(){
		//SET EACH PLAYER'S MULTIPLAYER
		GameManager.ins.JncInt = 0;
		GameManager.ins.BeerInt = 0;
		GameManager.ins.VodkaInt = 0;
		GameManager.ins.GinInt = 0;

		menuSet = false;
		menuNum = 5;

		multi = true;
	}

	//////////////
	///MULTI LEVEL   6
	//////////////
	
	public static void MultiplayerLevel(){
		//SET MULTIPLAYER
		menuSet = false;
		menuNum = 6;

		multi = true;
	}

	////////
	///MODES    7
	////////

	public static void Modes(int level) {
		multi = false;
		//GameManager.ins.levelNum = level;

		if (level == 0){
			StartGame(4);
		}

		menuSet = false;
		menuNum = 7;
	}


	void FixedUpdate() {
		//Cam Lerp
		camObj.transform.position = Vector3.Lerp(camObj.transform.position, newPos, smooth * Time.deltaTime);
		camObj.transform.rotation = Quaternion.Lerp(camObj.transform.rotation, newRot, smooth * Time.deltaTime);


		//LerpMarker

		for (int i = 0; i < Markers.Length; i++) {
			if (i == lidx){
				Markers[i].transform.localScale = Vector3.Lerp (Markers[i].transform.localScale, selectedMarkerScale, (smooth*2) * Time.deltaTime);
				Markers[i].transform.position = Vector3.Lerp(Markers[i].transform.position, new Vector3(Markers[i].transform.position.x, shiftedYs[i], Markers[i].transform.position.z), (smooth*2) * Time.deltaTime);
			} else {
				Markers[i].transform.localScale = Vector3.Lerp (Markers[i].transform.localScale, unselectedMarkerScale, (smooth*2) * Time.deltaTime);
				Markers[i].transform.position = Vector3.Lerp(Markers[i].transform.position, new Vector3(Markers[i].transform.position.x, originalYs[i], Markers[i].transform.position.z), (smooth*2) * Time.deltaTime);
			}
		}

		if (lerpPhone){
			CellPhone.transform.localPosition = Vector3.Lerp (CellPhone.transform.localPosition, newPhonePos, smooth * Time.deltaTime);
		}
	}


	void Update () {
		menuNumPublic = menuNum;
		print ("menuNum " + menuNum);
		timer += Time.deltaTime;

		//GET INPUT
		bool isUp = Input.GetAxis("Vertical") > 0.8f,
			 isDown = Input.GetAxis("Vertical") < -0.8f,
		     isRight = Input.GetAxis ("Horizontal") > 0.8f,
			 isLeft = Input.GetAxis ("Horizontal") < -0.8f;

		bool justUp = (isUp && !wasUp),
			 justDown = (isDown && !wasDown),
			 justRight = (isRight && !wasRight),
			 justLeft = (isLeft && !wasLeft);



		if (Input.GetButtonDown("Down") || justDown || tiltF) {
			//WENT DOWN
			if (!stopMove){
				if (menuNum == 1) {
					//DOWN IN MAINMENU
					if (idx == 0 && 
					    ((GameManager.ins.splashScript != null && GameManager.ins.splashScript.numPlayers < 2) ||
					  	 (GameManager.ins.controller == GameState.GameController.mouse))){
						idx += 2;
					}
					else {
						idx += 1;
						idx %= items.Length;
					}

				} /*else if (menuSet) {
					//DOWN IN SETTINGS
					sidx += 1;
					sidx %= sitems.Length;
				} */else if (menuNum == 3){
					//DOWN IN DIFF
					/*if (Diff.totalDrunk > 0){
						if (Diff.drinkID[didx] > 0){
							Diff.drinkID[didx] -= 1;
							Diff.totalDrunk -= 1;
						}
					}*/
					if (Diff.totalDrunk <= 0){
						Diff.totalDrunk = 0;
					}
				} else if (menuNum == 4) {
					//DOWN IN LEVELS
					lidx += 1;
					lidx %= litems.Length;
				} else if (menuNum == 7) {
					//DOWN IN MODES
					midx += 1;
					midx %= mitems.Length;
				//} else if (menuNum == 5) {
					//DOWN IN MULTICHAR
					//mcidx += 1;
					//mcidx %= mcitems.Length;
				} else if (menuNum == 6) {
					//DOWN IN MULTIMODES
					mlidx += 1;
					mlidx %= mlitems.Length;
				}
				if (tiltF){
					stopMove = true;
					if (menuNum != 3){
						corDelay = 1.0f;
					}
					else {
						corDelay = 0.2f;
					}
					StartCoroutine (resumeMove());
				}
				timer = 0;
			}
		}

		if (Input.GetButtonDown("Up") || justUp || tiltB) {
			//WENT UP
			if (!stopMove){
				if (menuNum == 1) {
					//UP IN MAINMENU
					if (idx == 2 && 
					    ((GameManager.ins.splashScript != null && GameManager.ins.splashScript.numPlayers < 2) ||
						 (GameManager.ins.controller == GameState.GameController.mouse))){
						idx -= 2;
					}
					else {
						idx += items.Length - 1;
						idx %= items.Length;
					}
				} /*else if (menuSet) {
					//UP IN SETTINGS
					sidx += sitems.Length - 1;
					sidx %= sitems.Length;
				} */else if (menuNum == 3) {
					//UP IN DIFF
					if (Diff.totalDrunk < 5){
						if (Diff.drinkID[didx] < 5){
							Diff.drinkID[didx] += 1;
							Diff.totalDrunk += 1;
						}
					}
					if (Diff.totalDrunk >= 5){
						Diff.totalDrunk = 5;
					}
				} else if (menuNum == 4){
					lidx += litems.Length-1;
					lidx %= litems.Length;
				} else if (menuNum == 7){
					midx += mitems.Length-1;
					midx %= mitems.Length;
				//} else if (menuNum == 5){
				//	mcidx += mcitems.Length-1;
				//	mcidx %= mcitems.Length;
				} else if (menuNum == 6){
					mlidx += mlitems.Length-1;
					mlidx %= mlitems.Length;
				}
				if (tiltB){
					stopMove = true;
					if (menuNum != 3){
						corDelay = 1.0f;
					}
					else {
						corDelay = 0.2f;
					}
					StartCoroutine (resumeMove());
				}
				timer = 0;
			}
		}

		if (Input.GetButtonDown ("Right") || justRight || tiltR) {
			//WENT RIGHT
			if (!stopMove){
				if (tiltR){
					stopMove = true;
					corDelay = 0.5f;
					StartCoroutine (resumeMove());
				}
				if (menuNum == 2){
					//IN CHARACTERS, GO RIGHT TO CHOOSE CHARACTER
					cidx += 1;
					cidx %= citems.Length;
					//HIGHLIGHTED CHARACTER
					CR.charID = cidx;
				} else if (menuNum == 3){
					//RIGHT IN DIFF
					didx += 1;
					didx %= ditems.Length;
					Diff.currDrink = didx;
				}

				/*if (menuSet){
					//IF IN SETTINGS, GO RIGHT TO RAISE VOLUME
					if (sidx == 0){
						AudioManager.ins.GetComponent<AudioSource>().volume += 0.1f;
					} else if (sidx == 1){
						AudioManager.ins.GetComponent<AudioSource>().volume += 0.1f;
					}
				}*/
			}
		}

		if (Input.GetButtonDown ("Left") || justLeft || tiltL) {
			//WENT LEFT
			if (!stopMove){
				if (tiltL){
					print ("TILTING LEFT");
					stopMove = true;
					corDelay = 1.0f;
					StartCoroutine (resumeMove());
				}
				if (menuNum == 2){
					//IN CHARACTERS, GO LEFT TO CHOOSE CHARACTER
					cidx += citems.Length - 1;
					cidx %= citems.Length;
					//HIGHLIGHTED CHARACTER
					CR.charID = cidx;
				} else if (menuNum == 3){
					//LEFT IN DIFF
					didx += ditems.Length - 1;
					didx %= ditems.Length;
					Diff.currDrink = didx;
				}

				/*if (menuSet){
					//IF IN SETTINGS, GO LEFT TO LOWER VOLUME
					if (sidx == 0){
						AudioManager.ins.GetComponent<AudioSource>().volume -= 0.1f;
					} else if (sidx == 1){
						AudioManager.ins.GetComponent<AudioSource>().volume -= 0.1f;
					}
				}*/
			}
		}

		if (Input.GetButtonDown("Confirm") || movePressed){// || GameManager.ins.GetComponent<UniMoveSplash>().UniMoveAllPlayersIn()) {
			//CONFIRMED
			if (menuNum == 1) {
				items[idx].command();
			} /*else if (menuSet){
				sitems[sidx].command();
			}*/ else if (menuNum == 2){
				citems[cidx].command();
			} else if (menuNum == 3){
				ditems[didx].command();
				if (didx == 4){
					Diff.totalDrunk = 0;
					for (int i = 0; i < Diff.drinkID.Length; i++){
						Diff.drinkID[i] = 0;
					}
				}
			} else if (menuNum == 4){
				if (lidx == 0){
					if (GameManager.ins.controller == GameState.GameController.move){
						UniMoveSplash splash = GameManager.ins.GetComponent<UniMoveSplash>();
						splash.setNumPlayers();
						splash.setGame ();
					}
				}
				litems[lidx].command();
			} else if (menuNum == 5){
				stopConfirm = true;
				corDelay = 0.3f;
				StartCoroutine(resumeConfirm ());
				mcitems[mcidx].command();
			} else if (menuNum == 6){
				if (!stopConfirm){
					if (GameManager.ins.controller == GameState.GameController.move){
						UniMoveSplash splash = GameManager.ins.GetComponent<UniMoveSplash>();
						splash.setNumPlayers();
						splash.setGame ();
					}
					mlitems[mlidx].command();
				}
			} else if (menuNum == 7){
				if (GameManager.ins.controller == GameState.GameController.move){
					UniMoveSplash splash = GameManager.ins.GetComponent<UniMoveSplash>();
					splash.setNumPlayers();
					splash.setGame ();
				}
				mitems[midx].command();
			}

			if (!lerping){
				lerping = true;
				StartCoroutine(LerpCam ());
			}
		}

		if (Input.GetButtonDown("Cancel") | cancelSelection) {
			//CANCELLED
			if (menuNum == 1){
				//GameManager.ins.UnMenu();
			} else if (menuNum == 4){
				menuNum = 3;
			} else if (menuNum == 3){
				menuNum = 2;
			} else if (menuNum == 6){
				menuNum = 5;
			} else if (menuNum == 7){
				menuNum = 4;
			} else {
				menuNum = 1;
				menuSet = false;
				newPos = new Vector3 (0,0,0);
				newRot = new Quaternion (0,0,0,0);
			}

			if (!lerping){
				lerping = true;
				StartCoroutine(LerpCam());
			}
		}

		wasUp = isUp;
		wasDown = isDown;
		wasRight = isRight;
		wasLeft = isLeft;

		//////////////////////////
		/////HIGHLIGHTED OBJS/////
		//////////////////////////

		//Main Menu
		if (idx == 0){
			GameObject.Find ("ButtonPlay").GetComponentInChildren<SpriteRenderer>().enabled = true;
			GameObject.Find ("ButtonMulti").GetComponentInChildren<SpriteRenderer>().enabled = false;
			GameObject.Find ("ButtonHTP").GetComponentInChildren<SpriteRenderer>().enabled = false;
			GameObject.Find ("ButtonSounds").GetComponentInChildren<SpriteRenderer>().enabled = false;
		} else if (idx == 1){
			GameObject.Find ("ButtonPlay").GetComponentInChildren<SpriteRenderer>().enabled = false;
			GameObject.Find ("ButtonMulti").GetComponentInChildren<SpriteRenderer>().enabled = true;
			GameObject.Find ("ButtonHTP").GetComponentInChildren<SpriteRenderer>().enabled = false;
			GameObject.Find ("ButtonSounds").GetComponentInChildren<SpriteRenderer>().enabled = false;
		} else if (idx == 2){
			GameObject.Find ("ButtonPlay").GetComponentInChildren<SpriteRenderer>().enabled = false;
			GameObject.Find ("ButtonMulti").GetComponentInChildren<SpriteRenderer>().enabled = false;
			GameObject.Find ("ButtonHTP").GetComponentInChildren<SpriteRenderer>().enabled = true;
			GameObject.Find ("ButtonSounds").GetComponentInChildren<SpriteRenderer>().enabled = false;
		} else if (idx == 3){
			GameObject.Find ("ButtonPlay").GetComponentInChildren<SpriteRenderer>().enabled = false;
			GameObject.Find ("ButtonMulti").GetComponentInChildren<SpriteRenderer>().enabled = false;
			GameObject.Find ("ButtonHTP").GetComponentInChildren<SpriteRenderer>().enabled = false;
			GameObject.Find ("ButtonSounds").GetComponentInChildren<SpriteRenderer>().enabled = true;
		}

		/*********
		 * SINGLE PLAYER
		 * ********/

		//Diff Menu
		if (didx == 0){
			//Drink1
		} else if (didx == 1){
			//Drink2
		} else if (didx == 2){
			//Drink3
		} else if (didx == 3){
			//Drink4
		} else if (didx == 4){
			//BATHROOM
		} else if (didx == 5){
			//GO HOME
		}

		//Level Menu
		if (lidx == 0){
			//Tutorial
		} else if (lidx == 1){
			//Easy house
		} else if (lidx == 2){
			//Medium house
		} else if (lidx == 3){
			//Hard house
		}

		//Mode Menu
		if (midx == 0){
			//Score attack
		} else if (midx == 1){
			//Stealth
		}
		if (menuNum == 4){
			if (!GameObject.Find ("LevelSprites").GetComponent<SpriteRenderer>().enabled){
				GameObject.Find ("LevelSprites").GetComponent<SpriteRenderer>().enabled = true;
			}
			GameObject.Find ("LevelSprites").GetComponent<SpriteRenderer>().sprite = levSprite[lidx];
		}

		else if (menuNum != 4){
			if (GameObject.Find ("LevelSprites").GetComponent<SpriteRenderer>().enabled){
				GameObject.Find ("LevelSprites").GetComponent<SpriteRenderer>().enabled = false;
            }
		}

		if(menuNum == 6){
			if (mlidx == 0) {
				phoneScreen.sprite = screens [1];
				bubbles.GetComponent<SpriteRenderer>().sprite = bubbleSpr [0];
				reply.sprite = replySpr [0];	
			} else if (mlidx == 1) {
				phoneScreen.sprite = screens [2];
				bubbles.GetComponent<SpriteRenderer>().sprite = bubbleSpr [1];
				reply.sprite = replySpr [1];
			}
		}

		if (menuNum == 7){
			if (!Mod.activeSelf){
				Mod.SetActive (true);
			}
			if (CellPhone.activeSelf){
				CellPhone.SetActive(false);
			}
			if (midx == 0){
				Choice.transform.localPosition = new Vector3(Choice.transform.localPosition.x, -0.1795873f, Choice.transform.localPosition.z);
			}
			else if (midx == 1){
				Choice.transform.localPosition = new Vector3(Choice.transform.localPosition.x, 0.22f, Choice.transform.localPosition.z);
			}
		}

		if (menuNum != 7){
			if (!CellPhone.activeSelf){
				CellPhone.SetActive(true);
			}
			if (Mod.activeSelf){
				Mod.SetActive (false);
			}
		}
	}

	public IEnumerator LerpCam() {
		//MODES
		if (menuNum == 7){
			newPos = new Vector3 (0, 40, 0);
			newRot = new Quaternion (0, 1, 0, 3.72529e-07f);
		}

		//MULTIPLAYER LEVELS
		if (menuNum == 6){
			newPos = new Vector3 (0, -25, 0);
			newRot = new Quaternion (0, 1, 0, -2.980232e-08f);
		}

		//MULTIPLAYER CHARACTERS
		if (menuNum == 5){
			newPos = new Vector3(0, -25, 0);
			newRot = new Quaternion(0,0,0,1);
			Lev.transform.position = new Vector3(Lev.transform.position.x, -25, Lev.transform.position.z);
		}

		//LEVEL/MODE
		if (menuNum == 4){
			newPos = new Vector3(0, 40, 0);
			newRot = new Quaternion(0, 1, 0, 3.72529e-07f);
			phoneScreen.sprite = screens [0];
		}

		//DIFFICULTY
		if (menuNum == 3){
			newPos = new Vector3(0,30,0);
			newRot = new Quaternion (0, 0.7071071f, 0, 0.7071066f);
		}

		//CHARACTER
		if (menuNum == 2){
			Lev.transform.position = new Vector3(Lev.transform.position.x, 40, Lev.transform.position.z);
			phoneScreen.sprite = screens [0];
			newPos = new Vector3(0,30,0);
			newRot = new Quaternion (0, 0, 0, 1);
		}

		//MAIN
		if (menuNum == 1){
			newPos = new Vector3(0,0,0);
			newRot = new Quaternion (0, 0, 0, 1);
		}

		if (menuNum == 0){
			menuNum = 1;
		}

		//SETTING
		/*
		if (menuSet){
			newPos = new Vector3 (25,0,0);
			newRot = new Quaternion(0,0,0,0);
		}*/

		yield return new WaitForSeconds(0.001f);
		lerping = false;
	}

	IEnumerator resumeMove(){
		yield return new WaitForSeconds (corDelay);
		stopMove = false;
	}

	IEnumerator resumeConfirm(){
		yield return new WaitForSeconds (corDelay);
		stopConfirm = false;
	}

	IEnumerator stopPhone(){
		lerpPhone = true;
		yield return new WaitForSeconds(0.5f);
		lerpPhone = false;
	}
}