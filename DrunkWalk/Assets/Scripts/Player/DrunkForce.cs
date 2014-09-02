using UnityEngine;
using System.Collections;

// ONLY AFFECTS HEAD RIGIDBODY AND CAMERA
// ADDITIONAL DRUNK FORCE (NOT PLAYER MOVEMENT) - ALSO CONTAINS CAM WOBBLE SCRIPT!

public class DrunkForce : InGame {

	// -------- COMPONENTS TO GET IN START() -------- 
	public Rigidbody rhead; 	// object's head rigidbody
	public DrunkMovement dm; 	// script from the player
	public GameObject feet;

	// -------- PRIVATE GAME OBJECT TO INSTANTIATE FROM PREFAB --------
	private GameObject pfeet; 	// stands for "player" feet

	// -------- PRIVATE VARIABLES --------
	// drunk force stuff
	private enum Dir { forward, right, left, back }; 	// to modify drunkDir
	private int drunkDir; 		// randomly changing after coroutine delays (random delays?)
	private bool camHiCapped;	// cam wobble high-cap reached
	private bool camLoCapped;	// cam wobble low-cap reached


	// -------- PUBLIC VARIABLES --------

	// DRUNK FORCE PARAMS

	private float drunkInc;	// amount by which we increment/multiply the toppling force
	private float dWobble;
	
	// CAMERA PARAMS
	public float smooth; 
	public float camInc; 	// cam wobble amount
	public float camAcc;    // cam wobble acceleration
	public float camHiCap; 	// cam wobble high-cap
	public float camLoCap;	// cam wobble low-cap
	public float rotInc;	// cam orient inc
	
	// COROUTINE DELAYS
	public float drunkDelay;

	// LOSE CONDITION: IF CAMERA IS TOO CRAZY U LOOKIN INSANE
	public float boundRotForward;	// x < 30
	public float boundRotBack;		// x > 330
	public float boundRotRight;		// z < 30
	public float boundRotLeft; 		// z > 330 

	// TO STOP WOBBLE WHEN HIT A WALL
	public bool stopWobble; 
	public bool recoiled; 

	void Start () {
		rhead = gameObject.GetComponent<Rigidbody> ();
		dm = gameObject.GetComponent<DrunkMovement> ();
		stopWobble = false;
		drunkInc = 0;
		dWobble = 0;

		//Depending on total drinks, random drunk force
		if (GameManager.ins.numOfPlayers == 1) {
			if (GameManager.ins.diffInt == 0){
				drunkInc = 0;
			} else if (GameManager.ins.diffInt >= 1 && GameManager.ins.diffInt <= 2) {
								drunkInc = 0.02f;
						} else if (GameManager.ins.diffInt <= 4) {
								drunkInc = 0.05f;
						} else if (GameManager.ins.diffInt <= 5) {
								drunkInc = 0.08f;
						}

						//Depending on total amount of GIN, wobble becomes harsher
								if (GameManager.ins.GinInt >= 1 && GameManager.ins.GinInt <= 2) {
										dWobble = 0.3f;
								} else if (GameManager.ins.GinInt <= 4) {
										dWobble = 0.8f;
								} else if (GameManager.ins.GinInt <= 5) {
										dWobble = 1.1f;
								} else {
										dWobble = 0;
								}
				}
	}
	
	void Update () {
		feet = dm.pfeet;
		// check camera rotation lose condition
		/*if ((transform.localEulerAngles.x > boundRotForward && transform.localEulerAngles.x < boundRotBack)	||	
		    (transform.localEulerAngles.z > boundRotRight && transform.localEulerAngles.z < boundRotLeft)){
			Debug.Log("LOST BC OF ANGLE");
			dm.fallen = true;
		}*/

		// camera wobble
		if (!stopWobble) {
			if (dm.radius < dm.maxRad) {
				//camWobble (switchBackForward(player.direction)); 	// USE THIS IF INVERTED BACK/FWD AT -90 DEGREES
				camWobble (dm.direction); 
			}
		}
		
		// DRUNK FORCE
		//if (GameManager.ins.diffInt > 0){
		//	StartCoroutine(newDrunkDirection ());
		//	drunkForce (drunkDir);
		//}


		// check camera rotation caps
		if (camInc >= camHiCap){
			camHiCapped = true;
		} else {
			camHiCapped = false;
		}

		if (camInc <= camLoCap){
			camLoCapped = true;
		} else {
			camLoCapped = false;
		}
	}

	void FixedUpdate(){
		if (recoiled){
//			print ("resetting the rotation after recoil");
			transform.rotation = Quaternion.Lerp (transform.rotation, new Quaternion(transform.rotation.x, 0, 0, transform.rotation.w), 0.5f*Time.deltaTime);
			recoiled = false; 
		}
		if (stopWobble){
			transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w), 0.5f * Time.deltaTime);
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * DRUNKFORCE 
	 * Depending on the (random) inputted direction, add a drunk force in the corresponding direction
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	// depending on the direction of the lean, set a constantforce on the rigidbody of the head 
	private void drunkForce (int direction){	//print ("moving head ");

		switch (direction) {
			
		case (int) Dir.forward:					//print ("moving head forward");
			rhead.AddForce (0, 0, drunkInc);  
			break;
			
		case (int) Dir.right:					//print ("moving head to the right");
			rhead.AddForce (drunkInc, 0, 0); 
			break;
			
		case (int) Dir.left:					//print ("moving head to the left");
			rhead.AddForce (-drunkInc, 0, 0); 
			break;
			
		case (int) Dir.back:
			rhead.AddForce (0, 0, -(drunkInc/2));
			break;
			
		default:
			break; 
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * CAMERA WOBBLE
	 * Given inputted LEAN DIRECTION, the camera will wobble correspondingly
	 * (1) If leaning forward of backward, progressively rotate on the x-axis 
	 * (2) If leaning right/left, progressively rotate on the z-axis
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	// FOR NOW camera wobbles as lean changes 
	private void camWobble(int lean){

		switch (lean) {
		case (int) Dir.forward:
			transform.localRotation = Quaternion.Lerp (transform.localRotation, 
			                                           Quaternion.Euler	(Mathf.Rad2Deg*(Mathf.Atan((Mathf.Abs(transform.position.y - feet.transform.position.y))/(Mathf.Abs(transform.position.z - feet.transform.position.z))*(4+dWobble))), transform.localEulerAngles.y, transform.localEulerAngles.z), 
			                                           Time.deltaTime * (smooth));
			break;
		case (int) Dir.right:
			if (transform.localRotation.z <= -0.13f){
				transform.localRotation = new Quaternion (transform.localRotation.x, transform.localRotation.y, -0.13f, transform.localRotation.w);
			}
			else {
				transform.localRotation = Quaternion.Lerp (transform.localRotation, 
				                                           Quaternion.Euler (transform.localEulerAngles.x, transform.localEulerAngles.y, -Mathf.Rad2Deg*(Mathf.Atan((Mathf.Abs(transform.position.y - feet.transform.position.y))/(Mathf.Abs(transform.position.x - feet.transform.position.x)))*(1.5f+dWobble))), 
				                                           Time.deltaTime * smooth);
			}
			break;
		case (int) Dir.left: 
			if (transform.localRotation.z >= 0.13f){
				transform.localRotation = new Quaternion (transform.localRotation.x, transform.localRotation.y, 0.13f, transform.localRotation.w);
			}
			else {
				transform.localRotation = Quaternion.Lerp (transform.localRotation, 
				                                           Quaternion.Euler (transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Rad2Deg*(Mathf.Atan((Mathf.Abs(transform.position.y - feet.transform.position.y))/(Mathf.Abs(transform.position.x - feet.transform.position.x)))*(1.5f+dWobble))), 
				                                           Time.deltaTime * smooth);
			}
			break;
		case (int) Dir.back:
			if (transform.localRotation.x <= -0.1f){
				transform.localRotation = new Quaternion (-0.1f, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
			}
			else {
				transform.localRotation = Quaternion.Lerp (transform.localRotation, 
			    	                                       Quaternion.Euler (-Mathf.Rad2Deg*(Mathf.Atan((Mathf.Abs(transform.localPosition.y - feet.transform.localPosition.y))/(Mathf.Abs(transform.localPosition.z - feet.transform.localPosition.z)))*(0.5f+dWobble)),  transform.localEulerAngles.y, transform.localEulerAngles.z), 
			        	                                   Time.deltaTime * (smooth));
			}
			break;
		default:
			break; 
		}
	}



	/* --------------------------------------------------------------------------------------------------------------------------
	 * Delay changing the DRUNK DIRECTION to trigger DRUNK FORCE
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	IEnumerator newDrunkDirection(){
		yield return new WaitForSeconds(drunkDelay);
		drunkDir = Random.Range ((int) Dir.forward, (int) Dir.back+1);
	}
}
