using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// MOVE CONTROLLER TILT TO READ WHERE PLAYER LEANS AND RESULTING MOVEMENT 

public class PlayerMovement : MonoBehaviour {
	
	public Vector3 mouse;		// current mouse position on screen
	public float delay; 		// time delay between feet movement and head movement 
	public float fallDelay; 	
	public float getupDelay; 
	public float hinc = 0.5f;	// force increment for head
	public float finc = 0.5f; 	// force increment for feet
	public float camInc = 0.5f; 

	public Rigidbody rhead;		// rigidbody at the head of the player
	public Rigidbody rfeet;		// rigidbody at the feet of the player
	public Camera cam; 			// to force the camera to just fall over if leaning too much
	public Camera fallCam; 
	public UniMoveController UniMove; // get UniMove

	private int halfWidth; 		// half the width of screen
	private int halfHeight; 	// half the height of screen

	private enum Dir { forward, right, left, back }; 
	public int direction; 
	private bool fallen;

	List<UniMoveController> moves = new List<UniMoveController>();

	void Start () {
		int count = UniMoveController.GetNumConnected();

		for (int i = 0; i < count; i++){
			UniMove = GetComponent<UniMoveController>();

			if (!UniMove.Init(i)) 
			{	
				Destroy(UniMove);	// If it failed to initialize, destroy and continue on
				continue;
			}
					
			// This example program only uses Bluetooth-connected controllers
			PSMoveConnectionType conn = UniMove.ConnectionType;
			if (conn == PSMoveConnectionType.Unknown || conn == PSMoveConnectionType.USB) 
			{
				Destroy(UniMove);
			}
			else 
			{
				moves.Add(UniMove);
				
				// Start all controllers with a white LED
				UniMove.SetLED(Color.white);
			}
		}

		halfWidth = Screen.width / 2; 
		halfHeight = Screen.height / 2; 

		fallen = false;
	}
	
	void Update () {

		/* --------------------------------------------------------------------------------------------------------------------------
		 * (1) MAKE THE KNOB GLOW A COLOUR DEPENDING ON WHICH BUTTON IS PRESSED
		 * (2) SET THE RUMBLE BASED ON TRIGGER
		 * -------------------------------------------------------------------------------------------------------------------------- */
		foreach (UniMoveController UniMove in moves) 
		{
			// Instead of this somewhat kludge-y check, we'd probably want to remove/destroy
			// the now-defunct controller in the disconnected event handler below.
			if (UniMove.Disconnected) continue;
			
			// Button events. Works like Unity's Input.GetButton
			// if (UniMove.GetButtonDown(PSMoveButton.Circle)){
			// 	Debug.Log("Circle Down");
			// }
			// if (UniMove.GetButtonUp(PSMoveButton.Circle)){
			// 	Debug.Log("Circle UP");
			// }
			
			// Change the colors of the LEDs based on which button has just been pressed:
			if (UniMove.GetButtonDown(PSMoveButton.Circle)) 		UniMove.SetLED(Color.cyan);
			else if(UniMove.GetButtonDown(PSMoveButton.Cross)) 		UniMove.SetLED(Color.red);
			else if(UniMove.GetButtonDown(PSMoveButton.Square)) 	UniMove.SetLED(Color.yellow);
			else if(UniMove.GetButtonDown(PSMoveButton.Triangle)) 	UniMove.SetLED(Color.magenta);
			else if(UniMove.GetButtonDown(PSMoveButton.Move)) 		UniMove.SetLED(Color.black);

			// Set the rumble based on how much the trigger is down
			UniMove.SetRumble(UniMove.Trigger);
		}
		
		// if the player has leaned too much, FALL AND LOSE
		if (fallen) {
			fallToLose();
		}
		else { //print ("0. got mouse position ");
			direction = getLeanDirection(); 		//print ("1. got direction");
			fallen = isLeaningTooMuch (); 			
			moveHead (direction); 					//print ("2. moved head"); 
			StartCoroutine(delayFeet ()); 			//print ("3. delayed feet");
		}
	}
	
	private int getLeanDirection(){	//print("entered get direction");
		
		if (UniMove.az <= -0.4f) {

			if (UniMove.ax > -0.3f && UniMove.ax < 0.3f) {
				return (int) Dir.back; 
			}
			else {
				if (UniMove.ax > 0.3f) {
					return (int) Dir.left; 
				}
				if (UniMove.ax < -0.3f){
					return (int) Dir.right; 
				}
			}
		}
		else {	
			if (UniMove.ax > -0.3f && UniMove.ax< 0.3f) {
				return (int) Dir.forward; 
			}
			else {
				if (UniMove.ax > 0.3f) {
					return (int) Dir.left; 
				}
				if (UniMove.ax < -0.3f){
					return (int) Dir.right; 
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
		
		// (2) if angle is at least 30
		if (angle >= 30.0f) { 	// print ("FALLEN!");
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
	 * (5) SWITCH TO LOSE SCREEN (different scene)
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void fallToLose(){	print ("YOU LOSE"); 
		
		// (1) switch cameras: from main to fallCam
		cam.enabled = false;
		fallCam.enabled = true; 
		
		// (2) play FALLING sounds
		
		// (3) animation? blink/blackout 
		
		// (4) play FALLEN TO FLOOR sound
		
		// (5) SWITCH TO LOSE SCREEN
		// Application.LoadLevel("nameofthescene"); 
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
