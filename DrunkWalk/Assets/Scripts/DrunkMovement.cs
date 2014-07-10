using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrunkMovement : InGame {

	public bool useMouse; 

	// FORCE INCREMENTS FOR HEAD AND FEET
	public float hinc;
	public float incAcc;

	public float smooth = 1.5f;

	// MOVE CONTROLLER
	public UniMoveController UniMove; 	// get UniMove
	List<UniMoveController> moves = new List<UniMoveController>();
	private float initX, initZ;			// for calibration
	public float boundBack;				// bounds to check tilt of controller
	public float boundForward;
	public float boundRight;
	public float boundLeft; 

	// MOUSE INPUT
	private Vector3 mouse;		// current mouse position on screen
	private int halfWidth; 		// half the width of screen (mouse bounds)
	private int halfHeight; 	// half the height of screen (mouse bounds) 
	
	// GENERAL VARIABLES FOR ALL CONTROLS 
	public Rigidbody rhead;		// rigidbody at the head of the player
	public Rigidbody rfeet;		// rigidbody at the feet of the player
	public DrunkForce df; 
	public Camera cam; 			// to force the camera to just fall over if leaning too much
	public DepthOfFieldScatter dof; // depth of field component on cam


	private enum Dir { forward, right, left, back }; 
	public int direction;
	
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

	// GET BACK UP ONCE FALLEN
	private bool fallen;
	private Vector3 fallenPos; 
	private Quaternion fallenRot; 
	public int tapsGetUp;
	public int frameFall; 
	public int tapCurrent;
	private bool frozen; 


	// Use this for initialization
	void Start () {
		rfeet.MovePosition(new Vector3(transform.position.x, rfeet.position.y, transform.position.z));
		fallen = false;		
		soundPlayed = false; 		
		headY = transform.position.y; 
		frozen = false; 

		//Mouse Start
		halfWidth = Screen.width / 2; 
		halfHeight = Screen.height / 2; 

		//PSMove Start
		initX = UniMove.ax;	// calibrate ax
		initZ = UniMove.az;	// calibrate az
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
	}
	
	void Update () {
		
		resetY (); 
		
		if (Input.GetKey("r"))
			Application.LoadLevel (Application.loadedLevel);
		
		// CHECK UNIMOVE BUTTONS 
		UniMoveSet ();
		
		// IF THE PLAYER LEANS TOO MUCH, FALL AND LOSE
		if (fallen) {
			tapsToGetUp(); 
			//fallToLose();
		}
		else {  
			//angleBlur (angleBetween);
			if (useMouse) 
				mouse = Input.mousePosition;
			direction = getLeanDirection(); 		//print ("1. got direction");
			fallen = isLeaningTooMuch (); 			
			moveHead (direction); 					//print ("2. moved head"); 
		}

	}
	
	void FixedUpdate() {
		
		// DELAYING PLACE FEET AT HEAD'S XY POS
		currentFrame++; 
		if (!fallen){
			if (currentFrame >= delayFrame){
				placeFeet ();
				currentFrame = 0;
			}
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



	// PREVENT THE COLLIDER FROM FLOATING ABOVE OBJECTS
	private void resetY(){
		rhead.MovePosition (new Vector3 (transform.position.x, headY, transform.position.z)); 
	}

	//Set PSMove
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
		/*
		if (angle >= 0.3f && angle <= maxAngle){
			dof.aperture += 0.53f;
		} else if (angle < 0.3f){
			dof.aperture -= 0.7f;
		}
		*/
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * (1) Check the current angle between the vector between the head rigidbody and the feed rigidboy with the vertical vector
	 * (2) If the angle is at least 30 degrees, then you are leaning too much! (return true)
	 * (3) otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private int getLeanDirection(){	//print("entered get direction");
		// ----------------- CHECK ALL DIRECTIONS NO PRIORITY
		if (useMouse) {
			if (mouse.y < halfHeight) {	
				if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){ 
					return (int) Dir.back; 
				}
				else {
					if (mouse.x >= halfWidth){		// print ("leaning right");
						return (int) Dir.right; 
					}
					else {							// print ("leaning left");
						return (int) Dir.left; 
					}
				}
			}
			else if (mouse.y >= halfHeight) {
				if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){	// print ("leaning forward");
					return (int) Dir.forward; 
				}
				else {
					if (mouse.x >= halfWidth){ 	// print ("leaning right");
						return (int) Dir.right; 
					}
					else {						// print ("leaning left");
						return (int) Dir.left; 
					}
				}
			}
		}
		else {
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
		}
		return (0);
	}

	// public void toggleLR(){
	// 	if (wentL){
	// 		hinc = 1;
	// 		wentL = false;
	// 	} else if (wentR){
	// 		hinc = 1;
	// 		wentR = false;
	// 	}
	// }

	// public void toggleFB(){
	// 	if (wentF){
	// 		hinc = 1;
	// 		wentF = false;
	// 	} else if (wentB){
	// 		hinc = 1;
	// 		wentB = false;
	// 	}
	// }
	
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
	 * DEPENDING ON THE LEAN DIRECTION, ADD A FORCE TO THE RIGIDBODY OF THE HEAD
	 * 
	 * (1) switch/case to check which direction we're leaning
	 * (2) add the force in the appropriate direction
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void moveHead (int direction){	//print ("moving head ");
		//Invoke("resetHinc", 1);
		
		switch (direction) {
			
		case (int) Dir.forward:				//print ("moving head forward");
			rhead.AddForce (hinc*transform.forward);  
			break;
			
		case (int) Dir.right:				//print ("moving head to the right");
			rhead.AddForce (hinc*transform.right); 
			break;
			
		case (int) Dir.left:				//print ("moving head to the left");
			rhead.AddForce (-hinc*transform.right); 
			break;
			
		case (int) Dir.back:				//print ("stopping head movement");
			rhead.AddForce (-hinc*transform.forward); 
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
		// rfeet.MovePosition(Vector3.Lerp(rfeet.position, transform.position, smooth * Time.frameCount));
		rfeet.MovePosition(new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z)); 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * WORKING ON THIS~
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void stopRead(){
		fallenPos = transform.position; 
		fallenRot = transform.rotation; 
		currentFrame = 0; 
		frozen = true;
		rhead.isKinematic = true;

		df.enabled = false; 
	}
	
	public void tapsToGetUp(){
		print ("CLICK!"); 
		if (!frozen){
			stopRead (); 
		}
		bool buttonTapped = Input.anyKeyDown; 
		// read button taps 
		if (buttonTapped) {
			tapCurrent++; 
		}
		if (tapCurrent >= tapsGetUp) {
			print ("WINnie"); 
			GetUp (); 
		}
		else if (currentFrame >= frameFall){
			print ("BOOi"); 
			fallToLose (); 
		}
	}
	
	private void GetUp(){
		transform.position = fallenPos;
		rfeet.position = new Vector3 (fallenPos.x, rfeet.position.y, fallenPos.z); 
		transform.rotation = new Quaternion (0, fallenRot.y, 0, fallenRot.w); 
		tapCurrent = 0; 
		frozen = false;
		rhead.isKinematic = false;
		fallen = false; 
		 
		df.enabled = true; 
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
		
		// (2) play FALLING sounds
		
		// (3) animation? blink/blackout 
		
		// (4) play FALLEN TO FLOOR sound
		
		// (5) SWITCH TO LOSE SCREEN
		Debug.Log ("Fallen");
		//Application.LoadLevel("Lost"); 
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
