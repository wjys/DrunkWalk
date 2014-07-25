using UnityEngine;

public class PauseMenu : Menu {
	public GameManager GM;
	public Texture2D backgroundTexture;
	public Color backgroundColor;
	public GUISkin skin;

	private int idx = 0;
	private float timer = 0;

	private bool wasDown = false;
	private bool wasUp = false;

	private Item[] items= new Item[] {
		new Item("resume game", delegate () { noPause(); }),
		new Item("restart the level", delegate () { Application.LoadLevel(Application.loadedLevel);}),
		new Item("options", delegate () { Application.LoadLevel (Application.loadedLevel); }),
		new Item("quit to title", delegate () { Application.Quit(); })
	};

	void OnGUI () {
		GUI.skin = skin;
		GUI.color = backgroundColor;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);
		GUI.color = Color.white;
		GUIMenu(idx, 200, 80, items, timer);
	}

	public static void noPause(){
		GameManager GM = GameManager.ins;
		GM.UnPause();
	}

	void Update () {
		print (items[0]);
		timer += Time.deltaTime;

		bool isUp = Input.GetAxis("Vertical") > 0.8f,
			 isDown = Input.GetAxis("Vertical") < -0.8f;
		bool justUp = isUp && !wasUp,
			 justDown = isDown && !wasDown;

		if (Input.GetButtonDown("Down") || justDown) {
			Debug.Log("d");
			idx += 1;
			idx %= items.Length;
			timer = 0;
		}
		if (Input.GetButtonDown("Up") || justUp) {
			Debug.Log("u");
			idx += items.Length - 1;
			idx %= items.Length;
			timer = 0;
		}

		if (Input.GetButtonDown("Confirm")) {
			Debug.Log("conf");
			items[idx].command();
		}

		if (Input.GetButtonDown("Cancel")) {
			Debug.Log("canc");
			GameManager.ins.UnPause();
		}

		wasUp = isUp;
		wasDown = isDown;
	}
}