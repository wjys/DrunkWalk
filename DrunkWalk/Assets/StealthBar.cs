using UnityEngine;
using System.Collections;

public class StealthBar : MonoBehaviour {
	public GameObject alarm, noiseIcon, noiseLevel;
	public Sprite[] noiseIcons;

	void Start () {
		Transform[] trans = gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform t in trans){
			if (t.gameObject.name.Equals ("Alarm")){
				alarm = t.gameObject;
			} else if (t.gameObject.name.Equals ("NoiseIcon")){
				noiseIcon = t.gameObject;
			} else if (t.gameObject.name.Equals ("NoiseLevel")){
				noiseLevel = t.gameObject;
			}
		}
	}
	
	void Update () {

	}
}
