using UnityEngine;
using System.Collections;

public class EyeUI : MonoBehaviour {
	public GameObject[] Eyes;
	public SpriteRenderer[] Pupil1, Pupil2, Pupil3;
	public Sprite[] EyeSprites;
	public DrunkMovement dm;

	// Use this for initialization
	void Start () {
	//get Eye Objects
		Transform[] trans = gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform t in trans){
			if (t.gameObject.name.Equals ("Leye")) {
				SpriteRenderer[] rends = t.gameObject.GetComponentsInChildren<SpriteRenderer>();

			} else if (t.gameObject.name.Equals ("Reye")) {

				SpriteRenderer[] rends = t.gameObject.GetComponentsInChildren<SpriteRenderer>();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
