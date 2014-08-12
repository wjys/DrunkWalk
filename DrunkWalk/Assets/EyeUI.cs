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
				Eyes[0] = t.gameObject;
				SpriteRenderer[] rends = t.gameObject.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer r in rends){
					if (r.gameObject.name.Equals("PupilAware")){
						Pupil1[0] = r;
					}
					else if (r.gameObject.name.Equals ("PupilDrowsy")){
						Pupil2[0] = r;
					}
					else if (r.gameObject.name.Equals ("PupilSleep")){
						Pupil3[0] = r;
					}
				}

			} else if (t.gameObject.name.Equals ("Reye")) {
				Eyes[1] = t.gameObject;
				SpriteRenderer[] rends = t.gameObject.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer r in rends){
					if (r.gameObject.name.Equals("PupilAware")){
						Pupil1[0] = r;
					}
					else if (r.gameObject.name.Equals ("PupilDrowsy")){
						Pupil2[0] = r;
					}
					else if (r.gameObject.name.Equals ("PupilSleep")){
						Pupil3[0] = r;
					}
				}

			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
