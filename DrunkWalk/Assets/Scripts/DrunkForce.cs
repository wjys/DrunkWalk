using UnityEngine;
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
	
	// TO DRAG INTO COMPONENT
	public Rigidbody rhead; 	// object's head rigidbody
	public DrunkMovement player; 	// script from the player   
	
	// COROUTINE DELAYS
	public float drunkDelay;
	
	// TO STOP WOBBLE WHEN HIT A WALL
	public bool hitWall; 

	// LOSE CONDITION: IF CAMERA IS TOO CRAZY U LOOKIN INSANE
	public float boundRotForward;	// x < 30
	public float boundRotBack;		// x > 330
	public float boundRotRight;		// z < 30
	public float boundRotLeft; 		// z > 330 

	
	void Start () {
		hitWall = false; 
	}
	
	void Update () {
		// check camera rotation lose condition
		rotateFall (); 

		// camera wobble
		if (!hitWall) {
			camWobble (player.direction); 
		}
		
		// drunk force
		StartCoroutine(newDrunkDirection ());
		drunkForce (drunkDir);
	}

	private void rotateFall(){
		print ("checking rotation"); 
		if ((transform.eulerAngles.x > boundRotForward && transform.eulerAngles.x < boundRotBack)	||	
		    (transform.eulerAngles.z > boundRotRight && transform.eulerAngles.z < boundRotLeft)){
			player.tapsToGetUp(); 
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
			camInc += camAcc;
			transform.rotation = new Quaternion (transform.rotation.x + camInc, transform.rotation.y, transform.rotation.z, transform.rotation.w); 
			break;
		case (int) Dir.right:
			camInc -= camAcc;
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, transform.rotation.z - camInc, transform.rotation.w); 
			break;
		case (int) Dir.left:
			camInc += camAcc;
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, transform.rotation.z + camInc, transform.rotation.w); 
			break;
		case (int) Dir.back:
			camInc -= camAcc;
			transform.rotation = new Quaternion (transform.rotation.x - camInc, transform.rotation.y, transform.rotation.z, transform.rotation.w); 
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
