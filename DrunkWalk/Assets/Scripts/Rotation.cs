using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
	
	public UniMoveController UniMove;
	public Camera cam; 
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
		return (0);
	}

	private void turnHead (int direction){
		switch (direction){

		case (int) Turn.left:
			//Debug.Log("Turning Left?");

			cam.transform.rotation = new Quaternion (cam.transform.rotation.x, cam.transform.rotation.y - camInc, cam.transform.rotation.z, cam.transform.rotation.w); 
			break;

		case (int) Turn.right:
			//Debug.Log("Turning Right?");

			cam.transform.rotation = new Quaternion (cam.transform.rotation.x, cam.transform.rotation.y + camInc, cam.transform.rotation.z, cam.transform.rotation.w); 
			break;

		default:
			break;
		}
	}
}
