using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	public GameObject couch;
	public GameObject[] tubs;
	public GameObject target;
	public GameObject me;
	public Transform spriteScale;
	
	// COMPASS OBJECTS
	public GameObject compassBed;
	public GameObject compassCouch;
	public GameObject compassTub;
	public GameObject compassBar; 
	public float compassBound = 4.4f;
	public Quaternion rot;
	public float angle; 
	private float initX; 
	
	// BED SCALE VARS
	public float maxScaleRad;
	public float minBedScale;
	public float smooth;

	// 
	
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * START
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void Start () { 
		//GetScaleRate ();
		//GetBedSprite ();
		GetCompassSprites();
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * UPDATE
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void Update () {
		//scaleBed ();
		if (me.activeSelf){
			if (GameManager.ins.mode == GameState.GameMode.Party){
				switchTarget();
			}
			compassCheck (); 
			compassObjects ();
		}
		else {
			compassCouch.SetActive (false);
			compassBed.SetActive (false);
			compassTub.SetActive (false);
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * (NOT USED) FIXED UPDATE: for scaling the sprite
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void FixedUpdate(){
		/*if (spriteScale.localScale.x >= 0.75f){
			spriteScale.localPosition = Vector3.Lerp (spriteScale.localPosition, new Vector3 (spriteScale.localPosition.x, 0.0f, spriteScale.localPosition.z), smooth * Time.deltaTime);
		}
		else {
			spriteScale.localPosition = Vector3.Lerp (spriteScale.localPosition, new Vector3 (spriteScale.localPosition.x, 0.02f, spriteScale.localPosition.z), smooth * Time.deltaTime);
		}*/
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * COMPASS CHECK: get the rotation/angle to check for determining where to place compass sprites
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void compassCheck(){
		Vector3 dirVector = target.transform.position - me.transform.position;
		Vector3 dirFacing = me.transform.TransformDirection (Vector3.forward); 
		dirVector.y = 0;
		dirFacing.y = 0;
		
		rot = Quaternion.FromToRotation(dirVector, dirFacing);
		angle = Vector3.Angle (dirVector, dirFacing);
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * COMPASS OBJECTS: place the compass objects depending on angle/rotation gotten above
	 * -------------------------------------------------------------------------------------------------------------------------- */

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

	/* --------------------------------------------------------------------------------------------------------------------------
	 * GET COMPASS SPRITES: get gameobjects with couch and tub sprite renderers
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void GetCompassSprites(){
		SpriteRenderer[] renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer r in renderers){
			if (compassCouch != null && compassTub != null){
				break;
			}
			if (r.name.Equals ("BedSprite")){
				compassBed = r.gameObject;
			}

			if (r.name.Equals ("CouchSprite")){
				compassCouch = r.gameObject;
				r.enabled = false;
			}
			else if (r.name.Equals("TubSprite")){
				compassTub = r.gameObject;
				r.enabled = false;;
			}
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * SWITCH TARGET
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void switchTarget(){
		GameManager gm = GameManager.ins.GetComponent<GameManager>();
		if (gm.winnerIndex > 0){
			if (gm.winnerIndex == 1){
				SpriteRenderer rend = compassCouch.GetComponent<SpriteRenderer>();
				if (!rend.enabled){
					compassBed.SetActive (false);
					rend.enabled = true;
				}
				target = couch;
			}
			else if (gm.winnerIndex == 2){
				SpriteRenderer rend = compassTub.GetComponent<SpriteRenderer>();
				if (!rend.enabled){
					compassCouch.SetActive(false);
					rend.enabled = true;
				}
				getClosestTub ();
			}
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * GET CLOSEST TUB: go through the tub objects and find the one closest to the player and set that as the target
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void getClosestTub(){
		float[] tubDist = new float[tubs.Length];
		for (int i = 0; i < tubs.Length; i++){
			float headX = me.transform.position.x;
			float headZ = me.transform.position.z;
			float tubX = tubs[i].transform.position.x;
			float tubZ = tubs[i].transform.position.z;
			
			tubDist[i] = Mathf.Sqrt (((tubX - headX) * (tubX - headX)) + ((tubZ - headZ) * (tubZ - headZ)));
		}
		int minIndex = 0;
		float min = 0;
		for (int i = 0; i < tubDist.Length; i++){
			if (i == 0){
				min = tubDist[i];
			}
			else if (tubDist[i] < min){
				min = tubDist[i];
				minIndex = i;
			}
		}
		
		target = tubs[minIndex];
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/* --------------------------------------------------------------------------------------------------------------------------
	 * (NOT USED) GET BED SPRITE: the object to scale the bed
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void GetBedSprite(){
		Transform[] trans = gameObject.GetComponentsInChildren<Transform> ();
		foreach (Transform t in trans) {
			if (t.name.Equals ("BedScale")){
				spriteScale = t;
				break;
			}
		}
		spriteScale.localPosition = new Vector3(spriteScale.localPosition.x, 0.02f, spriteScale.localPosition.z);
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * (NOT USED) SCALE BED: scale bed depending on distance between player and bed
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void scaleBed(){
		float headX = me.transform.position.x;
		float headZ = me.transform.position.z;
		float bedX = target.transform.position.x;
		float bedZ = target.transform.position.z;
		
		float r = Mathf.Sqrt (((bedX - headX) * (bedX - headX)) + ((bedZ - headZ) * (bedZ - headZ)));
		float ratio = maxScaleRad / r;
		
		if (ratio >= 1) {
			ratio = 1;
		}
		spriteScale.localScale = new Vector3 (ratio, ratio, ratio);
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * (NOT USED) GET SCALE RATE: get max scale of bed sprite
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void GetScaleRate(){
		float headX = me.transform.position.x;
		float headZ = me.transform.position.z;
		float bedX = target.transform.position.x;
		float bedZ = target.transform.position.z;
		
		float r = Mathf.Sqrt (((bedX - headX) * (bedX - headX)) + ((bedZ - headZ) * (bedZ - headZ)));	
		maxScaleRad = r * minBedScale;
	}
}
