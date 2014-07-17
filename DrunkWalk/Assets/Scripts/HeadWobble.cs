using UnityEngine;
using System.Collections;

public class HeadWobble : MonoBehaviour {

	public GameObject player; 
	public DrunkMovement dm; 
	public float targetInc; 
	private Vector3 currentPosition; 

	public bool stopWobble; 
	private enum Dir { forward, right, left, back };

	void Start () {
		stopWobble = false; 
	}

	void Update () {
		if (!stopWobble)
			headTilt (dm.direction); 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO RETURN. ARG: int lean = the direction that the player is moving in
	 * 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	private void headTilt(int lean){
		switch (lean) {
		case (int) Dir.forward:
			//print ("fwd " + player.transform.rotation.x); 
			if (player.transform.rotation.x >= 0.15f){
				print ("lean too fwd"); 
				dm.fallen = true; 
			}
			else {
				transform.position = new Vector3 (transform.position.x, transform.position.y - (dm.radius/2)*targetInc, transform.position.z);
			}
			break;
		case (int) Dir.back:
			//print ("bk " + player.transform.rotation.x); 
			if (player.transform.rotation.x <= -0.15f){
				print ("lean too bk"); 
				dm.fallen = true; 
			}
			else {
				transform.position = new Vector3 (transform.position.x, transform.position.y + (dm.radius/2)*targetInc, transform.position.z);
			}
			break;
		case (int) Dir.right:
			//print ("right, angle = " + player.transform.rotation.y); 

			//.position = new Vector3 (transform.position.x + (dm.radius/2)*targetInc, transform.position.y, transform.position.z);
			break;
		case (int) Dir.left:
			//transform.position = new Vector3 (transform.position.x - (dm.radius/2)*targetInc, transform.position.y, transform.position.z);
			break; 
		}
	}
}
