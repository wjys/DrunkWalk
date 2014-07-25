using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrunkMovement : InGame {

   	// ___  ____ _  _ _  _ _  _    _  _ ____ _  _ ____ _  _ ____ _  _ ___ 
	// |  \ |__/ |  | |\ | |_/     |\/| |  | |  | |___ |\/| |___ |\ |  |  
	// |__/ |  \ |__| | \| | \_    |  | |__|  \/  |___ |  | |___ | \|  |  
	//

	// -------- PLAYER ID: 0 to 3 --------
	public int id; 	// assign in inspector
	
	// -------- ALL COMPONENTS TO GET IN START() -------- 
	public UniMoveController UniMove; 	// get UniMove
	public Rigidbody rhead;		// rigidbody at the head of the player
	public Animator meAnim;
	public GameObject feet; 
	public GameObject pfeet; 
	public Rigidbody rfeet;

	// -------- ALL OBJECTS/COMPONENTS TO BE DRAGGED INTO INSPECTOR -------- 
	public Camera cam; 				// to force the camera to just fall over if leaning too much
	public Camera fallCam;
	public DepthOfFieldScatter dof; // depth of field component on cam
	public Transform target;

	// ENUM TO SWITCH BETWEEN CONTROLLERS
	public int controller;
	private enum controlInput { mouse, move, xbox }; 

	// FORCE INCREMENTS FOR HEAD AND FEET
	public float hinc;
	public float incAcc;
	
	public float speed;
	public float smooth;

	// MOVE CONTROLLER
	private float initX, initZ;			// for calibration
	public float boundBack;				// bounds to check tilt of controller
	public float boundForward;
	public float boundRight;
	public float boundLeft; 

	// MOUSE INPUT
	private Vector3 mouse;		// current mouse position on screen
	private int halfWidth; 		// half the width of screen (mouse bounds)
	private int halfHeight; 	// half the height of screen (mouse bounds) 

	private enum Dir { forward, right, left, back }; 
	public int direction;
	
	public float radius;
	public float maxRad; 
	
	// FRAME STUFF (instead of time)
	public int currentFrame;
	public int delayFrame; 
	public int currentSoundFrame; 
	public int delaySoundFrame; 
	
	// SOUNDS
	public AudioClip[] clips;
	public AudioClip footstep;
	private bool soundPlayed;
	private float footstepTimer;

	public float stepDiff;
	
	// GET RBs' Y COORDS SO THAT THE PLAYER DOESN'T FLOAT OVER BED
	private float headY;

	// GET BACK UP ONCE FALLEN
	public bool fallen;
	public bool buttonTapped;

	private Vector3 fallenPos; 
	private Quaternion fallenRot; 
	public int tapsGetUp;
	public int frameFall; 
	public int tapCurrent;
	private bool frozen;
	public int fallCt;
	public int maxFallCt; 

	private bool gettingUp;
	private Quaternion newRot;
	private Vector3 newPos;
	
	private static bool camLerp;

	// LERP FOOT???
	private Vector3 newFeetPos;
	private bool moveFeet;

	//COLLISION RUMBLE
	public float hitRumble;

	//TRIGGER
	private bool trigger1;
	private static float triggerInt;


	void Start () {
		UniMove = gameObject.GetComponent<UniMoveController> ();
		pfeet = Instantiate (feet) as GameObject;
		pfeet.name = "Feet " + id;
		rfeet = pfeet.GetComponent<Rigidbody> ();
		rhead = gameObject.GetComponent<Rigidbody> ();
		meAnim = gameObject.GetComponent<Animator> ();
		triggerInt = UniMove.Trigger;

		trigger1 = false;

		// Place feet under head
		rfeet.MovePosition(new Vector3(transform.position.x, rfeet.position.y, transform.position.z));

		//Bools
		fallen = false;		
		soundPlayed = false; 		
		frozen = false; 
		fallCt = 0;
		headY = transform.position.y; 

		//Mouse Start
		halfWidth = Screen.width / 2; 
		halfHeight = Screen.height / 2; 

		//PSMove Start
		initX = UniMove.ax;	// calibrate ax
		initZ = UniMove.az;	// calibrate az
	}
	
	void Update () {

		setMoveColour ();

		//print (triggerInt);
		if (triggerInt == 1){
			trigger1 = true;
			triggerInt = 0;
		} else {
			trigger1 = false;
		}

		triggerInt = UniMove.Trigger;


		resetY (); 
		
		if (Input.GetKey("r"))
			Application.LoadLevel (Application.loadedLevel);
		
		// IF THE PLAYER LEANS TOO MUCH, FALL AND LOSE
		if (fallen) {
			tapsToGetUp();
			direction = -1;
		}
		else {  
			transform.LookAt(target, Vector3.up); 
			angleBlur ();
			if (controller == (int) controlInput.mouse) 
				mouse = Input.mousePosition;
			direction = getLeanDirection(); 
			fallen = isLeaningTooMuch (); 			
			moveHead (direction); 

		}

		playFootstep(radius * 100);

		newPos = transform.position;
	}
	
	void FixedUpdate() {

		if (camLerp == true){
			transform.rotation = Quaternion.Lerp(transform.rotation, newRot, smooth * Time.deltaTime);//new Quaternion (transform.rotation.x, 0, transform.rotation.z, 0), smooth * Time.deltaTime);
			camLerp = false;
		}

		if (moveFeet == true){
			rfeet.position = Vector3.Lerp (rfeet.position, newFeetPos, smooth * Time.deltaTime);
		}
		
		// DELAYING PLACE FEET AT HEAD'S XY POS
		currentFrame++; 
		if (!fallen){
			if (currentFrame >= delayFrame){
				placeFeet ();
				currentFrame = 0;
				moveFeet = true;
			}
		}
		
		// PLAY A GRUNT
		if (!soundPlayed){
			soundPlayed = true; 
			playSound (clips [Random.Range (0, 5)]);
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
	

	/* --------------------------------------------------------------------------------------------------------------------------
	 * PARAM: angleBetween = the angle between the head/feet vector and the vertical vector
	 * The closer angleBetween is to 30.0f, the blurrier things get!
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void angleBlur (){

		if (radius >= 0.5f && radius <= maxRad){
			dof.aperture += 0.5f;
		} else if (radius < 0.5f){
			dof.aperture -= 0.7f;
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * (1) Check the current angle between the vector between the head rigidbody and the feed rigidboy with the vertical vector
	 * (2) If the angle is at least 30 degrees, then you are leaning too much! (return true)
	 * (3) otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private int getLeanDirection(){

		if (controller == (int) controlInput.mouse) {
			if (mouse.y < halfHeight) {	
				if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){ 
					return (int) Dir.back; 
				}
				else {
					if (mouse.x >= halfWidth){		
						return (int) Dir.right; 
					}
					else {	
						return (int) Dir.left; 
					}
				}
			}
			else if (mouse.y >= halfHeight) {
				if (Mathf.Abs(mouse.x - halfWidth) < Mathf.Abs(mouse.y - halfHeight)){
					return (int) Dir.forward; 
				}
				else {
					if (mouse.x >= halfWidth){ 
						return (int) Dir.right; 
					}
					else {
						return (int) Dir.left; 
					}
				}
			}
		}
		else if (controller == (int) controlInput.move){
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
		else if (controller == (int) controlInput.xbox){

			if (Input.GetAxis("LeftStickY") > 0.9f){
				return (int) Dir.back;
			}
			if (Input.GetAxis("LeftStickY") < -0.9f){
				return (int) Dir.forward;
			}
			if (Input.GetAxis("LeftStickX") > 0.9f){
				return (int) Dir.right;
			}
			if (Input.GetAxis("LeftStickX") < -0.9f){
				return (int) Dir.left; 
			}
		}
		return (0);
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * Play the footsteps -> time between footsteps depend on the radius between the head and the feet 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void playFootstep (float speed) {
		footstepTimer += Time.deltaTime;
		if (direction > -1){
				float nowSpeed = stepDiff * speed;
				if (footstepTimer > nowSpeed) {
					footstepTimer -= nowSpeed;
					playSound(footstep);
				}
			}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * RETURN BOOL to fallen: true if isLeaningTooMuch. NO ARGS. 
	 * (1) Check the current angle between the vector between the head rigidbody and the feed rigidboy with the vertical vector
	 * (2) If the angle is at least 30 degrees, then you are leaning too much! (return true)
	 * (3) otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private bool isLeaningTooMuch(){ 
		
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
	 * NO RETURN. ARG: the direction the player is leaning 
	 * (1) switch/case to check which direction we're leaning
	 * (2) add the force in the appropriate direction
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void moveHead (int direction){
		//Invoke("resetHinc", 1);
		switch (direction) {
			
		case (int) Dir.forward:	
			rhead.AddForce (hinc*transform.forward);  
			break;
			
		case (int) Dir.right:
			rhead.AddForce (hinc*transform.right); 
			break;
			
		case (int) Dir.left:
			rhead.AddForce (-hinc*transform.right); 
			break;
			
		case (int) Dir.back:
			rhead.AddForce (-hinc*transform.forward); 
			break; 
			
		default:
			break; 
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * AFTER DELAY, PLACE THE FEET DIRECTLY UNDER THE HEAD.
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void placeFeet (){

		newFeetPos = new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z);
		//rfeet.MovePosition(new Vector3 (rhead.position.x, rfeet.position.y, rhead.position.z)); 
		moveFeet = false;

	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * WHAT HAPPENS WHEN YOU FALL - P1 
	 * stopRead(): no arg, no return
	 * 		(1) saves the position and rotation of the player before falls, 
	 * 		(2) freezes player movement and rotation
	 * 		(3) increase the number of times the player has fallen
	 * 		(4) if the player has fallen the max number of times allowed, lose the game 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void stopRead(){
		fallenPos = transform.position; 
		fallenRot = transform.rotation; 
		currentFrame = 0; 
		frozen = true;

		rhead.constraints = RigidbodyConstraints.FreezeAll;
		fallCt++; 
		if (fallCt >= maxFallCt) {
			GameManager.ins.playerStatus = GameState.PlayerStatus.Lost;
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * WHAT HAPPENS WHEN YOU FALL - PART 2: ELECTRIC BOOGALOO
	 * tapsToGetUp(): no arg, no return
	 * 		(1) change the player status in the game manager to fallen
	 * 		(2) switch to the fall camera
	 * 		(3) play the fall over animation
	 * 		(4) stopRead() if hasn't already been stopped (i.e. if frozen is false)
	 * 		(5) 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	public void tapsToGetUp(){

		//Set Global Player State
		if (GameManager.ins.playerStatus != GameState.PlayerStatus.Fallen){
			GameManager.ins.playerStatus = GameState.PlayerStatus.Fallen;
		}

		//Disable maincam, enable fallcam
		cam.enabled = false;
		fallCam.enabled = true;

		//Play fallover Anim
		meAnim.SetBool("fallOver", true);

		//At this point, no movement should be read
		if (!frozen){
			stopRead (); 
		}

		/////////////////
		//[TAP TO GET UP]
		/////////////////

		//buttonTapped = (trigger1 = true);
		buttonTapped = Input.anyKeyDown; 
		//buttonTapped = UniMove.Trigger;

		// read button taps 
		if (buttonTapped) {
			//print ("TAPPED");
			tapCurrent++; 
		}
		if (tapCurrent >= tapsGetUp) {
			//SUCCESSFULLY GOT UP

			meAnim.SetBool("fallOver", false);
			meAnim.SetBool("getUp", true);
			if (GameManager.ins.playerStatus == GameState.PlayerStatus.Fallen){
				GameManager.ins.playerStatus = GameState.PlayerStatus.Fine;
            }
			//IF PLAYING FINE, GET BACK UP
			if (meAnim.GetCurrentAnimatorStateInfo(0).IsName("Fine")){
				GetUp();
			}
		} // LOST
		else if (currentFrame >= frameFall){
			//GameManager.ins.playerStatus = GameState.PlayerStatus.Lost;
		}
	}

	/////////////////
	//GETTING UP/////
	/////////////////

	private void GetUp(){
		cam.enabled = true;
		fallCam.enabled = false;

		newPos = fallenPos;

		//RESET POSITION
		transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);
		rfeet.position = new Vector3 (fallenPos.x, rfeet.position.y, fallenPos.z);
		StartCoroutine(backToOrigin());

		//Set Global Player State
		
	}

	public IEnumerator backToOrigin () {
		newRot = new Quaternion(0.0f, fallenRot.y, 0.0f, fallenRot.w);
		gettingUp = true;
		camLerp = true;

		yield return new WaitForSeconds(0.5f);

		frozen = false;
		rhead.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;

		//UniMove.ax = initX;
		//UniMove.az = initZ;

		tapCurrent = 0; 
		gettingUp = false;
		fallen = false;
		//newRot = transform.rotation;

	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * PLAY SELECTED GRUNT SOUND
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void playSound(AudioClip clip){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clip); 
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * SET MOVE CONTROLLER COLOUR
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void setMoveColour(){
		switch (id) {
		case 1:
			UniMove.SetLED (Color.cyan);
			break;
		case 2:
			UniMove.SetLED (Color.red);
			break;
		case 3:
			UniMove.SetLED (Color.green);
			break;
		case 4:
			UniMove.SetLED (Color.magenta);
			break;
		default:
			break;
		}
	}
}
