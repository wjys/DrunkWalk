using UnityEngine;
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
	private float to; 	// destination rotation
	private float cur;


	// get direction of lean 
	private enum Turn { left, right }; 
	public int direction; 

	// delay before checking rotation after rotated
	private bool rotating;  
	

	// Use this for initialization
	void Start () {
		rotating = false; 
	}
	
	// Update is called once per frame
	void Update () {
		if (!rotating){
			direction = getTurnDirection();
		}
		else {
			turnHead(direction);
		}
	}

	private int getTurnDirection(){

		if (player.useMouse){
			if (Input.GetMouseButtonDown(0)){	// left mouse button
				if (transform.eulerAngles.y > 270.0f || transform.eulerAngles.y < 90.0f){
					to = transform.eulerAngles.y - 45.0f; 
					cur = transform.eulerAngles.y;
				}
				rotating = true; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				return (int) Turn.left; 
			}
			if (Input.GetMouseButton(1)){	// right mouse button
				if (transform.eulerAngles.y > 270.0f || transform.eulerAngles.y < 90.0f){
					to = transform.eulerAngles.y + 45.0f;
					cur = transform.eulerAngles.y; 
				}
				rotating = true; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				return (int) Turn.right; 
			}
		}
		else {
			if (UniMove.gy <= boundRight){
				if (transform.eulerAngles.y > 270.0f || transform.eulerAngles.y < 90.0f){
					to = transform.eulerAngles.y + 45.0f;
					cur = transform.eulerAngles.y; 
				}
				rotating = true; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				return (int) Turn.right;
			}
			if (UniMove.gy >= boundLeft){
				if (transform.eulerAngles.y > 270.0f || transform.eulerAngles.y < 90.0f){
					to = transform.eulerAngles.y - 45.0f; 
					cur = transform.eulerAngles.y;
				}
				rotating = true; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				return (int) Turn.left;
			}
		}
		return (-1); 
	}

	private void turnHead (int direction) {
		transform.rotation = Quaternion.Euler (transform.eulerAngles.x, cur, transform.eulerAngles.z); 
		rhead.rotation = transform.rotation; 
		print ("rb " + rhead.rotation); 
		print (transform.rotation); 

		switch (direction){
		case (int) Turn.left: 
			cur -= speed; 
			if (cur <= to){
				rotating = false; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			}
			break;

		case (int) Turn.right:
			cur += speed; 
			if (cur >= to){
				rotating = false; 
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			}
			break;

		default:
			break; 
		}
	}
}
