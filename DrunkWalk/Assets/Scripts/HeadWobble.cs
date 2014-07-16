using UnityEngine;
using System.Collections;

public class HeadWobble : MonoBehaviour {


	public DrunkMovement player; 
	public float targetInc; 

	public bool stopWobble; 
	private enum Dir { forward, right, left, back };

	void Start () {
		stopWobble = false; 
	}

	void Update () {
		if (!stopWobble)
			headTilt (player.direction); 
	}

	private void headTilt(int lean){
		switch (lean) {
		case (int) Dir.forward:
			transform.position = new Vector3 (transform.position.x, transform.position.y - (player.radius/2)*targetInc, transform.position.z);
			break;
		case (int) Dir.back:
			transform.position = new Vector3 (transform.position.x, transform.position.y + (player.radius/2)*targetInc, transform.position.z);
			break;
		case (int) Dir.right:
			transform.position = new Vector3 (transform.position.x + (player.radius/2)*targetInc, transform.position.y, transform.position.z);
			break;
		case (int) Dir.left:
			transform.position = new Vector3 (transform.position.x - (player.radius/2)*targetInc, transform.position.y, transform.position.z);
			break; 
		}
	}
}

/*
	private float getAngleX(){
			float diffX = transform.localPosition.x - feet.transform.localPosition.x;
			float diffY = transform.localPosition.y - feet.transform.localPosition.y;
			return (Mathf.Atan(diffX/diffY)*Mathf.Rad2Deg);
	}	

	private float getAngleZ(){
			float diffZ = transform.localPosition.z - feet.transform.localPosition.z;
			float diffY = transform.localPosition.y - feet.transform.localPosition.y;
			return (Mathf.Atan(diffZ/diffY)*Mathf.Rad2Deg);
	}


	private void headWobbleX(float angle){
		transform.localRotation = Quaternion.Euler (angle, transform.localRotation.y, transform.localRotation.z); 
	}
	private void headWobbleZ(float angle){
		transform.localRotation = Quaternion.Euler (transform.localRotation.x, transform.rotation.y, angle);
	}

	// THE FOLLOWING AREN'T USED
	/*
	private int ReadRadiusX(){
		if (transform.localPosition.x > feet.transform.localPosition.x) {
			return (int) Dir.forward;
		}
		else if (transform.localPosition.x < feet.transform.localPosition.x){
			return (int) Dir.back;
		}
		return -1; 
	}
	
	private int ReadRadiusZ(){
		if (transform.localPosition.z > feet.transform.localPosition.z){
			return (int) Dir.right;
		}
		else if (transform.localPosition.z < feet.transform.localPosition.z){
			return (int) Dir.left; 
		}
		return -1; 
	}
	*/
}
