using UnityEngine;
using System.Collections;

// ONLY AFFECTS HEAD RIGIDBODY AND CAMERA
// ADDITIONAL DRUNK FORCE (NOT PLAYER MOVEMENT) - ALSO CONTAINS CAM WOBBLE SCRIPT!

public class TopplingForce : MonoBehaviour {

	// DRUNK FORCE PARAMS
	private enum Dir { forward, right, left, back }; // to modify drunkDir
	private int drunkDir; 	// randomly changing after coroutine delays (random delays?)
	public float drunkInc;	// amount by which we increment/multiply the toppling force

	// CAMERA PARAMS
	public float camInc; 	// cam wobble amount
	public float boundCamForward;
	public float boundCamBack;
	public float boundCamRight;
	public float boundCamLeft; 

	// TO DRAG INTO COMPONENT
	public Rigidbody rhead; 	// object's head rigidbody
	public PlayerMovement playerMovement; 	// script from the player   

	// COROUTINE DELAYS
	public float drunkDelay;

	// TO STOP WOBBLE WHEN HIT A WALL
	public bool hitWall; 


	void Start () {
		hitWall = false; 
	}

	void Update () {
		// check camera rotation lose condition
		// camLose (transform.rotation, playerMovement.direction);

		// camera wobble
		if (!hitWall) {
			camWobble (playerMovement.direction); 
		}

		// drunk force
		StartCoroutine(newDrunkDirection ());
		drunkForce (drunkDir); 
		
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
			transform.rotation = new Quaternion (transform.rotation.x + camInc, transform.rotation.y, transform.rotation.z, transform.rotation.w); 
			break;
		case (int) Dir.right:
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, transform.rotation.z - camInc, transform.rotation.w); 
			break;
		case (int) Dir.left:
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, transform.rotation.z + camInc, transform.rotation.w); 
			break;
		case (int) Dir.back:
			transform.rotation = new Quaternion (transform.rotation.x - camInc, transform.rotation.y, transform.rotation.z, transform.rotation.w); 
			break;
		default:
			break; 
		}
	}
	/* --------------------------------------------------------------------------------------------------------------------------
	 * EXTRA LOSE CONDITION
	 * Given inputted CURRENT CAMERA ROTATION, we check if the rotation exceeds the cap
	 * => lose if exceeding cap
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void camLose(Quaternion camRotation, int direction){

		print("LOST BECAUSE CAMERA ANGLE");

		switch (direction) {
		case (int) Dir.forward:
			if (camRotation.x > boundCamForward){
				playerMovement.fallToLose (); 
			}
			break;
		case (int) Dir.right:
			if (camRotation.x > boundCamBack) {
				playerMovement.fallToLose (); 
			}
			break;
		case (int) Dir.left:
			if (camRotation.z > boundCamRight) {
				playerMovement.fallToLose ();
			}
			break;
		case (int) Dir.back:
			if (camRotation.z > boundCamLeft) {
				playerMovement.fallToLose ();
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
