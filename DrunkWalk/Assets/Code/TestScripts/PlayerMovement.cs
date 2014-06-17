using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// MOUSE POSITION TO READ WHERE PLAYER LEANS AND RESULTING MOVEMENT 

public class PlayerMovement : MonoBehaviour {
	
	public Vector3 mouse;		// current mouse position on screen
	public float delay; 		// time delay between feet movement and head movement 
	public float hinc = 0.5f;	// force increment for head
	public float finc = 0.5f; 	// force increment for feet

	public Rigidbody rhead;		// rigidbody at the head of the player
	public Rigidbody rfeet;		// rigidbody at the feet of the player
	public ConstantForce fhead; // constant force acting on head of the player 
	public ConstantForce ffeet;	// cst force on feet of player
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

		foreach (UniMoveController UniMove in moves) 
		{
			// Instead of this somewhat kludge-y check, we'd probably want to remove/destroy
			// the now-defunct controller in the disconnected event handler below.
			if (UniMove.Disconnected) continue;
			
			// Button events. Works like Unity's Input.GetButton
			if (UniMove.GetButtonDown(PSMoveButton.Circle)){
				Debug.Log("Circle Down");
			}
			if (UniMove.GetButtonUp(PSMoveButton.Circle)){
				Debug.Log("Circle UP");
			}
			
			// Change the colors of the LEDs based on which button has just been pressed:
			if (UniMove.GetButtonDown(PSMoveButton.Circle)) 		UniMove.SetLED(Color.cyan);
			else if(UniMove.GetButtonDown(PSMoveButton.Cross)) 	UniMove.SetLED(Color.red);
			else if(UniMove.GetButtonDown(PSMoveButton.Square)) 	UniMove.SetLED(Color.yellow);
			else if(UniMove.GetButtonDown(PSMoveButton.Triangle)) 	UniMove.SetLED(Color.magenta);
			else if(UniMove.GetButtonDown(PSMoveButton.Move)) 		UniMove.SetLED(Color.black);

			// Set the rumble based on how much the trigger is down
			UniMove.SetRumble(UniMove.Trigger);
		}

		// get the current mouse position
		//mouse = Input.mousePosition; 


		if (isLeaningTooMuch()) {
			// FALL = rotate camera down
		}
		//else { //print ("0. got mouse position ");
			direction = getLeanDirection (); 	//print ("1. got direction");
			moveHead (direction); 					//print ("2. moved head"); 
			StartCoroutine(delayFeet ()); 			//print ("3. delayed feet");
			moveFeet (direction); 					//print ("4. moved feet"); 
		//}
	}

	private int getLeanDirection(){	//Vector3 mouse){	//print("entered get direction");
		
		//if (mouse.y < halfHeight) {	// if mouse in lower half of screen, leaning back 
		if (UniMove.az <= -0.4f) {

			if (UniMove.ax > -0.3f && UniMove.ax < 0.3) {
			//if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){ 
				return (int) Dir.back; 
			}
			else {
				if (UniMove.ax > 0.3f) {
				//if (mouse.x >= halfWidth){		// print ("leaning right");
					return (int) Dir.left; 
				}
				if (UniMove.ax < -0.3f){
				//else {							// print ("leaning left");
					return (int) Dir.right; 
				}
			}
		}
		else {	// if mouse is in top half, check right/left lean as well
			if (UniMove.ax > -0.3f && UniMove.ax< 0.3) {
			//if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){	// print ("leaning forward");
				return (int) Dir.forward; 
			}
			else {
				if (UniMove.ax > 0.3f) {
				//if (mouse.x >= halfWidth){ 	// print ("leaning right");
					return (int) Dir.left; 
				}
				if (UniMove.ax < -0.3f){
				//else {						// print ("leaning left");
					return (int) Dir.right; 
				}
			}
		}
		return (0);
	}

	// !! NB: FOR NOW IF LEAN BACK, STOP PLAYER

	private bool isLeaningTooMuch(){ //print ("checking lean");
		Vector3 vertVec = new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z); 
		float angle = Vector3.Angle (vertVec, rhead.position); 
		if (angle >= 30.0f) { 	// print ("FALLEN!");
			fallen = true; 
			return true;
		} 						// print ("STILL STANDING");
		return false; 
	}


	// depending on the direction of the lean, set a constantforce on the rigidbody of the head 
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
			rhead.AddForce (0, 0, 0); 
			break; 
			
		default:
			break; 
		}
	}


	// depending on the direction of the lean, set a constantforce on the rigidbody of the feet 
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
			ffeet.force.Set (0, 0, 0); 

			rfeet.position = new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z); 
			break; 
			
		default:
			break; 
		}
	}

	// delay the movement of the feet after the movement of the head 
	private IEnumerator delayFeet (){				//print ("delaying");
		yield return new WaitForSeconds(delay);
		//yield break; 
	}
}
