using UnityEngine;
using System.Collections;

public class MultiMenu : Menu {

	static public GameManager ins;

	public Texture2D backgroundTexture;
	public Color backgroundColor;
	public GUISkin skin;

	///////////////////
	//MULTIPLAYER MENUS
	///////////////////
	
	//MultiCharacter (5)
	private int mcidx = 0;
	
	private Item[] mcitems = new Item[] {
		new Item("", delegate () {Debug.Log ("MULTI CHARACTERS"); })
	};
	
	//MultiMode (6)
	private int mlidx = 0;
	
	private Item[] mlitems = new Item[] {
		new Item("", delegate () {Debug.Log ("MULTI LEVELS"); })
	};

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
