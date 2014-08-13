using UnityEngine;
using System.Collections;

public class PlayerNum : MonoBehaviour {
	public SpriteRenderer[] renderer;
	public Sprite[] resultChars;

	// Use this for initialization
	void Start () {
		renderer = gameObject.GetComponentsInParent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		foreach (SpriteRenderer r in renderer){
			if (!r.name.Equals ("EndCrown")){
				if (r.sprite == resultChars[0]) {
					transform.localPosition = new Vector3 (0.73f, transform.localPosition.y, transform.localPosition.z);
				} else if (r.sprite == resultChars[1]) {
					transform.localPosition = new Vector3 (0.8f, 0.8f, transform.localPosition.z);
				} else if (r.sprite == resultChars[2]) {
					transform.localPosition = new Vector3 (0.26f, 2.48f, transform.localPosition.z);
				} else if (r.sprite == resultChars[3]) {
					transform.localPosition = new Vector3 (2.17f, 1.24f, transform.localPosition.z);
				} else if (r.sprite == resultChars[4]) {
					transform.localPosition = new Vector3 (-0.73f, 2.09f, transform.localPosition.z);
				} else if (r.sprite == resultChars[5]) {
					transform.localPosition = new Vector3 (-2.36f, -0.38f, transform.localPosition.z);
				} else if (r.sprite == resultChars[6]) {
					transform.localPosition = new Vector3 (0.0f, 2.8f, transform.localPosition.z);
				} else if (r.sprite == resultChars[7]) {
					transform.localPosition = new Vector3 (-1.36f, 1.17f, transform.localPosition.z);
				}
			}
		}
	}
}
