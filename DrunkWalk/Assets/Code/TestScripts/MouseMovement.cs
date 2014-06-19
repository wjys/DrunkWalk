using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// MOUSE POSITION TO READ WHERE PLAYER LEANS AND RESULTING MOVEMENT 

public class MouseMovement : MonoBehaviour {
	
	public Vector3 mouse;		// current mouse position on screen
	public float delay; 		// time delay between feet movement and head movement 
	public float hinc = 0.5f;	// force increment for head
	public float finc = 0.5f; 	// force increment for feet
	public float camInc = 0.5f; 

	// OBJECTS TO DRAG INTO COMPONENT
	public Rigidbody rhead;		// rigidbody at the head of the player
	public Rigidbody rfeet;		// rigidbody at the feet of the player
	public Camera cam; 			// to force the camera to just fall over if leaning too much
	public Camera fallCam; 		// cam to switch to if lose the game
	public DepthOfFieldScatter dof; // depth of field component on cam

	private int halfWidth; 		// half the width of screen
	private int halfHeight; 	// half the height of screen
	
	private enum Dir { forward, right, left, back }; 
	public int direction; 
	private bool fallen;
	private float angleBetween;
	private float maxAngle = 1.0f; 

	void Start () {
		halfWidth = Screen.width / 2; 
		halfHeight = Screen.height / 2; 
		
		fallen = false;
	}
	
	void Update () {
		// if the player has leaned too much, FALL AND LOSE
		if (fallen) {
			fallToLose();
		}

		// else, lean and drunk walk
		else {
			// get the current mouse position
			mouse = Input.mousePosition; 
			angleBlur (angleBetween);
			direction = getLeanDirection (mouse); 	//print ("1. got direction");
			fallen = isLeaningTooMuch (); 
			moveHead (direction); 					//print ("2. moved head"); 
			StartCoroutine(delayFeet ()); 			//print ("3. delayed feet");
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * PARAM: angleBetween = the angle between the head/feet vector and the vertical vector
	 * The closer angleBetween is to 30.0f, the blurrier things get!
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void angleBlur (float angle){

		print ("ap = " + dof.aperture); 
		
		if (angle >= 0.4f && angle <= maxAngle){
			dof.aperture += 0.1f;
		} else if (angle < 0.4f){
			dof.aperture -= 0.8f;
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * PARAM: mouse vector3 (where it is) => return the direction in which the character should be leaning 
	 * 
	 * (a) the mouse is currently in the bottom half of the game screen
	 * 		(a1) mouse is further down than in either L/R directions of the screen => lean BACK
	 * 		(a2) mouse is further to the RIGHT => lean RIGHT
	 * 		(a3) mouse is further to the LEFT => lean LEFT
	 * 
	 * (b) mouse currently in upper half of the game screen
	 * 		(b1) mouse is further up than to the L/R sides => lean FORWARD
	 * 		(b2) mouse is further to the RIGHT => lean RIGHT
	 * 		(b3) mouse is further to the LEFT => lean LEFT
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private int getLeanDirection(Vector3 mouse){	//print("entered get direction");

		// (a) 
		if (mouse.y < halfHeight) {	
			// (a1)
			if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){ 
				return (int) Dir.back; 
			}
			else {
				// (a2)
				if (mouse.x >= halfWidth){		// print ("leaning right");
					return (int) Dir.right; 
				}
				// (a3)
				else {							// print ("leaning left");
					return (int) Dir.left; 
				}
			}
		}
		// (b)
		else {
			// (b1)
			if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){	// print ("leaning forward");
				return (int) Dir.forward; 
			}
			else {
				// (b2)
				if (mouse.x >= halfWidth){ 	// print ("leaning right");
					return (int) Dir.right; 
				}
				// (b3)
				else {						// print ("leaning left");
					return (int) Dir.left; 
				}
			}
		}
		return (0);
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * (1) Check the current angle between the vector between the head rigidbody and the feed rigidboy with the vertical vector
	 * (2) If the angle is at least 30 degrees, then you are leaning too much! (return true)
	 * (3) otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private bool isLeaningTooMuch(){ //print ("checking lean");
		Vector3 vertVec = new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z); 

		// (1) check angle between vectors
		float angle = Vector3.Angle (vertVec, rhead.position); 
		angleBetween = angle;
		//print ("angle = " + angle); 

		// (2) if angle is at least 30
		if (angle >= maxAngle) { 	// print ("FALLEN!");
			return true;
		} 						// print ("STILL STANDING");
		return false; 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * !! FOR NOW, IF LEAN TOO MUCH, FALL AND LOSE THE GAME
	 * 
	 * (1) switch from main camera view to the fall camera view (going down to the floor)
	 * (2) play fallING sounds (progressively going down sounds)
	 * (3) play some kind of blink or blackout animation (fade out to black)
	 * (4) play fallEN to floor sound (random between many different fallen sounds)
	 * (5) display "YOU LOST/TRY AGAIN" 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void fallToLose(){	print ("YOU LOSE"); 

		// (1) switch cameras: from main to fallCam
		cam.enabled = false;
		fallCam.enabled = true; 

		// (2) play FALLING sounds

		// (3) animation? blink/blackout 

		// (4) play FALLEN TO FLOOR sound

		// (5) display "YOU LOST. DO SOMETHING TO TRY AGAIN" or something
	}
	

	/* --------------------------------------------------------------------------------------------------------------------------
	 * DEPENDING ON THE LEAN DIRECTION, ADD A FORCE TO THE RIGIDBODY OF THE HEAD
	 * 
	 * (1) switch/case to check which direction we're leaning
	 * (2) add the force in the appropriate direction
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void moveHead (int direction){	//print ("moving head ");
		
		switch (direction) {
			
		case (int) Dir.forward:				//print ("moving head forward");
			rhead.AddForce (0, 0, hinc);  
			break;
			
		case (int) Dir.right:				//print ("moving head to the right");
			rhead.AddForce (hinc, 0, 0); 
			break;
			
		case (int) Dir.left:				//print ("moving head to the left");
			rhead.AddForce (-hinc, 0, 0); 
			break;
			
		case (int) Dir.back:				//print ("stopping head movement");
			rhead.AddForce (0, 0, -hinc); 
			//rhead.position = new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z); 
			break; 
			
		default:
			break; 
		}
	}
	

	/* --------------------------------------------------------------------------------------------------------------------------
	 * DEPENDING ON DIRECTION OF THE LEAN, ADD A FORCE TO THE FEET RIGIDBODY 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void moveFeet (int direction){			//print ("moving feet");
		switch (direction) {
			
		case (int) Dir.forward:						//print ("moving feet forward");
			rfeet.AddForce (0, 0, finc);
			break;
			
		case (int) Dir.right:						//print ("moving feet right");
			rfeet.AddForce (finc, 0, 0);
			break;
			
		case (int) Dir.left:						//print ("moving feet left");
			rfeet.AddForce (-finc, 0, 0);
			break;
			
			// if player leans back, the feet will match the feet 
		case (int) Dir.back:						//print ("stopping feet under head");
			rfeet.AddForce (0, 0, -finc); 
			break; 
			
		default:
			break; 
		}
	}
	/* --------------------------------------------------------------------------------------------------------------------------
	 * AFTER DELAY, PLACE THE FEET DIRECTLY UNDER THE HEAD 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void placeFeet (int direction){			//print ("moving feet");
		switch (direction) {
			
		case (int) Dir.forward:						//print ("moving feet forward");
			rfeet.MovePosition(new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z));  
			break;
			
		case (int) Dir.right:						//print ("moving feet right");
			rfeet.MovePosition(new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z));  
			break;
			
		case (int) Dir.left:						//print ("moving feet left");
			rfeet.MovePosition(new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z));   
			break;
			
			// if player leans back, the feet will match the feet 
		case (int) Dir.back:						//print ("stopping feet under head");
			rfeet.MovePosition(new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z)); 
			break; 
			
		default:
			break; 
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * DELAY THE MOVEMENT OF THE FEET AFTER THE MOVEMENT OF THE HEAD
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private IEnumerator delayFeet (){				//print ("delaying");
		yield return new WaitForSeconds(delay);
		moveFeet (direction); 					//print ("4. moved feet"); 
		StartCoroutine (delayPlaceFeet ());
		//yield break; 
	}

	private IEnumerator delayPlaceFeet (){
		yield return new WaitForSeconds(delay);
		placeFeet (direction);
	}
}
