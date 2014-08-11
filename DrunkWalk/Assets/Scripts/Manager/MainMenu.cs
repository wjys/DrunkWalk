using UnityEngine;
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

	//Difficulty
	public Difficulty Diff;

	//Levels
	public GameObject Lev;

	//Main (1)
	private int idx = 0;

	private Item[] items= new Item[] {
		new Item("START", delegate () { ChooseCharacter (); }),
		new Item("MULTIPLAYER", delegate () { MultiplayerChar (); }),
		new Item("SET UP", delegate () { Settings (); }),
		new Item("EXIT", delegate () { Application.Quit(); })
	};

	//Settings (set)
	private int sidx = 0;

	private Item[] sitems = new Item[] {
		new Item("SOUNDFX", delegate () { Debug.Log ("SOUNDFX"); }),
		new Item("MUSIC", delegate () { Debug.Log ("MUSIC"); }),
		new Item("CONTROLLER", delegate () { Debug.Log ("CONTROLLER"); })
	};

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
		new Item("JACK & COKE", delegate () { Debug.Log ("Jack & Coke"); }),
		new Item("BEER", delegate () { Debug.Log ("Beer"); }),
		new Item("WHISKEY", delegate () { Debug.Log ("Whiskey"); }),
		new Item("SANGRIA", delegate () { Debug.Log ("Sangria"); }),
		new Item("RESET", delegate () { Debug.Log ("Reset Drunk"); }),
		new Item("LEAVE BAR", delegate () { ChooseLevel (); }),
	};

	//SingleLevel (4)
	private int lidx = 0;

	private Item[] litems = new Item[] {
		new Item("TUTORIAL", delegate () { Modes (0); }),
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
	public bool readyStart;
	public float corDelay;

	///////////////////////////////////////////////////////////////////


	void Start() {
		//If there's no instance of this, make one
		if (mMenuIns == null){
			mMenuIns = Instantiate(mMenu) as GameObject;
			mMenuIns.SetActive(true);
		}

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

		readyStart = false;
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
			GUIMenu(idx, 200, 80, items, timer);}
		else if (menuSet == true) {
			GUIMenu(sidx, 200, 80, sitems, timer);}
		//else if (menuNum == 2) {
		//	GUIMenu (cidx, 200, 80, citems, timer);}
		//else if (menuNum == 3) {
		//	GUIMenu (didx, 200, 80, ditems, timer);}
		else if (menuNum == 4) {
			GUIMenu (lidx, 200, 80, litems, timer);}
		else if (menuNum == 5){
			GUIMenu (mcidx, 200, 80, mcitems, timer);}
		else if (menuNum == 6){
			GUIMenu (mlidx, 200, 80, mlitems, timer);}
		else if (menuNum == 7){
			GUIMenu (midx, 200, 80, mitems, timer);}
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

	public static void Menu1 (){
		menuNum = 1;
		menuSet = false;
	}

	//////////
	//SETTINGS    set
	//////////
	
	public static void Settings(){
		menuNum = 0;
		menuSet = true;

		newPos = new Vector3 (25,0,0);
		newRot = new Quaternion(0,0,0,0);
	}

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
		GameManager.ins.WhiskeyInt = 0;
		GameManager.ins.SangriaInt = 0;

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
		GameManager.ins.WhiskeyInt = 0;
		GameManager.ins.SangriaInt = 0;

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
					if (idx == 0 && GameManager.ins.GetComponent<UniMoveSplash>().numPlayers < 2){
						idx += 2;
					}
					else {
						idx += 1;
						idx %= items.Length;
					}

				} else if (menuSet) {
					//DOWN IN SETTINGS
					sidx += 1;
					sidx %= sitems.Length;
				} else if (menuNum == 3){
					//DOWN IN DIFF
					if (Diff.totalDrunk > 0){
						if (Diff.drinkID[didx] > 0){
							Diff.drinkID[didx] -= 1;
							Diff.totalDrunk -= 1;
						}
					}
					if (Diff.totalDrunk <= 0){
						Diff.totalDrunk = 0;
					}
				} else if (menuNum == 4) {
					//DOWN IN SETTINGS
					lidx += 1;
					lidx %= litems.Length;
				} else if (menuNum == 7) {
					//DOWN IN SETTINGS
					midx += 1;
					midx %= mitems.Length;
				//} else if (menuNum == 5) {
					//DOWN IN SETTINGS
					//mcidx += 1;
					//mcidx %= mcitems.Length;
				} else if (menuNum == 6) {
					//DOWN IN SETTINGS
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
					if (GameManager.ins.GetComponent<UniMoveSplash>().numPlayers < 2 && idx == 2){
						idx -= 2;
					}
					else {
						idx += items.Length - 1;
						idx %= items.Length;
					}
				} else if (menuSet) {
					//UP IN SETTINGS
					sidx += sitems.Length - 1;
					sidx %= sitems.Length;
				} else if (menuNum == 3) {
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

				if (menuSet){
					//IF IN SETTINGS, GO RIGHT TO RAISE VOLUME
					if (sidx == 0){
						AudioManager.ins.GetComponent<AudioSource>().volume += 0.1f;
					} else if (sidx == 1){
						AudioManager.ins.GetComponent<AudioSource>().volume += 0.1f;
					}
				}
			}
		}

		if (Input.GetButtonDown ("Left") || justLeft || tiltL) {
			//WENT LEFT
			if (!stopMove){
				if (tiltL){
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

				if (menuSet){
					//IF IN SETTINGS, GO LEFT TO LOWER VOLUME
					if (sidx == 0){
						AudioManager.ins.GetComponent<AudioSource>().volume -= 0.1f;
					} else if (sidx == 1){
						AudioManager.ins.GetComponent<AudioSource>().volume -= 0.1f;
					}
				}
			}
		}

		if (Input.GetButtonDown("Confirm") || movePressed || GameManager.ins.GetComponent<UniMoveSplash>().UniMoveAllPlayersIn()) {
			//CONFIRMED
			if (menuNum == 1) {
				items[idx].command();
			} else if (menuSet){
				sitems[sidx].command();
			} else if (menuNum == 2){
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
					UniMoveSplash splash = GameManager.ins.GetComponent<UniMoveSplash>();
					splash.setNumPlayers();
					splash.setGame ();
				}
				litems[lidx].command();
			} else if (menuNum == 5){
				stopMove = true;
				corDelay = 0.3f;
				StartCoroutine(resumeMove ());
				mcitems[mcidx].command();
			} else if (menuNum == 6){
				if (!stopMove){
					UniMoveSplash splash = GameManager.ins.GetComponent<UniMoveSplash>();
					splash.setNumPlayers();
					splash.setGame ();
					mlitems[mlidx].command();
				}
			} else if (menuNum == 7){
				UniMoveSplash splash = GameManager.ins.GetComponent<UniMoveSplash>();
				splash.setNumPlayers();
				splash.setGame ();
				mitems[midx].command();
			}

			if (!lerping){
				print ("LERP CONFIRM");
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
				print ("LERP CANCEL");
				lerping = true;
				StartCoroutine(LerpCam());
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
		//MODES
		if (menuNum == 7){
			newPos = new Vector3 (0, 25, 0);
			newRot = new Quaternion (0, 1, 0, 3.72529e-07f);
		}

		//MULTIPLAYER LEVELS
		if (menuNum == 6){
			newPos = new Vector3 (0, -25, 0);
			newRot = new Quaternion (0, 1, 0, -2.980232e-08f);
		}

		//MULTIPLAYER CHARACTERS
		if (menuNum == 5){
			print ("LERP CAM NUM 5");
			newPos = new Vector3(0, -25, 0);
			newRot = new Quaternion(0,0,0,1);
			Lev.transform.position = new Vector3(Lev.transform.position.x, -25, Lev.transform.position.z);
		}

		//LEVEL/MODE
		if (menuNum == 4){
			newPos = new Vector3(0, 25, 0);
			newRot = new Quaternion(0, 1, 0, 3.72529e-07f);
		}

		//DIFFICULTY
		if (menuNum == 3){
			newPos = new Vector3(0,25,0);
			newRot = new Quaternion (0, 0.7071071f, 0, 0.7071066f);
		}

		//CHARACTER
		if (menuNum == 2){
			Lev.transform.position = new Vector3(Lev.transform.position.x, 25, Lev.transform.position.z);
			newPos = new Vector3(0,25,0);
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
		if (menuSet){
			newPos = new Vector3 (25,0,0);
			newRot = new Quaternion(0,0,0,0);
		}

		yield return new WaitForSeconds(0.001f);
		lerping = false;
	}

	IEnumerator resumeMove(){
		yield return new WaitForSeconds (corDelay);
		stopMove = false;
	}
}