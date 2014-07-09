using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// MOUSE POSITION TO READ WHERE PLAYER LEANS AND RESULTING MOVEMENT 

public class MouseMovement : InGame {
	
	public Vector3 mouse;		// current mouse position on screen
	public float hinc = 0.5f;	// force increment for head
	public float finc = 0.5f; 	// force increment for feet

	// OBJECTS TO DRAG INTO COMPONENT
	public Rigidbody rhead;		// rigidbody at the head of the player
	public Rigidbody rfeet;		// rigidbody at the feet of the player
	public Camera cam; 			// to force the camera to just fall over if leaning too much
	public DepthOfFieldScatter dof; // depth of field component on cam

	private int halfWidth; 		// half the width of screen
	private int halfHeight; 	// half the height of screen
	
	private enum Dir { forward, right, left, back }; 
	public int direction;


	private bool fallen;

	public float radius;
	public float maxRad; 

	// FRAME STUFF (instead of time)
	public int currentFrame;
	public int delayFrame; 
	public int currentSoundFrame; 
	public int delaySoundFrame; 

	// sound stuff 
	public AudioClip[] clips; 
	private bool soundPlayed; 

	// GET RBs' Y COORDS SO THAT THE PLAYER DOESN'T FLOAT OVER BED
	private float headY;



	void Start () {
		rfeet.MovePosition(new Vector3(transform.position.x, rfeet.position.y, transform.position.z));
		
		fallen = false;
		
		soundPlayed = false; 
		
		headY = transform.position.y; 
		halfWidth = Screen.width / 2; 
		halfHeight = Screen.height / 2; 
	}
	
	void Update () {
		resetY (); 
		
		if (Input.GetKey("r"))
			Application.LoadLevel (Application.loadedLevel);
		
		// IF THE PLAYER LEANS TOO MUCH, FALL AND LOSE
		if (fallen) {
			currentFrame = 0; 
			fallToLose();
		}
		else {  
			mouse = Input.mousePosition;
			//angleBlur (angleBetween);
			direction = getLeanDirection(mouse); 		//print ("1. got direction");
			fallen = isLeaningTooMuch (); 			
			moveHead (direction); 					//print ("2. moved head"); 
		}
	}

	void FixedUpdate() {
		
		// DELAYING PLACE FEET AT HEAD'S XY POS
		currentFrame++; 
		if (currentFrame >= delayFrame){
			placeFeet ();
			currentFrame = 0;
		}
		
		// PLAY A GRUNT
		if (!soundPlayed){
			soundPlayed = true; 
			playGrunt (clips [Random.Range (0, 5)]);
		}
		else {
			currentSoundFrame++; 
			
			if (currentSoundFrame >= delaySoundFrame){
				soundPlayed = false; 
				currentSoundFrame = 0; 
			}
		}
	}

	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * PARAM: angleBetween = the angle between the head/feet vector and the vertical vector
	 * The closer angleBetween is to 30.0f, the blurrier things get!
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void angleBlur (float angle){

		//print ("ap = " + dof.aperture); 
		/*
		if (angle >= 0.3f && angle <= maxAngle){
			dof.aperture += 0.5f;
		} else if (angle < 0.3f){
			dof.aperture -= 0.8f;
		}*/
	}

	// PREVENT THE COLLIDER FROM FLOATING ABOVE OBJECTS
	private void resetY(){
		rhead.MovePosition (new Vector3 (transform.position.x, headY, transform.position.z)); 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * PARAM: mouse vector3 (where it is) => return the direction in which the character should be leaning 
	 * 
	 * (a) the mouse is currently in the bottom half of the game screen
	 * 		(a1) mouse is further down than in either L/R directions of the screen => lean BACK
	 * 		(a2) mouse is further to the RIGHT => lean RIGHT
	 * 		(a3) mouse is further to the LEFT => lean LEFT
	 * 
	 * (b) mouse currently in upper half of the game screen
	 * 		(b1) mouse is further up than to the L/R sides => lean FORWARD
	 * 		(b2) mouse is further to the RIGHT => lean RIGHT
	 * 		(b3) mouse is further to the LEFT => lean LEFT
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private int getLeanDirection(Vector3 mouse){	//print("entered get direction");

		// (a) 
		if (mouse.y < halfHeight) {	
			// (a1)
			if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){ 
				return (int) Dir.back; 
			}
			else {
				// (a2)
				if (mouse.x >= halfWidth){		// print ("leaning right");
					return (int) Dir.right; 
				}
				// (a3)
				else {							// print ("leaning left");
					return (int) Dir.left; 
				}
			}
		}
		// (b)
		else if (mouse.y >= halfHeight) {
			// (b1)
			if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){	// print ("leaning forward");
				return (int) Dir.forward; 
			}
			else {
				// (b2)
				if (mouse.x >= halfWidth){ 	// print ("leaning right");
					return (int) Dir.right; 
				}
				// (b3)
				else {						// print ("leaning left");
					return (int) Dir.left; 
				}
			}
		}
		return (-1);
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * (1) Check the current angle between the vector between the head rigidbody and the feed rigidboy with the vertical vector
	 * (2) If the angle is at least 30 degrees, then you are leaning too much! (return true)
	 * (3) otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private bool isLeaningTooMuch(){ //print ("checking lean");
		
		float x = Mathf.Abs (rfeet.position.x - transform.position.x);
		float y = Mathf.Abs (rfeet.position.z - transform.position.z); 
		radius = Mathf.Sqrt (x * x + y * y); 
		
		if (radius >= maxRad) {
			return true; 
		}
		return false; 
	}
	/* --------------------------------------------------------------------------------------------------------------------------
	 * !! FOR NOW, IF LEAN TOO MUCH, FALL AND LOSE THE GAME
	 * 
	 * (1) switch from main camera view to the fall camera view (going down to the floor)
	 * (2) play fallING sounds (progressively going down sounds)
	 * (3) play some kind of blink or blackout animation (fade out to black)
	 * (4) play fallEN to floor sound (random between many different fallen sounds)
	 * (5) display "YOU LOST/TRY AGAIN" 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	public void fallToLose(){	
		//print ("YOU LOSE"); 
		
		// (2) play FALLING sounds
		
		// (3) animation? blink/blackout 
		cam.enabled = false; 

		// (4) play FALLEN TO FLOOR sound
		
		// (5) SWITCH TO LOSE SCREEN
		Debug.Log ("Fallen");
		//Application.LoadLevel("Lost"); 
	}
	

	/* --------------------------------------------------------------------------------------------------------------------------
	 * DEPENDING ON THE LEAN DIRECTION, ADD A FORCE TO THE RIGIDBODY OF THE HEAD
	 * 
	 * (1) switch/case to check which direction we're leaning
	 * (2) add the force in the appropriate direction
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void moveHead (int direction){	//print ("moving head ");
		
		switch (direction) {
			
		case (int) Dir.forward:				//print ("moving head forward");
			rhead.AddForce (0, 0, hinc);  
			break;
			
		case (int) Dir.right:				//print ("moving head to the right");
			rhead.AddForce (hinc, 0, 0); 
			break;
			
		case (int) Dir.left:				//print ("moving head to the left");
			rhead.AddForce (-hinc, 0, 0); 
			break;
			
		case (int) Dir.back:				//print ("stopping head movement");
			rhead.AddForce (0, 0, -hinc); 
			//rhead.position = new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z); 
			break; 
			
		default:
			break; 
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * AFTER DELAY, PLACE THE FEET DIRECTLY UNDER THE HEAD 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void placeFeet (){			//print ("moving feet");
		rfeet.MovePosition(new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z)); 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * PLAY SELECTED GRUNT SOUND
	 * -------------------------------------------------------------------------------------------------------------------------- */
		
	private void playGrunt(AudioClip clip){

		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clip); 
	
	}
}
