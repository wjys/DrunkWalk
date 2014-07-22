using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	public GameObject bed;
	public GameObject me;

	// COMPASS OBJECTS
	public GameObject compassBed;
	public GameObject compassBar; 
	public float compassBound = 4.4f;
	public Quaternion rot;
	public float angle; 
	private float initX; 
	// Use this for initialization
	void Start () { 
	}
	
	// Update is called once per frame
	void Update () {
		compassCheck (); 
		compassObjects ();
	}

	private void compassCheck(){
		Vector3 dirVector = bed.transform.position - me.transform.TransformDirection(Vector3.forward);
		Vector3 dirFacing = me.transform.TransformDirection (Vector3.forward); 
		dirVector.y = 0;
		dirFacing.y = 0;
		
		rot = Quaternion.FromToRotation(dirVector, dirFacing);
		angle = Vector3.Angle (dirVector, dirFacing);
	}

	private void compassObjects(){
		if (angle >= 90) {
			if (rot.y > 0){
				compassBar.transform.localPosition = new Vector3 (-compassBound, compassBar.transform.localPosition.y, compassBar.transform.localPosition.z); 

			}
			if (rot.y < 0){
				compassBar.transform.localPosition = new Vector3 (compassBound, compassBar.transform.localPosition.y, compassBar.transform.localPosition.z); 
			}
		}
		else {
			if (rot.y > 0){
				compassBar.transform.localPosition = new Vector3 (-compassBound*(angle/90.0f), compassBar.transform.localPosition.y, compassBar.transform.localPosition.z);
			}
			if (rot.y < 0){
				compassBar.transform.localPosition = new Vector3 (compassBound*(angle/90.0f), compassBar.transform.localPosition.y, compassBar.transform.localPosition.z);
			}
		}
	}
}
