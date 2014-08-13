using UnityEngine;
using System.Collections;

public class SingleWinSound : MonoBehaviour {

	public int character;
	public AudioClip[] char_wins;
	public AudioClip[] zach_wins;
	public AudioClip[] ana_wins;
	public AudioClip[] acb_wins;
	public AudioClip[] winnie_wins;

	public bool soundPlayed;

	// Use this for initialization
	void Start () {
		soundPlayed = false;
		GetWinnerClips ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.ins.SingleWin){
			StartCoroutine (playWin());
		}
		else {
			this.enabled = false;
		}
	}

	private void GetWinnerClips(){
		switch (GameManager.ins.chosenChar){
		case 1:
			char_wins = zach_wins;
			break;
		case 2:
			char_wins = ana_wins;
			break;
		case 3:
			char_wins = acb_wins;
			break;
		case 4:
			char_wins = winnie_wins;
			break;
		default: 
			break;
		}
	}

	IEnumerator playWin(){
		if (!soundPlayed){

			audio.clip = char_wins[Random.Range (0, char_wins.Length)];
			audio.Play ();
			soundPlayed = true;
		}

		yield return new WaitForSeconds (audio.clip.length);
		this.enabled = false;
	}
}
