﻿using UnityEngine;
using System.Collections;

// ONLY AFFECTS HEAD RIGIDBODY AND CAMERA
// ADDITIONAL DRUNK FORCE (NOT PLAYER MOVEMENT) - ALSO CONTAINS CAM WOBBLE SCRIPT!

public class DrunkForce : MonoBehaviour {
	
	// DRUNK FORCE PARAMS
	private enum Dir { forward, right, left, back }; // to modify drunkDir
	private int drunkDir; 	// randomly changing after coroutine delays (random delays?)
	public float drunkInc;	// amount by which we increment/multiply the toppling force
	
	// CAMERA PARAMS
	public float camInc; 	// cam wobble amount
	public float camAcc;    // cam wobble acceleration
	public float camHiCap; 	// cam wobble high-cap
	public float camLoCap;	// cam wobble low-cap

	private bool camHiCapped;	// cam wobble high-cap reached
	private bool camLoCapped;	// cam wobble low-cap reached
	
	// TO DRAG INTO COMPONENT
	public Rigidbody rhead; 	// object's head rigidbody
	public DrunkMovement player; 	// script from the player   

	
	// COROUTINE DELAYS
	public float drunkDelay;
	
	// TO STOP WOBBLE WHEN HIT A WALL
	public bool stopWobble; 
	public bool recoiled; 

	// LOSE CONDITION: IF CAMERA IS TOO CRAZY U LOOKIN INSANE
	public float boundRotForward;	// x < 30
	public float boundRotBack;		// x > 330
	public float boundRotRight;		// z < 30
	public float boundRotLeft; 		// z > 330 

	//CAMERA FX
	public DepthOfFieldScatter dof;


	void Start () {
		stopWobble = false; 
	}
	
	void Update () {
		// check camera rotation lose condition
		rotateFall (); 

		// camera wobble
		if (!stopWobble) {
			if (player.radius < player.maxRad) {
				//camWobble (switchBackForward(player.direction)); 	// USE THIS IF INVERTED BACK/FWD AT -90 DEGREES
				camWobble (player.direction); 
			}
		}
		
		// drunk force
		StartCoroutine(newDrunkDirection ());
		drunkForce (drunkDir);


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

		/*if (recoiled){
			print ("resetting the rotation after recoil");
			transform.rotation = Quaternion.Lerp (transform.rotation, new Quaternion(transform.rotation.x, 0, 0, transform.rotation.w), 0.5f*Time.deltaTime);
			recoiled = false; 
		}*/
	}

	void FixedUpdate(){

	}



	// head rotation too far = fall over

	private void rotateFall(){
		if ((transform.localEulerAngles.x > boundRotForward && transform.localEulerAngles.x < boundRotBack)	||	
		    (transform.localEulerAngles.z > boundRotRight && transform.localEulerAngles.z < boundRotLeft)){
			Debug.Log("LOST BC OF ANGLE");
			player.fallen = true;
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
		//print ("leaning");


		//DEPTH OF FIELD STUFF
		/*if (player.radius >= 0.3f){
			dof.aperture += (Mathf.Abs(camInc)*500);
		} else if (player.radius < 0.3f){
			dof.aperture -= (Mathf.Abs(camInc)*500);
		}*/

		switch (lean) {
		case (int) Dir.forward:
			if (!camHiCapped){
				camInc += camAcc;
			}
			transform.rotation = new Quaternion (transform.rotation.x + camInc, transform.rotation.y, transform.rotation.z, transform.rotation.w); 
			break;
		case (int) Dir.right:
			if (!camLoCapped){
				camInc -= camAcc;
			}
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - camInc*0.1f, transform.rotation.z + camInc, transform.rotation.w); 
			break;
		case (int) Dir.left:
			if (!camHiCapped){
				camInc += camAcc;
			}
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - camInc*0.1f, transform.rotation.z + camInc, transform.rotation.w); 
			break;
		case (int) Dir.back:
			if (!camLoCapped){
				camInc -= camAcc;
			}
			transform.rotation = new Quaternion (transform.rotation.x + camInc, transform.rotation.y, transform.rotation.z, transform.rotation.w); 
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
