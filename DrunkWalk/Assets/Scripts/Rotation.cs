﻿using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
	
	public UniMoveController UniMove;
	public float camInc; 

	// MIN/MAX ANGLES
	public float minAngle;
	public float maxAngle; 
	public float speed; 

	// UNIMOVE DETECTION BOUNDS FOR gy
	public float boundLeft;
	public float boundRight; 

	// SHOULD I SET UP ITS OWN BOOL HERE TO CHECK FOR MOUSE VS MOVE INPUT OR JUST CHECK IN DRUNKMOVEMENT?
	public DrunkMovement player; 
	public Rigidbody rhead; 
	public float to; 	// destination rotation
	public float cur;


	// get direction of lean 
	private enum Turn { left, right }; 
	public int direction; 

	// delay before checking rotation after rotated
	public bool rotating;  

	// FOR DELAY AFTER MOVE ROTATION
	public int rotateDelay; 
	private int currentFrame;
	public bool delaying; 
		

	void Start () {
		rotating = false; 
		delaying = false; 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * (1) if the player isn't rotating, then get the direction of the turn (if applicable)
	 * (2) if the player IS rotating but we're not resetting the move (delay), then turn the player
	 * (3) if we ARE resetting the move (delay), then delay
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void Update () {
		if (!rotating){
			direction = getTurnDirection();
		}
		else if (!delaying) {
			turnHead(direction);
		}
		else if (delaying){
			delayRotation (); 
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * if waited rotateDelay number of frames, 
	 * (1) reset currentFrame to 0
	 * (2) no longer delaying and no longer rotating
	 * otherwise, add to currentFrame with every frame 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void delayRotation(){
		if (currentFrame >= rotateDelay) {
			currentFrame = 0;
			delaying = false; 
			rotating = false; 
		}
		currentFrame++; 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. RETURN: int = ENUM INDEX OF THE DIRECTION OF ROTATION (right or left) 
	 * if using the mouse, 
	 * (1) left-mouse click 
	 * (2) right-mouse click
	 * if using the move,
	 * (1) 
	 * (2) 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private int getTurnDirection(){

		float y = transform.eulerAngles.y;

		if (player.useMouse){
			if (Input.GetMouseButtonDown(0)){	// left mouse button

				if ((y > -5 && y < 5) || (y > 355)){
					to = -45.0f; 
				}
				else if ((y >-50 && y < -40) || (y > 310 && y < 320)){
					to = -90.0f; 
				}
				else if (y > 85 && y < 95){
					to = 0.0f; 
				}
				else if (y > 40 && y < 50){
					to = 45.0f; 
				}
				else {
					return (-1); 
				}
				rotating = true; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				return (int) Turn.left; 
			}
			if (Input.GetMouseButton(1)){	// right mouse button
				if ((y > -5 && y < 5) || (y > 355)){
					to = 45.0f; 
				}
				else if (y > 40 && y < 50){
					to = 90.0f; 
				}
				else if ((y > -95 && y < -85) || (y > 265 && y < 275)){
					to = -45.0f; 
				}
				else if ((y > -50 && y < -40) || (y > 310 && y < 320)){
					to = 0.0f; 
				}
				else {
					return (-1); 
				}
				rotating = true; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				return (int) Turn.right; 
			}
		}
		else {
			if (UniMove.gy <= boundRight){
				if ((y > -5 && y < 5) || (y > 355)){
					to = -45.0f; 
				}
				else if ((y >-50 && y < -40) || (y > 310 && y < 320)){
					to = -90.0f; 
				}
				else if (y > 85 && y < 95){
					to = 0.0f; 
				}
				else if (y > 40 && y < 50){
					to = 45.0f; 
				}
				else {
					return (-1); 
				}
				rotating = true; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				return (int) Turn.right;
			}
			if (UniMove.gy >= boundLeft){
				if ((y > -5 && y < 5) || (y > 355)){
					to = 45.0f; 
				}
				else if (y > 40 && y < 50){
					to = 90.0f; 
				}
				else if ((y > -95 && y < -85) || (y > 265 && y < 275)){
					to = -45.0f; 
				}
				else if ((y > -50 && y < -40) || (y > 310 && y < 320)){
					to = 0.0f; 
				}
				else {
					return (-1); 
				}
				rotating = true; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				return (int) Turn.left;
			}
		}
		return (-1); 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * ARG: int = current direction index (enum) of rotation. NO RETURN
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void turnHead (int direction) {
		transform.rotation = Quaternion.Euler (transform.eulerAngles.x, cur, transform.eulerAngles.z); 
		rhead.rotation = transform.rotation; 
		//print ("rb " + rhead.rotation); 
		//print (transform.rotation); 

		switch (direction){
		case (int) Turn.left: 
			cur -= speed; 
			if (cur <= to){
				if (player.useMouse){
					rotating = false; 
				}
				else {
					delayRotation();
				}
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			}
			break;

		case (int) Turn.right:
			cur += speed; 
			if (cur >= to){
				if (player.useMouse){
					rotating = false; 
				}
				else {
					delayRotation();
				}
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			}
			break;

		default:
			break; 
		}
	}
}
