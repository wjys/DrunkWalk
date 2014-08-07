using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	public GameObject bed;
	public GameObject me;
	public Transform bedSpriteScale;

	// COMPASS OBJECTS
	public GameObject compassBed;
	public GameObject compassBar; 
	public float compassBound = 4.4f;
	public Quaternion rot;
	public float angle; 
	private float initX; 

	// BED SCALE VARS
	public float maxScaleRad;
	public float minBedScale;
	public float smooth;


	// Use this for initialization
	void Start () { 
		GetScaleRate ();
		GetBedSprite ();
	}
	
	// Update is called once per frame
	void Update () {
		scaleBed ();
		compassCheck (); 
		compassObjects ();
	}

	void FixedUpdate(){
		if (bedSpriteScale.localScale.x >= 0.75f){
			bedSpriteScale.localPosition = Vector3.Lerp (bedSpriteScale.localPosition, new Vector3 (bedSpriteScale.localPosition.x, 0.0f, bedSpriteScale.localPosition.z), smooth * Time.deltaTime);
		}
		else {
			bedSpriteScale.localPosition = Vector3.Lerp (bedSpriteScale.localPosition, new Vector3 (bedSpriteScale.localPosition.x, 0.02f, bedSpriteScale.localPosition.z), smooth * Time.deltaTime);
		}
	}

	private void compassCheck(){
		Vector3 dirVector = bed.transform.position - me.transform.position;
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

	private void scaleBed(){
		float headX = me.transform.position.x;
		float headZ = me.transform.position.z;
		float bedX = bed.transform.position.x;
		float bedZ = bed.transform.position.z;
		
		float r = Mathf.Sqrt (((bedX - headX) * (bedX - headX)) + ((bedZ - headZ) * (bedZ - headZ)));
		float ratio = maxScaleRad / r;

		if (ratio >= 1) {
			ratio = 1;
		}
		bedSpriteScale.localScale = new Vector3 (ratio, ratio, ratio);
	}

	private void GetScaleRate(){
		float headX = me.transform.position.x;
		float headZ = me.transform.position.z;
		float bedX = bed.transform.position.x;
		float bedZ = bed.transform.position.z;
		
		float r = Mathf.Sqrt (((bedX - headX) * (bedX - headX)) + ((bedZ - headZ) * (bedZ - headZ)));	
		maxScaleRad = r * minBedScale;
	}

	private void GetBedSprite(){
		Transform[] trans = gameObject.GetComponentsInChildren<Transform> ();
		foreach (Transform t in trans) {
			if (t.name.Equals ("BedScale")){
				bedSpriteScale = t;
				break;
			}
		}
		bedSpriteScale.localPosition = new Vector3(bedSpriteScale.localPosition.x, 0.02f, bedSpriteScale.localPosition.z);
	}
}
