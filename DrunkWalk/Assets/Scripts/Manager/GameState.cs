using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

	//GAME STATES

	public enum GameStatus { Splash, Paused, Game, Tutorial, End, Scores }

	//GAME MODES

	public enum GameMode {ScoreAttack, Stealth, Race, Party}

	//LEVELS

	public enum Level {Easy, Medium, Hard}

	//CONTROLLERS

	public enum GameController { mouse, move, xbox }; 

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
