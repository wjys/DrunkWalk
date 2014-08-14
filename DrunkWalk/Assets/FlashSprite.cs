using UnityEngine;
using System.Collections;

public class FlashSprite : MonoBehaviour {

	public int flashCt;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.GetComponent<SpriteRenderer>().enabled){
			StartCoroutine (flash());
		}
	}

	IEnumerator flash(){
		yield return new WaitForSeconds(3);
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}
}
