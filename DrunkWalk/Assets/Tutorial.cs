using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	public string[] inst;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.ins.status == GameState.GameStatus.Game){
			gameObject.guiText.enabled = true;
			gameObject.guiText.text = inst[0];
		}
	}
}
