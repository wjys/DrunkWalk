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
