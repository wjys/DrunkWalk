﻿using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	public AudioClip[] songs;

	// Use this for initialization
	void Start () {
		gameObject.name = "AudioManager";
		GetComponent<AudioSource>().clip = songs[Random.Range(0, songs.Length)];
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine (switchSongs ());
		if (GameManager.ins.status == GameState.GameStatus.Splash){
			// audio.volume = 
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
	}

	IEnumerator switchSongs(){
		if (!GetComponent<AudioSource>().isPlaying){
			GetComponent<AudioSource>().Play();
		}
		yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length + 0.5f);
		GetComponent<AudioSource>().clip = songs[Random.Range (0, songs.Length)];
	}
}
