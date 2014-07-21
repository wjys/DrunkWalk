using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	public GameObject bed;
	public GameObject me;

	public Quaternion rot;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 dirVector = bed.transform.position - me.transform.position;
		dirVector.y = 0;

		rot = Quaternion.FromToRotation(me.transform.position, dirVector);

		//print (rot);

		//rot.ToAngleAxis(
	}
}
