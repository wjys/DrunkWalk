using UnityEngine;
using System.Collections;

// ONLY AFFECTS HEAD RIGIDBODY AND CAMERA
// ADDITIONAL DRUNK FORCE (NOT PLAYER MOVEMENT) - ALSO CONTAINS CAM WOBBLE SCRIPT!

public class TopplingForce : MonoBehaviour {

	private enum Dir { forward, right, left, back }; // to modify drunkDir
	private int drunkDir; 	// randomly changing after coroutine delays (random delays?)
	public float drunkInc;	// amount by which we increment/multiply the toppling force
	public float camInc; 

	// TO DRAG INTO COMPONENT
	public Rigidbody rhead; 	// object's head rigidbody
	public PlayerMovement playerMovement; 	// script from the player  
	public Camera cam; 

	// COROUTINE DELAYS
	public float drunkDelay;

	void Start () {
	}

	void Update () {
		// check camera rotation lose condition
		camLose (cam.transform.rotation);

		// camera wobble
		camWobble (playerMovement.direction); 

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
			cam.transform.rotation = new Quaternion (cam.transform.rotation.x + camInc, cam.transform.rotation.y, cam.transform.rotation.z, cam.transform.rotation.w); 
			break;
		case (int) Dir.right:
			cam.transform.rotation = new Quaternion (cam.transform.rotation.x, cam.transform.rotation.y + camInc*2, cam.transform.rotation.z - camInc, cam.transform.rotation.w); 
			break;
		case (int) Dir.left:
			cam.transform.rotation = new Quaternion (cam.transform.rotation.x, cam.transform.rotation.y - camInc*2, cam.transform.rotation.z + camInc, cam.transform.rotation.w); 
			break;
		case (int) Dir.back:
			cam.transform.rotation = new Quaternion (cam.transform.rotation.x - camInc, cam.transform.rotation.y, cam.transform.rotation.z, cam.transform.rotation.w); 
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

	private void camLose(Quaternion camRotation){
		//playerMovement.fallToLose (); 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * Delay changing the DRUNK DIRECTION to trigger DRUNK FORCE
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	IEnumerator newDrunkDirection(){
		yield return new WaitForSeconds(drunkDelay);
		drunkDir = Random.Range ((int) Dir.forward, (int) Dir.back+1);
	}
}
