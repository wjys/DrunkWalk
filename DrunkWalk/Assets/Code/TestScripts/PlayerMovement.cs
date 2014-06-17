using UnityEngine;
using System.Collections;

// MOUSE POSITION TO READ WHERE PLAYER LEANS AND RESULTING MOVEMENT 

public class PlayerMovement : MonoBehaviour {
	
	public Vector3 mouse;		// current mouse position on screen
	public float delay; 		// time delay between feet movement and head movement 
	public float hinc;			// force increment for head
	public float finc; 			// force increment for feet

	public Rigidbody rhead;		// rigidbody at the head of the player
	public Rigidbody rfeet;		// rigidbody at the feet of the player
	public ConstantForce fhead; // constant force acting on head of the player 
	public ConstantForce ffeet;	// cst force on feet of player 

	private int halfWidth; 		// half the width of screen
	private int halfHeight; 	// half the height of screen

	private enum Dir { up, right, left, back }; 
	private int direction; 
	private bool fallen;

	void Start () {
		halfWidth = Screen.width / 2; 
		halfHeight = Screen.height / 2; 

		//rhead = GetComponent<Rigidbody> ();
		//rfeet = GetComponent<Rigidbody> ();
		//fhead = GetComponentInChildren<ConstantForce> ();
		//ffeet = GetComponentInChildren<ConstantForce> ();

		fallen = false;
	}
	
	void Update () {
		// get the current mouse position
		mouse = Input.mousePosition; 

		if (isLeaningTooMuch()) {
			// FALL 
		}
		//else {
			direction = getLeanDirection (mouse); 
			moveHead (direction);
			StartCoroutine(delayFeet ()); 
			moveFeet (direction); 
		//}
	}

	private int getLeanDirection(Vector2 mouse){
		// if leaning right 
		if (mouse.x >= halfWidth) {
			Debug.Log("MOUSE????");
			// check if lean forward
			if (mouse.y >= halfHeight){
				// check if right lean is more 'important' than lean forward
				if (Mathf.Abs(mouse.x - halfWidth) >= Mathf.Abs(mouse.y - halfHeight)){
					return (int) Dir.right; 
				}
				// backwards lean overrides the side leans 
				else {
					return (int) Dir.up; 
				}
			}
			else { // lean back 
				return (int) Dir.back; 
			}
		}
		// if leaning left
		else {
			// check if lean forward
			if (mouse.y >= halfHeight){
				// check if right lean is more 'important' than lean forward
				if (Mathf.Abs(mouse.x - halfWidth) >= Mathf.Abs(mouse.y - halfHeight)){
					return (int) Dir.left; 
				}
				// forwards lean overrides the side leans 
				else {
					return(int)  Dir.up; 
				}
			}
			else { // lean back 
				return (int) Dir.back; 
			}
		}
	}

	// !! NB: FOR NOW IF LEAN BACK, STOP PLAYER

	private bool isLeaningTooMuch(){
		Vector3 vertVec = new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z); 
		float angle = Vector3.Angle (vertVec, rhead.position); 
		if (angle >= 30.0f) {
			fallen = true; 
			return true;
		}
		return false; 
	}


	// depending on the direction of the lean, set a constantforce on the rigidbody of the head 
	private void moveHead (int direction){
		switch (direction) {
			
		case (int) Dir.up:
			fhead.force.Set (0, 0, hinc);  
			break;
			
		case (int) Dir.right:
			fhead.force.Set (hinc, 0, 0); 
			break;
			
		case (int) Dir.left:
			fhead.force.Set (-hinc, 0, 0); 
			break;
			
		case (int) Dir.back:
			fhead.force.Set (0, 0, 0); 
			break; 
			
		default:
			break; 
		}
	}


	// depending on the direction of the lean, set a constantforce on the rigidbody of the feet 
	private void moveFeet (int direction){
		switch (direction) {
			
		case (int) Dir.up:
			ffeet.force.Set (0, 0, finc); 
			break;
			
		case (int) Dir.right:
			ffeet.force.Set (finc, 0, 0); 
			break;
			
		case (int) Dir.left:
			ffeet.force.Set (-finc, 0, 0);  
			break;

		// if player leans back, the feet will match the feet 
		case (int) Dir.back:
			ffeet.force.Set (0, 0, 0); 
			rfeet.position = new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z); 
			break; 
			
		default:
			break; 
		}
	}

	// delay the movement of the feet after the movement of the head 
	private IEnumerator delayFeet (){
		yield return new WaitForSeconds(delay);
		//yield break; 
	}
}
