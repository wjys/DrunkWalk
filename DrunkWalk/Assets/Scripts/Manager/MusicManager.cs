using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	public AudioClip[] songs;

	// Use this for initialization
	void Start () {
		gameObject.name = "AudioManager";
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.ins.status == GameState.GameStatus.Splash){

		}
		else if (GameManager.ins.status == GameState.GameStatus.End){

		}
		else if (GameManager.ins.status == GameState.GameStatus.Paused){

		}
		else if (GameManager.ins.status == GameState.GameStatus.Tutorial){

		}
		else if (GameManager.ins.status == GameState.GameStatus.Game){
			if (GameManager.ins.mode == GameState.GameMode.ScoreAttack || GameManager.ins.mode == GameState.GameMode.Stealth){
				// LOWER VOLUME IF PLAYING SINGLE PLAYER
			}
			else {
				// HIGHER VOLUME BECAUSE MULTIPLAYER
			}
		}
		if (!audio.isPlaying){
			audio.Play();
		}
	}
}
