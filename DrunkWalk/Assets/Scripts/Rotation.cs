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


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		direction = getTurnDirection();
		turnHead (direction);

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

	private void turnHead (int direction){
		switch (direction){

		case (int) Turn.left:
			//Debug.Log("Turning Left?");

			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y - camInc, transform.rotation.z, transform.rotation.w); 
			break;

		case (int) Turn.right:
			//Debug.Log("Turning Right?");

			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y + camInc, transform.rotation.z, transform.rotation.w); 
			break;

		default:
			break;
		}
	}
}
