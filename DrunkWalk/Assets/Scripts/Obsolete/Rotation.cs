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


	// get direction of lean 
	private enum Turn { left, right }; 
	public int direction; 

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
		rotating = false; 
		delaying = false; 
		feetPlaced = false;
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
			if (!rotating){
				if (!feetPlaced){
					placeFeet (); 
				}
				df.enabled = false;
				rhead.constraints = RigidbodyConstraints.FreezeAll;
				direction = getTurnDirection();
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
			currentFrame = 0;
			rotating = false; 
		}
		currentFrame++;
	}

	private void placeFeet (){			//print ("moving feet");
		// rfeet.MovePosition(Vector3.Lerp(rfeet.position, transform.position, smooth * Time.frameCount));
		feet.transform.position = new Vector3 (rhead.position.x, feet.transform.position.y, rhead.position.z); 
		feetPlaced = true; 
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

		if (dm.direction == (int) Dir.left || dm.direction == (int) Dir.right){
			if (dm.controller == (int) controlInput.mouse){
				if (dm.direction == (int) Dir.left){
					if (Input.GetMouseButton(0)){	// left mouse button
						rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
						transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - rotInc, transform.rotation.z, transform.rotation.w); 
						rotating = true; 
						return (int) Turn.left; 
					}	
				}
				if (dm.direction == (int) Dir.right){
					if (Input.GetMouseButton(1)){	// right mouse button
						rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
						transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + rotInc, transform.rotation.z, transform.rotation.w); 
						rotating = true; 
						return (int) Turn.right; 
					}
				}
				return (-1); 
			}
			else if (dm.controller == (int) controlInput.move){
				if (UniMove.gy >= boundLeft){
					rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
					transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - rotInc, transform.rotation.z, transform.rotation.w); 
					rotating = true; 
					return (int) Turn.left;
				}
				if (dm.direction == (int) Dir.right){
					if (UniMove.gy <= boundRight){
						rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
						transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + rotInc, transform.rotation.z, transform.rotation.w); 
						rotating = true; 
						return (int) Turn.right; 
					}
				}
			}
			else if (dm.controller == (int) controlInput.xbox){
				if (Input.GetAxis("RightStickX") < -0.9f){
					rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
					transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - rotInc, transform.rotation.z, transform.rotation.w); 
					rotating = true; 
					return (int) Turn.left;
				}
				if (Input.GetAxis("RightStickX") > 0.9f){
					rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
					transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + rotInc, transform.rotation.z, transform.rotation.w); 
					rotating = true; 
					return (int) Turn.right; 
				}
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
			turnedLeft = true;
			cur -= speed; 
			if (cur <= to){
				if (dm.controller == (int) controlInput.move){
					delaying = true; 
				}
				else {
					rotating = false; 
				}
				feetPlaced = false;
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
				df.enabled = true; 
			}
			break;

		case (int) Turn.right:
			turnedLeft = false;
			cur += speed; 
			if (cur >= to){
				if (dm.controller == (int) controlInput.move){
					delaying = true; 
				}
				else {
					rotating = false; 
				}
				feetPlaced = false; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
				df.enabled = true;
			}
			break;

		default:
			break; 
		}
	}
}
