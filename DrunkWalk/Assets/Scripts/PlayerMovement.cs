using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// MOVE CONTROLLER TILT TO READ WHERE PLAYER LEANS AND RESULTING MOVEMENT 

public class PlayerMovement : MonoBehaviour {
	
	public float hinc = 0.5f;	// force increment for head
	public float finc = 0.5f; 	// force increment for feet

	// COMPONENTS/OBJECTS TO GET
	public Rigidbody rhead;		// rigidbody at the head of the player
	public Rigidbody rfeet;		// rigidbody at the feet of the player
	public Camera cam; 			// to force the camera to just fall over if leaning too much
	public Camera fallCam; 
	public UniMoveController UniMove; 	// get UniMove
	public DepthOfFieldScatter dof; 	// depth of field component on cam

	// BOUNDS TO CHECK TILT OF MOVE CONTROLLER
	private float initX, initZ;
	public float boundBack;
	public float boundForward;
	public float boundRight;
	public float boundLeft; 

	// TO INDICATE DIRECTION OF PLAYER'S MOVEMENT
	private enum Dir { forward, right, left, back }; 
	public int direction; 
	private bool fallen;
	public float angleBetween;  
	public float maxAngle; 
	public float maxAngleSides;

	// TIME STUFF
	public float currentTime;
	public float delayTime;
	public float currentSoundTime; 
	public float delaySound; 

	// FRAME STUFF (instead of time)
	public int currentFrame;
	public int delayFrame; 
	public int currentSoundFrame; 
	public int delaySoundFrame; 

	// SOUND STUFF  
	private bool soundPlayed; 
	public AudioClip[] clips; 

	// GET RBs' Y COORDS SO THAT THE PLAYER DOESN'T FLOAT OVER BED
	private float headY;
	
	List<UniMoveController> moves = new List<UniMoveController>();

	void Start () {
		rfeet.MovePosition(new Vector3(rhead.position.x, rfeet.position.y, rhead.position.z));
		int count = UniMoveController.GetNumConnected();

		for (int i = 0; i < count; i++){
			UniMove = GetComponent<UniMoveController>();

			if (!UniMove.Init(i)) 
			{	
				Destroy(UniMove);	// If it failed to initialize, destroy and continue on
				continue;
			}
					
			// This example program only uses Bluetooth-connected controllers
			PSMoveConnectionType conn = UniMove.ConnectionType;
			if (conn == PSMoveConnectionType.Unknown || conn == PSMoveConnectionType.USB) 
			{
				Destroy(UniMove);
			}
			else 
			{
				moves.Add(UniMove);
				
				// Start all controllers with a white LED
				UniMove.SetLED(Color.white);
			}
		}

		fallen = false;
		angleBetween = 0.0f; 
		
		soundPlayed = false; 

		initX = UniMove.ax;
		//initX = 0;
		initZ = UniMove.az;

		headY = rhead.position.y; 

	}
	
	void Update () {

		resetY (); 

		if (Input.GetKey("r"))
			Application.LoadLevel (Application.loadedLevel);

		// CHECK UNIMOVE BUTTONS 
		UniMoveSet ();
		
		// IF THE PLAYER LEANS TOO MUCH, FALL AND LOSE
		if (fallen) {
			fallToLose();
		}
		else {  
			angleBlur (angleBetween);
			direction = getLeanDirection(); 		//print ("1. got direction");
			fallen = isLeaningTooMuch (); 			
			moveHead (direction); 					//print ("2. moved head"); 
		}
	}

	void FixedUpdate() {

		// DELAYING PLACE FEET AT HEAD'S XY POS
		// currentTime += Time.deltaTime;
		currentFrame++; 
		//if (currentTime >= delayTime){
		if (currentFrame >= delayFrame){
			placeFeet (direction);
			//currentTime = 0.0f;
			currentFrame = 0;
		}

		// PLAY A GRUNT
		if (!soundPlayed){
			soundPlayed = true; 
			playGrunt (clips [Random.Range (0, 5)]);
			delaySound = Random.Range (5, 10); 
		}
		else {
			//currentSoundTime += Time.deltaTime;
			currentSoundFrame++; 

			//if (currentSoundTime >= delaySound){
			if (currentSoundFrame >= delaySoundFrame){
				soundPlayed = false; 
				//currentSoundTime = 0.0f; 
				currentSoundFrame = 0; 
			}
		}
	}

	// PREVENT THE COLLIDER FROM FLOATING ABOVE OBJECTS
	private void resetY(){
		rhead.MovePosition (new Vector3 (rhead.position.x, headY, rhead.position.z)); 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * (1) MAKE THE KNOB GLOW A COLOUR DEPENDING ON WHICH BUTTON IS PRESSED
	 * (2) SET THE RUMBLE BASED ON TRIGGER
	 * -------------------------------------------------------------------------------------------------------------------------- */
	private void UniMoveSet(){
		foreach (UniMoveController UniMove in moves) 
		{
			// Instead of this somewhat kludge-y check, we'd probably want to remove/destroy
			// the now-defunct controller in the disconnected event handler below.
			if (UniMove.Disconnected) continue;
			
			// Button events. Works like Unity's Input.GetButton
			// if (UniMove.GetButtonDown(PSMoveButton.Circle)){
			// 	Debug.Log("Circle Down");
			// }
			// if (UniMove.GetButtonUp(PSMoveButton.Circle)){
			// 	Debug.Log("Circle UP");
			// }
			
			// Change the colors of the LEDs based on which button has just been pressed:
			if (UniMove.GetButtonDown(PSMoveButton.Circle)) 		UniMove.SetLED(Color.cyan);
			else if(UniMove.GetButtonDown(PSMoveButton.Cross)) 		UniMove.SetLED(Color.red);
			else if(UniMove.GetButtonDown(PSMoveButton.Square)) 	UniMove.SetLED(Color.yellow);
			else if(UniMove.GetButtonDown(PSMoveButton.Triangle)) 	UniMove.SetLED(Color.magenta);
			else if(UniMove.GetButtonDown(PSMoveButton.Move)) 		UniMove.SetLED(Color.black);
			
			// Set the rumble based on how much the trigger is down
			UniMove.SetRumble(UniMove.Trigger);
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * PARAM: angleBetween = the angle between the head/feet vector and the vertical vector
	 * The closer angleBetween is to 30.0f, the blurrier things get!
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void angleBlur (float angle){
		
		//print ("ap = " + dof.aperture); 
		
		if (angle >= 0.3f && angle <= maxAngle){
			dof.aperture += 0.53f;
		} else if (angle < 0.3f){
			dof.aperture -= 0.7f;
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * (1) Check the current angle between the vector between the head rigidbody and the feed rigidboy with the vertical vector
	 * (2) If the angle is at least 30 degrees, then you are leaning too much! (return true)
	 * (3) otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private int getLeanDirection(){	//print("entered get direction");
		// ----------------- CHECK ALL DIRECTIONS NO PRIORITY

		if (UniMove.az <= boundBack + initZ) {
			return (int) Dir.back;
		}
		if (UniMove.az >= boundForward + initZ) {
			return (int) Dir.forward; 
		}
		if (UniMove.ax < boundRight + initX) {
			return (int) Dir.right; 
		}
		if (UniMove.ax > boundLeft + initX) {
			return (int) Dir.left; 
		}
		return (0);

		// ----------------- CHECK FRONT/BACK FIRST
		/*
		if (UniMove.az <= -0.2f + initZ) {

			if (UniMove.ax > -0.2f + initX && UniMove.ax < 0.2f + initX) {
				return (int) Dir.back; 
			}
			else {
				if (UniMove.ax > 0.2f + initX) {
					return (int) Dir.left; 
				}
				if (UniMove.ax < -0.2f + initX){
					return (int) Dir.right; 
				}
			}
		}
		else if (UniMove.az >= 0.7f + initZ){	

			if (UniMove.ax > -0.2f + initX && UniMove.ax< 0.2f + initX) {
				return (int) Dir.forward; 
			}
			else {
				if (UniMove.ax > 0.2f + initX) {
					return (int) Dir.left; 
				}
				if (UniMove.ax < -0.2f + initX){
					return (int) Dir.right; 
				}
			}
		}
		*/
		// CHECK RIGHT/LEFT FIRST
		/*
		// ----------------- LEFT
		if (UniMove.ax > 0.2f + initX) {
			if (UniMove.az > -0.2f + initZ && UniMove.az < 0.7f +initZ){
				return (int) Dir.left; 
			}
			else {
				if (UniMove.az <= -0.2f + initZ){
					return (int) Dir.back;
				}
				if (UniMove.az >= 0.7f + initZ){
					return (int) Dir.forward; 
				}
			}
		}
		else if (UniMove.ax < -0.2f + initX){
			if (UniMove.az > -0.2f + initZ && UniMove.az < 0.7f +initZ){
				return (int) Dir.left; 
			}
			else {
				if (UniMove.az <= -0.2f + initZ){
					return (int) Dir.back;
				}
				if (UniMove.az >= 0.7f + initZ){
					return (int) Dir.forward; 
				}
			}
		}
		*/
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * (1) Check the current angle between the vector between the head rigidbody and the feed rigidboy with the vertical vector
	 * (2) If the angle is at least 30 degrees, then you are leaning too much! (return true)
	 * (3) otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private bool isLeaningTooMuch(){ //print ("checking lean");
		Vector3 vertVec = new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z); 
		
		// (1) check angle between vectors
		float angle = Vector3.Angle (vertVec, rhead.position);
		angleBetween = angle;
		print ("angle = " + angle); 
		
		// (2) if angle is at least 30
		if (direction == (int) Dir.back || direction == (int) Dir.forward){
			if (angle >= maxAngle) { 	// print ("FALLEN!");
				return true;
			} 						// print ("STILL STANDING");
			return false; 
		}
		else {
			if (angle >= maxAngleSides) { 	// print ("FALLEN!");
				return true;
			} 						// print ("STILL STANDING");
			return false; 
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * !! FOR NOW, IF LEAN TOO MUCH, FALL AND LOSE THE GAME
	 * 
	 * (1) switch from main camera view to the fall camera view (going down to the floor)
	 * (2) play fallING sounds (progressively going down sounds)
	 * (3) play some kind of blink or blackout animation (fade out to black)
	 * (4) play fallEN to floor sound (random between many different fallen sounds)
	 * (5) SWITCH TO LOSE SCREEN (different scene)
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void fallToLose(){	
		//print ("YOU LOSE"); 
		
		// (1) switch cameras: from main to fallCam
		cam.enabled = false;
		fallCam.enabled = true; 
		
		// (2) play FALLING sounds
		
		// (3) animation? blink/blackout 
		
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
			rhead.AddForce (0, 0, (-hinc - 0.3f)); 
			//rhead.position = new Vector3 (rfeet.position.x, rhead.position.y, rfeet.position.z); 
			break; 
			
		default:
			break; 
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * AFTER DELAY, PLACE THE FEET DIRECTLY UNDER THE HEAD 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void placeFeet (int direction){			//print ("moving feet");
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
