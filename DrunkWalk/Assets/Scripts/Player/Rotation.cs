using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {

	// -------- COMPONENTS TO GET IN START() -------- 
	public UniMoveController UniMove;
	public DrunkMovement dm; 
	public DrunkForce df; 
	public Rigidbody rhead; 
	public GameObject feet; 
	public Collision coll;

	// -------- PRIVATE VARIABLES -------- 
	private enum controlInput { mouse, move, xbox }; 	// to know which controller we're using
	private enum Dir { forward, right, left, back }; 	// to modify drunkDir
	private enum Turn { left, right, idle }; 			// turn states
	private bool feetPlaced; 

	// -------- PUBLIC VARIABLES - TO SET IN INSPECTOR -------- 
	public int direction;

	public float camInc; 
	public float rotInc; 
	public float rotRate;

	public float minAngle;
	public float maxAngle; 
	public float speed; 
	
	public float boundLeft;		// unimove gy bound turning left
	public float boundRight; 	// unimove gy bound turning right

	public float tiltLeft;
	public float tiltRight;

	public int rotateDelay; 	// for delay after move rotation (in number of frames)
	public int currentFrame;
		
	// -------- PUBLIC BOOLS -------- 
	public bool rotated; 
	public bool rotating; 
	public bool delaying;
	public bool turnedLeft;	// to debug rot problem

	void Start () {
		UniMove = gameObject.GetComponent<UniMoveController>();
		dm 		= gameObject.GetComponent<DrunkMovement>();
		df 		= gameObject.GetComponent<DrunkForce>();
		rhead 	= gameObject.GetComponent<Rigidbody>();
		coll 	= gameObject.GetComponent<Collision>();

		if (dm.controller == (int)controlInput.mouse) {
			rotateDelay = 0; 
		}
		rotating = false; 
		delaying = false; 
		feetPlaced = false;
		rotated = false; 
		direction = (int) Turn.idle; 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * (1) if the player isn't rotating, then get the direction of the turn (if applicable)
	 * (2) if the player IS rotating but we're not resetting the move (delay), then turn the player
	 * (3) if we ARE resetting the move (delay), then delay
	 * -------------------------------------------------------------------------------------------------------------------------- */


	void Update () {
			
		if (coll.colliding){
			direction = (int) Turn.idle;
		}
		if (dm.fallen){
			direction = (int) Turn.idle;
		}

		feet = dm.pfeet;

		if (!coll.colliding){
			if (!dm.fallen){		// player is still standing
				if (!delaying){		// player hasn't already turned recently
					direction = turnHead(direction);
				}
				else {
					//delayRotation (); 
					StartCoroutine(delayRot ());
				}
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
			currentFrame = 0;
			delaying = false; 
			feetPlaced = false;
			rotated = false; 
		}
		currentFrame++;
	}

	IEnumerator delayRot(){
		yield return new WaitForSeconds (1.0f);
		currentFrame = 0;
		delaying = false; 
		feetPlaced = false;
		rotated = false; 
	}

	// NOT USED? PLACE FEET BEFORE ROTATION
	private void placeFeet (){	
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
					transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - rotInc*rotRate, transform.rotation.z, transform.rotation.w); 
					if (rotRate >= 1){
						rotRate = 1;
					}
					else rotRate += 0.1f;
					return (int) Turn.left; 
				}	
			}
			if (dm.direction == (int) Dir.right){
				if (Input.GetMouseButton(1)){	// right mouse button
					rotated = true; 
					transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + rotInc*rotRate, transform.rotation.z, transform.rotation.w);  
					if (rotRate >= 1){
						rotRate = 1;
					}
					else rotRate += 0.1f;
					return (int) Turn.right; 
				}
			}
			return (int) Turn.idle; 
		}
		else if (dm.controller == (int) controlInput.move){
			if (current == (int) Turn.idle){
				rotRate = 0;
				//print ("idle"); 
				if (UniMove.ax >= tiltLeft){
					if (UniMove.gy >= boundLeft){
						//print ("start turning left"); 
						//gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
						rotated = true; 
						return (int) Turn.left;
					}
				}
				if (UniMove.ax <= tiltRight){
					if (UniMove.gy <= boundRight){
						//print ("start turning right"); 
						//gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
						rotated = true; 
						return (int) Turn.right; 
					}
				}
				return (int) Turn.idle; 
			}
			if (current == (int) Turn.left){
				if ((UniMove.gy < boundRight && rotated)){
					//print ("stop turning left"); 
					delaying = true; 
					return (int) Turn.idle;
				}
				else {
					//print ("turning left"); 
					transform.localRotation = new Quaternion (transform.localRotation.x, transform.localRotation.y - rotInc*rotRate, transform.localRotation.z, transform.localRotation.w); 
					if (rotRate >= 0.8f){
						rotRate = 0.8f;
					}
					else rotRate += 0.05f;
					return (int) Turn.left; 
				}
			}
			if (current == (int) Turn.right){
				if ((UniMove.gy > boundLeft && rotated)){
					//print ("stop turning right"); 
					delaying = true; 
					return (int) Turn.idle;
				}
				else {
					//print ("turning right"); 
					transform.localRotation = new Quaternion (transform.localRotation.x, transform.localRotation.y + rotInc*rotRate, transform.localRotation.z, transform.localRotation.w); 
					if (rotRate >= 0.8f){
						rotRate = 0.8f;
					}
					else rotRate += 0.05f;
					return (int) Turn.right;
				}
			}
			return (int) Turn.idle; 
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
			return (int) Turn.idle; 
		}
		return (int)Turn.idle; 
	}
}
