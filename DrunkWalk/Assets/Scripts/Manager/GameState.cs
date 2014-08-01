using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

	//GAME STATES

	public enum GameStatus { Splash, Paused, Game, Tutorial }

	//GAME MODES

	public enum GameMode {ScoreAttack, Stealth, Race, Party}

	/*
	 * ScoreAttack = Single Player score-based gameplay
	 * Stealth = Single Player obstacle course
	 * Race = Multiplayer race to the bed
	 * Party = Multiplayer musical chairs-like gameplay
	*/

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
