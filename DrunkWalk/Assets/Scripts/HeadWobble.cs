using UnityEngine;
using System.Collections;

public class HeadWobble : MonoBehaviour {

	public GameObject player; 
	public DrunkMovement dm; 
	public float targetInc; 
	private Vector3 currentPosition; 

	public float fwdCap;
	public float bckCap; 

	public bool stopWobble; 
	private enum Dir { forward, right, left, back };

	void Start () {
		stopWobble = false; 
	}

	void Update () {
		if (!stopWobble)
			//transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, player.transform.position.z + 1); 
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
			if (player.transform.rotation.x >= fwdCap){
				print ("lean too fwd"); 
				dm.fallen = true; 
			}
			else {
				print ("leaning fwd");
				transform.position = new Vector3 (transform.position.x, transform.position.y - (dm.radius/dm.maxRad)*targetInc, player.transform.position.z + 1);
			}
			break;
		case (int) Dir.back:
			//print ("bk " + player.transform.rotation.x); 
			if (player.transform.rotation.x <= bckCap){
				print ("lean too bk"); 
				dm.fallen = true; 
			}
			else {
				print ("leaning bk");
				transform.position = new Vector3 (transform.position.x, transform.position.y + (dm.radius/dm.maxRad)*targetInc, player.transform.position.z + 1);
			}
			break;
		case (int) Dir.right:
			//print ("right, angle = " + player.transform.rotation.y); 
			print ("leaning rgt");
			transform.position = new Vector3 (transform.position.x + (dm.radius/2)*targetInc, transform.position.y, player.transform.position.z + 1);
			break;
		case (int) Dir.left:
			print ("leaning lft"); 
			transform.position = new Vector3 (transform.position.x - (dm.radius/2)*targetInc, transform.position.y, player.transform.position.z + 1);
			break; 
		}
	}
}
