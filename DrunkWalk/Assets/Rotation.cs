using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
	public UniMoveController UniMove;

	private enum Turn { left, right }; 
	public int direction; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		direction = getTurnDirection();
		turnHead (direction);

		Debug.Log(UniMove.gy);
		
	}

	private int getTurnDirection(){
		if (UniMove.gy <= 0.1f){
			return (int) Turn.left;
		}
		if (UniMove.gy >= 0.1f){
			return (int) Turn.right;
		}
		return (0);
	}

	private void turnHead (int direction){
		switch (direction){

		case (int) Turn.left:
			Debug.Log("Turning Left?");
			break;

		case (int) Turn.right:
			Debug.Log("Turning Right?");
			break;

		default:
			break;
		}
	}
}
