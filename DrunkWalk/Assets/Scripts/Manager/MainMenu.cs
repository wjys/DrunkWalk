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

	//Camera
	public GameObject camObj;
	public float smooth;
	public static Vector3 newPos;


	private Item[] items= new Item[] {
		new Item("START", delegate () { StartGame (); }),
		new Item("SET UP", delegate () { Settings (); }),
		new Item("CALIBRATE", delegate () { Application.LoadLevel(0);}),
		new Item("EXIT", delegate () { Application.Quit(); })
	};

	void Start() {
		mMenuIns = Instantiate(mMenu) as GameObject;
		mMenuIns.SetActive(true);
		camObj = GameObject.FindGameObjectWithTag("MainCamera");
		newPos = new Vector3 (0,0,0);
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

	public static void Settings(){
		newPos = new Vector3 (10,10,10);
	}

	void FixedUpdate() {
		camObj.transform.position = Vector3.Lerp(camObj.transform.position, newPos, smooth * Time.deltaTime);
	}

	void Update () {

		Debug.Log (idx);

		timer += Time.deltaTime;

		//GET INPUT
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




		//HIGHLIGHTED OBJS
		if (idx == 1){
			mMenuIns.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
		} else if (idx == 2){
			mMenuIns.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
		} else if (idx == 3){
			mMenuIns.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
		} else if (idx == 4){
			mMenuIns.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
		}
	}
}