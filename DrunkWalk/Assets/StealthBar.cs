using UnityEngine;
using System.Collections;

public class StealthBar : MonoBehaviour {
	public GameObject[] StealthIcons;
	public Sprite[] noiseIcons;

	void Start () {
		StealthIcons = new GameObject[3];
		Transform[] trans = gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform t in trans){
			if (t.gameObject.name.Equals ("Alarm")){
				StealthIcons[0] = t.gameObject;
			} else if (t.gameObject.name.Equals ("NoiseIcon")){
				StealthIcons[1] = t.gameObject;
			} else if (t.gameObject.name.Equals ("NoiseLevel")){
				StealthIcons[2] = t.gameObject;
			}
		}
	}
	
	void Update () {

	}
}
