using UnityEngine;
using System.Collections;

public class Ouch : MonoBehaviour {

	public string[] ouches;
	public int ouchIndex;
	public GUIText ouchGui;
	public Collision collision;
	public bool displayed; 

	// Use this for initialization
	void Start () {
		displayed = false; 
	}
	
	// Update is called once per frame
	void Update () {
	if (GameManager.ins.playerStatus == GameState.PlayerStatus.Fine){
		if (collision.recoiled == true){
			if (!displayed){
				gameObject.guiText.enabled = true;
				ouchIndex = Random.Range (0,5);
				ouchGui.text = ouches[ouchIndex];
				displayed = true; 
			}
		} else {
			gameObject.guiText.enabled = false;
			displayed = false; 
		}
		}
	}
}
