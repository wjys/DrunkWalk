using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
	
	public UniMoveController UniMove;
	public float camInc; 

	// MIN/MAX ANGLES
	public float minAngle;
	public float maxAngle; 

	// UNIMOVE DETECTION BOUNDS FOR gy
	public float boundLeft;
	public float boundRight; 

	// SHOULD I SET UP ITS OWN BOOL HERE TO CHECK FOR MOUSE VS MOVE INPUT OR JUST CHECK IN DRUNKMOVEMENT?
	public DrunkMovement player; 


	// get direction of lean 
	private enum Turn { left, right }; 
	public int direction; 

	// delay before checking rotation after rotated
	private bool rotating; 
	public float rotateDelay; 
	public float currentFrame; 

	// delay while rotating
	public float whileDelay;
	
	// Use this for initialization
	void Start () {
		rotating = false; 
		currentFrame = 0; 
	}
	
	// Update is called once per frame
	void Update () {
		direction = getTurnDirection();
		turnHead(direction); 
	}

	private int getTurnDirection(){
		if (player.useMouse){
			if (Input.GetMouseButtonDown(0)){	// left mouse button
				return (int) Turn.left; 
			}
			else if (Input.GetMouseButton(1)){	// right mouse button
				return (int) Turn.left; 
			}
		}
		else {
			if (UniMove.gy <= boundRight){
				return (int) Turn.right;
			}
			if (UniMove.gy >= boundLeft){
				return (int) Turn.left;
			}
		}
		return (-1);
	}

	private void turnHead (int direction) {
		switch (direction){
		case (int) Turn.left:
			if (transform.eulerAngles.y > 270.0f || transform.eulerAngles.y < 90.0f){
				print ("turning left"); 
				float to = transform.eulerAngles.y - 45.0f;
				transform.eulerAngles = new Vector3 (0, Mathf.LerpAngle (transform.eulerAngles.y, to, Time.time));
			}
			break;

		case (int) Turn.right:
			if (transform.eulerAngles.y < 90.0f || transform.eulerAngles.y > 270.0f){
				print ("turning right"); 
				float to = transform.eulerAngles.y + 45.0f;
				transform.eulerAngles = new Vector3 (0, Mathf.LerpAngle (transform.eulerAngles.y, to, Time.time));
			}
			break;

		default:
			break; 
		}
	}


	private IEnumerator rotateHead (int direction) {

		switch (direction){
			
		case (int) Turn.left:
			//Debug.Log("Turning Left?");
			
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - camInc, transform.rotation.z, transform.rotation.w); 

			yield return new WaitForSeconds(2); 
			break;
			
		case (int) Turn.right:
			//Debug.Log("Turning Right?");
			
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + camInc, transform.rotation.z, transform.rotation.w); 
			yield return new WaitForSeconds(2); 
			break;
			
		default:	// if not turning read gy
			direction = getTurnDirection (); 
			break;
		}
	}

	private void rotHead (int direction){
		if (!rotating){
			switch (direction){

			case (int) Turn.left:
				//Debug.Log("Turning Left?");

				transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - camInc, transform.rotation.z, transform.rotation.w); 
				if (currentFrame >= whileDelay){
					rotating = true;
					currentFrame = 0; 
				}
				else {
					currentFrame++; 
				}
				break;

			case (int) Turn.right:
				//Debug.Log("Turning Right?");

				transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + camInc, transform.rotation.z, transform.rotation.w); 
				if (currentFrame >= whileDelay){
					rotating = true;
					currentFrame = 0; 
				}
				else {
					currentFrame++; 
				}
				break;

			default:
				break;
			}

		}
	}
}
