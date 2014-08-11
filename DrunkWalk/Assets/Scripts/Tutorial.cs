using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	public string[] inst = new string[7] {	"",	// 0
											"Lean to move",	// 1
											"Don't move too fast, or your feet won't catch up with you!",	// 2
											"Lean and turn upper body slightly to begin turning",			// 3
											"Do the same in the opposite direction to stop",				// 4
											"", //"If you fall, tap trigger to stay awake!",				// 5
											"Walk to the door!"	};											// 6
	public int instIndex;
	public int _zero;
	public float instDelay;
	public bool alreadyFallen;
	public bool readingInstruction;
	public DrunkMovement dm;

	
	// Use this for initialization
	void Start () {
		instIndex = 0;
		_zero = 0;
		alreadyFallen = false;
		dm = GameObject.Find ("Head 1").GetComponent<DrunkMovement>();
		readingInstruction = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.ins.status == GameState.GameStatus.Tutorial){
			if (!gameObject.guiText.enabled){
				gameObject.guiText.enabled = true;
			}
			if (instIndex != 5){
				if (dm.fallen){
					gameObject.guiText.text = inst[_zero];
					alreadyFallen = true;
					readingInstruction = false;
				}
				else if (!readingInstruction){
					StartCoroutine (switchInstructions());
				}
			}
			else if (instIndex == 5){
				instIndex = 0;
				alreadyFallen = true;
			}
		}
	}

	IEnumerator switchInstructions(){
		gameObject.guiText.text = inst[instIndex];
		readingInstruction = true;
		yield return new WaitForSeconds (instDelay);
		switch (instIndex){
		case 0:
			if (alreadyFallen){
				instDelay = 10;
				instIndex = 6;
			}
			else {
				instIndex = 1;
				instDelay = 3;
			}
			break;
		case 1:
			instDelay = 3;
			instIndex = 2;
			break;
		case 2:
			instDelay = 5;
			instIndex = 3;
			break;
		case 3:
			instDelay = 5;
			instIndex = 4;
			break;
		case 4:
			if (!alreadyFallen){
				dm.fallen = true;
				instIndex = 5;
			}
			else {
				instIndex = 6;
			}
			break;
		default:
			break;
		}
		readingInstruction = false;
	}
}
