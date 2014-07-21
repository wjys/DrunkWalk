using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
	
	public UniMoveController UniMove;
	public DrunkForce df; 
	public float camInc; 
	private enum controlInput { mouse, move, xbox }; 

	// MIN/MAX ANGLES
	public float minAngle;
	public float maxAngle; 
	public float speed; 

	// UNIMOVE DETECTION BOUNDS FOR gy
	public float boundLeft;
	public float boundRight; 

	// SHOULD I SET UP ITS OWN BOOL HERE TO CHECK FOR MOUSE VS MOVE INPUT OR JUST CHECK IN DRUNKMOVEMENT?
	public DrunkMovement dm; 
	private enum Dir { forward, right, left, back }; // to modify drunkDir
	public Rigidbody rhead; 
	public GameObject feet; 
	public float to; 	// destination rotation
	public float cur;

	public float rotInc; 
	public bool rotated; 


	// get direction of lean 
	private enum Turn { left, right, idle }; 
	public int direction; 
	public int previous; 

	// delay before checking rotation after rotated
	public bool rotating;  
	public bool delaying;
	private bool feetPlaced; 

	// FOR DELAY AFTER MOVE ROTATION
	public int rotateDelay; 
	public int currentFrame;

	//DEBUG ROT PROBLEM
	public bool turnedLeft = false;
		

	void Start () {
		if (dm.controller == (int)controlInput.mouse) {
			rotateDelay = 0; 
		}
		rotating = false; 
		delaying = false; 
		feetPlaced = false;
		rotated = false; 
		direction = (int) Turn.idle; 
		previous = (int) Turn.idle; 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * (1) if the player isn't rotating, then get the direction of the turn (if applicable)
	 * (2) if the player IS rotating but we're not resetting the move (delay), then turn the player
	 * (3) if we ARE resetting the move (delay), then delay
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void Update () {

		// PLAYER HASN'T FALLEN
		if (!dm.fallen){
			// PLAYER NOT ROTATING => GET DIRECTION
			if (!delaying){
				direction = turnHead(direction);
			}
			else {
				delayRotation (); 
			}
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
			//rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ; 
			currentFrame = 0;
			delaying = false; 
			feetPlaced = false;
			rotated = false; 
		}
		currentFrame++;
	}

	private void placeFeet (){			//print ("moving feet");
		// rfeet.MovePosition(Vector3.Lerp(rfeet.position, transform.position, smooth * Time.frameCount));
		feet.transform.position = new Vector3 (rhead.position.x, feet.transform.position.y, rhead.position.z); 
		feetPlaced = true; 
		/*
		if (!feetPlaced){
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			placeFeet (); 
		}*/
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

	private int turnHead(int current){

		float y = transform.eulerAngles.y;

		if (dm.controller == (int) controlInput.mouse){
			if (dm.direction == (int) Dir.left){
				if (Input.GetMouseButton(0)){	// left mouse button
					rotated = true; 
					transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - rotInc, transform.rotation.z, transform.rotation.w); 
					return (int) Turn.left; 
				}	
				else if (rotated)
					delaying = true; 
			}
			if (dm.direction == (int) Dir.right){
				if (Input.GetMouseButton(1)){	// right mouse button
					rotated = true; 
					transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + rotInc, transform.rotation.z, transform.rotation.w);  
					return (int) Turn.right; 
				}
			}
			return (-1); 
		}
		else if (dm.controller == (int) controlInput.move){
			if (current == (int) Turn.idle){
				print ("idle"); 
				if (dm.direction == (int) Dir.left){
					if (UniMove.gy >= boundLeft){
						print ("start turning left"); 
						rotated = true; 
						return (int) Turn.left;
					}
				}
				if (dm.direction == (int) Dir.right){
					if (UniMove.gy <= boundRight){
						print ("start turning right"); 
						rotated = true; 
						return (int) Turn.right; 
					}
				}
			}
			if (current == (int) Turn.left){
				/*if (UniMove.gy < boundRight && rotated){
					print ("stop turning left"); 
					delaying = true; 
					return (int) Turn.idle;
				}
				else {*/
					print ("turning left"); 
					transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - rotInc, transform.rotation.z, transform.rotation.w); 
				//}
			}
			if (current == (int) Turn.right){
				/*if (UniMove.gy > boundLeft && rotated){
					print ("stop turning right"); 
					delaying = true; 
					return (int) Turn.idle;
				}
				else {*/
					print ("turning right"); 
					transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + rotInc, transform.rotation.z, transform.rotation.w); 
				//}
			}
		}
		else if (dm.controller == (int) controlInput.xbox){
			if (Input.GetAxis("RightStickX") < -0.9f){
				print ("turning LEFT"); 
				//rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY; 
				transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - rotInc, transform.rotation.z, transform.rotation.w); 
				return (int) Turn.left;
			}
			if (Input.GetAxis("RightStickX") > 0.9f){
				print ("turning RIGHT");
				//rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY; 
				transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + rotInc, transform.rotation.z, transform.rotation.w);
				return (int) Turn.right; 
			}
		}
		return (-1); 
	}
}
