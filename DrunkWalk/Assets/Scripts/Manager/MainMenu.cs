using UnityEngine;

public class MainMenu : Menu {

	static public GameManager ins;

	public Texture2D backgroundTexture;
	public Color backgroundColor;
	public GUISkin skin;

	private int idx = 0;
	private float timer = 0;

	private bool wasDown = false;
	private bool wasUp = false;

	//THE MENU ITEM VISUALIZATION
	private GameObject mMenuIns;
	public GameObject mMenu;
	public GameObject cube;


	private Item[] items= new Item[] {
		new Item("START", delegate () { StartGame (); }),
		new Item("SET UP", delegate () { Application.LoadLevel(Application.loadedLevel);}),
		new Item("CALIBRATE", delegate () { Application.LoadLevel(0);}),
		new Item("EXIT", delegate () { Application.Quit(); })
	};

	void Start() {
		mMenuIns = Instantiate(mMenu) as GameObject;
		mMenuIns.SetActive(true);
	}

	void OnGUI () {
		GUI.skin = skin;
		GUI.color = backgroundColor;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);
		GUI.color = Color.white;
		GUIMenu(idx, 200, 80, items, timer);
	}

	public static void StartGame(){
		GameManager.ins.game = true;
		GameManager.ins.status = GameState.GameStatus.Game;
		Application.LoadLevel ("WastedMove");
	}

	void Update () {

		Debug.Log (idx);

		timer += Time.deltaTime;

		bool isUp = Input.GetAxis("Vertical") > 0.8f,
			 isDown = Input.GetAxis("Vertical") < -0.8f;
		bool justUp = isUp && !wasUp,
			 justDown = isDown && !wasDown;

		if (Input.GetButtonDown("Down") || justDown) {
			idx += 1;
			idx %= items.Length;
			timer = 0;
		}

		if (Input.GetButtonDown("Up") || justUp) {
			Debug.Log ("UP");
			idx += items.Length - 1;
			idx %= items.Length;
			timer = 0;
		}

		if (Input.GetButtonDown("Confirm")) {
			items[idx].command();
		}

		if (Input.GetButtonDown("Cancel")) {
			GameManager.ins.UnMenu();
		}

		wasUp = isUp;
		wasDown = isDown;


		if (idx == 1){
			mMenuIns.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
		}
	}
}