using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
	
	public UniMoveController UniMove;
	public float camInc; 
	public float boundLeft;
	public float boundRight; 

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
		StartCoroutine(rotateHead (direction)); 
		/*
		direction = getTurnDirection();
		turnHead (direction);

		if (rotating) {
			currentFrame++; 

			if (currentFrame >= rotateDelay){
				rotating = false; 
				currentFrame = 0; 
			}
		}
		*/
		//Debug.Log(UniMove.gy);


		
	}

	private int getTurnDirection(){
		if (UniMove.gy <= boundRight){
			return (int) Turn.right;
		}
		if (UniMove.gy >= boundLeft){
			return (int) Turn.left;
		}
		return (-1);
	}

	private IEnumerator rotateHead (int direction) {

		switch (direction){
			
		case (int) Turn.left:
			//Debug.Log("Turning Left?");
			
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - camInc, transform.rotation.z, transform.rotation.w); 

			WaitForSeconds(2); 
			break;
			
		case (int) Turn.right:
			//Debug.Log("Turning Right?");
			
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + camInc, transform.rotation.z, transform.rotation.w); 
			WaitForSeconds(2); 
			break;
			
		default:	// if not turning read gy
			direction = getTurnDirection (); 
			break;
		}
	}

	private void turnHead (int direction){
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
