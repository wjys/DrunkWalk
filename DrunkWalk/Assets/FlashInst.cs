using UnityEngine;
using System.Collections;

public class FlashInst : MonoBehaviour {

	public Sprite[] instructions;
	public SpriteRenderer renderer;
	public int instShown;

	// Use this for initialization
	void Start () {
		renderer = gameObject.GetComponent<SpriteRenderer>();
		instShown = 0;
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.ins.mode == GameState.GameMode.Party){
			switch (GameManager.ins.winnerIndex){
			case 0:
				if (instShown == 0){
					renderer.sprite = instructions [0];
					instShown = 1;
					if (!renderer.enabled){
						renderer.enabled = true;
						StartCoroutine (flash());
					}
				}
				break;
			case 1:
				if (instShown == 1){
					renderer.sprite = instructions [1];
					instShown = 2;
					if (!renderer.enabled){
						renderer.enabled = true;
						StartCoroutine (flash());
					}
				}
				break;
			case 2:
				if (instShown == 1){
					renderer.sprite = instructions [2];
					instShown = 3;
					if (!renderer.enabled){
						renderer.enabled = true;
						StartCoroutine (flash());
					}
				}
				break;
			default:
				break;
			}
		}
	}

	IEnumerator flash(){
		yield return new WaitForSeconds(3);
		renderer.enabled = false;
	}
}
