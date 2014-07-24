using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

	public enum GameStatus { Splash, Paused, Game, Tutorial }
	public enum PlayerStatus { Fine, Fallen, Lost }
	public static int pScore;

	void Awake () {
		DontDestroyOnLoad(this);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
