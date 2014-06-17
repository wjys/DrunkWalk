using UnityEngine;
using System.Collections;

// MOUSE POSITION TO READ WHERE PLAYER LEANS AND RESULTING MOVEMENT 

public class PlayerMovement : MonoBehaviour {
	
	public Vector2 mouse;	// current mouse position on screen

	public Rigidbody rhead;	// rigidbody at the head of the player
	public Rigidbody rfeet;	// rigidbody at the feet of the player
	public ConstantForce cforce; 	// constant force acting on head of the player 

	private int halfWidth; 
	private int halfHeight; 

	void Start () {
		halfWidth = Screen.width / 2; 
		halfHeight = Screen.height / 2; 
	}
	
	void Update () {
		// get the current mouse position
		mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// if leaning right 
		if (mouse.x >= halfWidth) {
			// check if lean forward
			if (mouse.y >= halfHeight){
				// check if right lean is more 'important' than lean forward
				if (Mathf.Abs(mouse.x - halfWidth) >= Mathf.Abs(mouse.y - halfHeight)){

				}
				// backwards lean overrides the side leans 
				else {

				}
			}
			else { // lean back 

			}
		}
		// if leaning left
		else {
			// check if lean forward
			if (mouse.y >= halfHeight){
				// check if right lean is more 'important' than lean forward
				if (Mathf.Abs(mouse.x - halfWidth) >= Mathf.Abs(mouse.y - halfHeight)){
					
				}
				// backwards lean overrides the side leans 
				else {

				}
			}
			else { // lean back 
				
			}
		}
	}
}
