using UnityEngine;
using System.Collections;

public class CharacterReel : MonoBehaviour {
	private Quaternion newRot;
	public int charID;
	public float smooth;

	public GameObject[] chars;

	//Idle Animations
	//public Animator charAnim;

	// Use this for initialization
	void Start () {
		charID = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (charID == 0){
			newRot = new Quaternion(0,0,0,1);
		} else if (charID == 1){
			newRot = new Quaternion(0, 0.7071066f, 0, 0.7071066f);
		} else if (charID == 2){
			newRot = new Quaternion(0,1,0, 4.321336e-07f);
		} else if (charID == 3){
			newRot = new Quaternion(0,0.7071073f, 0, -0.7071073f);
		}
	}

	void FixedUpdate () {
		gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, newRot, smooth * Time.deltaTime);
	}
}
