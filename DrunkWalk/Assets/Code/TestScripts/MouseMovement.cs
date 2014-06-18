﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// MOUSE POSITION TO READ WHERE PLAYER LEANS AND RESULTING MOVEMENT 

public class MouseMovement : MonoBehaviour {
	
	public Vector3 mouse;		// current mouse position on screen
	public float delay; 		// time delay between feet movement and head movement 
	public float fallDelay; 	// the amount of time the player gets as falling to get back up (without losing points)
	public float getupDelay; 
	public float hinc = 0.5f;	// force increment for head
	public float finc = 0.5f; 	// force increment for feet
	public float camInc = 0.5f; 

	// OBJECTS TO DRAG INTO COMPONENT
	public Rigidbody rhead;		// rigidbody at the head of the player
	public Rigidbody rfeet;		// rigidbody at the feet of the player
	public Camera cam; 			// to force the camera to just fall over if leaning too much
	public Camera fallCam; 
	public Collision collScript; 
	//public UniMoveController UniMove; // get UniMove
	
	private int halfWidth; 		// half the width of screen
	private int halfHeight; 	// half the height of screen
	
	private enum Dir { forward, right, left, back }; 
	public int direction; 
	private float angleBetween;
	private bool falling; 
	private bool fallen;
	private int fallCt; 		// NOT IMPLEMENTED YET - FALL 3 TIMES TO LOSE. FOR NOW, FALL ONCE = LOSE
	
	// List<UniMoveController> moves = new List<UniMoveController>();
	
	void Start () {
		halfWidth = Screen.width / 2; 
		halfHeight = Screen.height / 2; 
		
		fallen = false;
		falling = false;
		fallCt = 0; 
	}
	
	void Update () {
		// get the current mouse position
		mouse = Input.mousePosition; 

		if (falling) {
			print ("LOSER");
			// (1) switch cameras
			cam.enabled = false;
			fallCam.enabled = true; 

			/*
			// (2) play falling sounds

			// (3) hit ground
			//collScript.score -= 500;
			//print ("Floor Collision " + collScript.score); 

			// (4) check button
			if (Input.GetMouseButtonDown(0)){
				rhead.MovePosition (new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z)); 
				fallCam.enabled = false;
				cam.enabled = true; 
				// play get back up sounds
				falling = false; 
			}
			else {
				if (fallCt < 100) {
					fallCt++; 
				}
				else {
					// BLACK OUT
					// LOSE THE GAME
					// PLAY LOSER SOUNDS
					rhead.MovePosition (new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z)); 
					print ("LOSER"); 
					//Application.LoadLevel(Application.loadedLevel); 
					falling = false; 
				}
			}
			*/
		}
		else { //print ("0. got mouse position ");
			//falling = isLeaningTooMuch(); 
			direction = getLeanDirection (mouse); 	//print ("1. got direction");
			moveHead (direction); 					//print ("2. moved head"); 
			StartCoroutine(delayFeet ()); 			//print ("3. delayed feet");
		}
	}
	
	private int getLeanDirection(Vector3 mouse){	//print("entered get direction");
		
		if (mouse.y < halfHeight) {	// if mouse in lower half of screen, leaning back 
			if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){ 
				return (int) Dir.back; 
			}
			else {
				if (mouse.x >= halfWidth){		// print ("leaning right");
					return (int) Dir.right; 
				}
				else {							// print ("leaning left");
					return (int) Dir.left; 
				}
			}
		}
		else {	// if mouse is in top half, check right/left lean as well
			if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){	// print ("leaning forward");
				return (int) Dir.forward; 
			}
			else {
				if (mouse.x >= halfWidth){ 	// print ("leaning right");
					return (int) Dir.right; 
				}
				else {						// print ("leaning left");
					return (int) Dir.left; 
				}
			}
		}
	}
	
	// !! NB: FOR NOW IF LEAN BACK, STOP PLAYER
	
	private bool isLeaningTooMuch(){ print ("checking lean");
		Vector3 vertVec = new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z); 
		angleBetween = Vector3.Angle (vertVec, rhead.position); 
		if (angleBetween >= 30.0f) { 	// print ("FALLEN!");
			falling = true; 
			print ("SWEET SPOT"); 
			falling = true; 
			return true;
		} 						// print ("STILL STANDING");
		falling = false; 
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
			rhead.AddForce (0, 0, -hinc); 
			rhead.MovePosition(new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z)); 
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
			rfeet.AddForce (0, 0, -finc); 
			rfeet.MovePosition (new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z)); 
			break; 
			
		default:
			break; 
		}
	}
	
	// delay the movement of the feet after the movement of the head 
	private IEnumerator delayFeet (){				//print ("delaying");
		yield return new WaitForSeconds(delay);
		moveFeet (direction); 					//print ("4. moved feet"); 
		//yield break; 
	}

	private IEnumerator isFalling(){
		yield return new WaitForSeconds(fallDelay);

	}

	private IEnumerator isGettingUp(){
		yield return new WaitForSeconds(getupDelay);
		// PLAY ANIMATION
	}
}
