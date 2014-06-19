using UnityEngine;
using System.Collections;

// ONLY AFFECTS HEAD RIGIDBODY AND CAMERA
// ADDITIONAL DRUNK FORCE (NOT PLAYER MOVEMENT) - ALSO CONTAINS CAM WOBBLE SCRIPT!

public class TopplingForce : MonoBehaviour {

	public int toppleDir;	// currently toppling in this direction
	public int leanDir;		// constantly update to find current direction of lean
	public int drunkDir; 	// randomly changing after coroutine delays (random delays?)
	public bool isToppling;		// true if currently toppling in direction

	private enum Dir { forward, right, left, back }; // to modify drunkDir

	public float inc; 		// INC to add to BASE below
	public float accelbase;	// BASE accelerate in lean direction
	public float drunkInc;	// amount by which we increment/multiply the toppling force
	public float camInc; 

	// TO DRAG INTO COMPONENT
	public Rigidbody rhead; 	// object's head rigidbody
	public PlayerMovement playerMovement; 	// script from the player  
	public Camera cam; 

	// COROUTINE DELAYS
	public float drunkDelay;

	void Start () {
		isToppling = false; 
	}

	void Update () {
		leanDir = playerMovement.direction;

		if (toppleDir != leanDir) {
			isToppling = false; 
			toppleDir = leanDir; 
		}

		// camera wobble
		camWobble (leanDir); 

		// drunk force
		StartCoroutine(newDrunkDirection ());
		drunkForce (drunkDir); 

		// accelerate in current lean direction
		//accelForce (toppleDir); 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * !! NOT USED - ACCELERATE IN LEAN DIRECTION 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void accelForce (int direction){
		
		if (!isToppling) {
			inc = 0.0f; 
			isToppling = true; 
		}
		
		switch (direction) {
			
		case (int) Dir.forward:				//print ("moving head forward");
			rhead.AddForce (0, 0, accelbase + inc);  
			break;
			
		case (int) Dir.right:				//print ("moving head to the right");
			rhead.AddForce (accelbase + inc, 0, 0); 
			break;
			
		case (int) Dir.left:				//print ("moving head to the left");
			rhead.AddForce (-(accelbase + inc), 0, 0); 
			break;
			
		default:
			break; 
		}
		inc += 0.05f; 
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * DRUNKFORCE 
	 * Depending on the (random) inputted direction, add a drunk force in the corresponding direction
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	// depending on the direction of the lean, set a constantforce on the rigidbody of the head 
	private void drunkForce (int direction){	//print ("moving head ");
		
		switch (direction) {
			
		case (int) Dir.forward:				//print ("moving head forward");
			rhead.AddForce (0, 0, drunkInc);  
			break;
			
		case (int) Dir.right:				//print ("moving head to the right");
			rhead.AddForce (drunkInc, 0, 0); 
			break;
			
		case (int) Dir.left:				//print ("moving head to the left");
			rhead.AddForce (-drunkInc, 0, 0); 
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
			cam.transform.rotation = new Quaternion (cam.transform.rotation.x, cam.transform.rotation.y, cam.transform.rotation.z, cam.transform.rotation.w); 
			break;
		case (int) Dir.right:
			cam.transform.rotation = new Quaternion (cam.transform.rotation.x, cam.transform.rotation.y, cam.transform.rotation.z - camInc, cam.transform.rotation.w); 
			break;
		case (int) Dir.left:
			cam.transform.rotation = new Quaternion (cam.transform.rotation.x, cam.transform.rotation.y, cam.transform.rotation.z + camInc, cam.transform.rotation.w); 
			break;
		case (int) Dir.back:
			cam.transform.rotation = new Quaternion (cam.transform.rotation.x, cam.transform.rotation.y, cam.transform.rotation.z, cam.transform.rotation.w); 
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
		drunkDir = Random.Range ((int) Dir.forward, (int) Dir.back);
	}
}
