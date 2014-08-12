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
	public float radDiff;
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
	
	// GET RBs' Y COORDS SO THAT THE PLAYER DOESN'T FLOAT OVER BED
	public float headY;
	private Vector3 initHead; 

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

	private Quaternion newRot;
	private Vector3 newPos;
	
	public bool camLerp;

	//LERP FOOT
	private Vector3 newFeetPos;
	private bool moveFeet;

	//COLLISION RUMBLE
	public float hitRumble;

	//TRIGGER
	public bool lidUp;
	public bool switchViews;
	public bool checkTaps;
	public bool gettingUp;
	public bool colliding;

	//ANIMATION
	public GameObject model;
	public Animator modelAnim;

	/* --------------------------------------------------------------------------------------------------------------------------
	 * START
	 * (1) setup all the components of the head game object
	 * (2) place the feet under the head
	 * (3) setup the variables (bools and integers)
	 * (4) get the head's initial Y (for constant resettting)
	 * (5) get the middle position of the screen (for mouse input)
	 * (6) calibrate the move controller 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void Start () {
		// (1) setup all the components
		UniMove = gameObject.GetComponent<UniMoveController> ();
		pfeet = Instantiate (feet) as GameObject;
		pfeet.name = "Feet " + id;
		rfeet = pfeet.GetComponent<Rigidbody> ();
		rhead = gameObject.GetComponent<Rigidbody> ();
		meAnim = gameObject.GetComponent<Animator> ();

		// (2) place feet under head
		pfeet.transform.position = new Vector3 (transform.position.x, pfeet.transform.position.y, transform.position.z);

		// (3) setup all the variables
		fallen = false;		
		frozen = false; 
		fallCt = 0;
		colliding = false;
		initHead = transform.position;
		//headY = transform.position.y; 

		// (4) get middle of the screen (for mouse)
		halfWidth = Screen.width / 2; 
		halfHeight = Screen.height / 2; 

		// (5) calibrate the move controller 
		initX = UniMove.ax;	// calibrate ax
		initZ = UniMove.az;	// calibrate az

		// (7) get animator of model
		Transform[] trans = gameObject.GetComponentsInChildren<Transform>();

		foreach (Transform t in trans){
			if (t.gameObject.name.Equals ("Model")){
				model = t.gameObject;
			}
		}

		modelAnim = model.GetComponent<Animator>();

		modelAnim.SetBool("Falling", false);
		modelAnim.SetBool ("GetUp", false);
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * UPDATE
	 * (1) set the lit up colour of the move controller of current player
	 * (2) move trigger NOT REALLY WORKING?
	 * (3) reset the y position of the head to keep it constant
	 * (4) if the player has FALLEN, tapsToGetUp() 
	 * (5) if the player has NOT fallen, 
	 * 		(5a) blur the game depending on the player's radius/angle
	 * 		(5b) if using the mouse, get the mouse position on screen
	 * 		(5c) get the new direction in which the player/head is leaning 
	 * 		(5d) move the player in the new direction gotten in (5c)
	 * 		(5e) play the footstep sound depending on the radius of the player 
	 * 		(5f) rumble when hit
	 * (6) update newPos (head's current position)
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void Update () {

		//If in splash, disable cam
		if (GameManager.ins.status == GameState.GameStatus.Splash){
			cam.enabled = false;
		}


		if (UniMove.GetButtonUp(PSMoveButton.Circle)){
			transform.position = initHead;
			transform.rotation = new Quaternion (0,0,0, transform.rotation.w);
			rhead.angularVelocity = Vector3.zero;
			pfeet.transform.position = new Vector3 (initHead.x, pfeet.transform.position.y, initHead.z);
		}

		// restart level if press R
		if (Input.GetKey("r"))
			Application.LoadLevel (Application.loadedLevel);

		// (1) each move keeps its coloured light on 
		if (!colliding){
			setMoveColour (); 
		}

		// (3) keep head Y position constant
		resetY ();

		if (fallen) {
			// (4) player has fallen
			if (!frozen){
				stopRead ();
			}
			else if (switchViews) {
				switchToFallCam();
			}
			else if (checkTaps){
				tapsToGetUp();
			}
			else if (gettingUp){
				GetUp();
			}
			direction = -1;
		}
		else { // (5) player hasn't fallen  

			transform.LookAt(target, Vector3.up); 	// not used 

			angleBlur ();

			if (controller == (int) controlInput.mouse) 
				mouse = Input.mousePosition;

			// get player's move direction, move the player
			direction = getLeanDirection(); 
			isLeaningTooMuch (); 			
			moveHead (direction); 


			// (5f) rumble when hit
			UniMove.SetRumble (hitRumble);
		}
		// (6)
		newPos = transform.position;
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * FIXED UPDATE
	 * (1) if camLerp (getting back up from falling), lerp the head rotation
	 * (2) if moveFeet, then set the new feet lerp target to directly below the head 
	 * (3) delay getting a new feet position after each new feet lerp position is set (below the head)
	 * (4) play a random grunting sound after a certain delay
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	void FixedUpdate() {

		// (1) cam lerps back when you get up from falling 
		if (camLerp){
			//print ("entered cam lerp");
			transform.rotation = Quaternion.Lerp(transform.rotation, newRot, smooth * Time.deltaTime);
			
		}

		// (2) feet lerping instead of snapping under head position
		if (moveFeet){
			pfeet.transform.position = Vector3.Lerp (pfeet.transform.position, newFeetPos, smooth * Time.deltaTime);
		}
		
		// (3) DELAYING PLACE FEET AT HEAD'S XZ POS
		currentFrame++; 
		if (!fallen){
			if (currentFrame >= delayFrame){
				getNewFeetPos ();	// i.e. get position at feet y, but x/z coordinates of head
				currentFrame = 0;
				moveFeet = true;	// lerp the feet to that new position
			}
		}
		

	}



	// PREVENT THE COLLIDER FROM FLOATING ABOVE OBJECTS
	private void resetY(){
		rhead.MovePosition (new Vector3 (transform.position.x, headY, transform.position.z)); 
	}
	

	/* --------------------------------------------------------------------------------------------------------------------------
	 * ANGLE BLUR
	 * The closer angleBetween is to 30.0f, the blurrier things get!
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void angleBlur (){

		radDiff = maxRad - radius;

		if (radDiff <= (maxRad/2)){
			if (dof.aperture < 10){
				dof.aperture += 0.05f;
			}
		} else if (radDiff > (maxRad/2)){
			if (dof.aperture > 0){
				dof.aperture -= 0.1f;
			}
		}

	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * GET LEAN DIRECTION. RETURN: the integer (corresponding to the direction in the Dir enum)
	 * (1) MOUSE: depending on the position of the mouse on the screen
	 * (2) MOVE: depending on how the player is leaning (move's az = forward/back, move's ax = right/left)
	 * (3) XBOX: depending on the the tilt of the left stick on the controller	
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private int getLeanDirection(){

		// (1) MOUSE 
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
		// (2) MOVE
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
		// (3) XBOX 
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
	 * IS LEANING TOO MUCH
	 * (1) get the current radius between the feet and the head (hypotenuse given the differences in x and z of head and feet)
	 * (2) if the radius is bigger than the max radius, the player falls (set fallen to true)
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void isLeaningTooMuch(){ 

		// (1) get current radius
		float x = Mathf.Abs (pfeet.transform.position.x - transform.position.x);
		float y = Mathf.Abs (pfeet.transform.position.z - transform.position.z); 
		radius = Mathf.Sqrt (x * x + y * y);

		// (2) check if player has fallen (radius is too big)
		if (radius >= maxRad) {
			//print ("player " + id + " is leaning too much -> has fallen");
			fallen = true;
		}
	}


	/* --------------------------------------------------------------------------------------------------------------------------
	 * MOVE HEAD: DEPENDING ON THE LEAN DIRECTION, ADD A FORCE TO THE RIGIDBODY OF THE HEAD
	 * ARG: the direction the player is leaning 
	 * (1) switch/case to check which direction we're leaning
	 * (2) add the force in the appropriate direction
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void moveHead (int direction){
		//Invoke("resetHinc", 1);
		switch (direction) {
			
		case (int) Dir.forward:	
			rhead.AddForce (hinc*transform.forward);
			modelAnim.SetInteger("Direction", 1);
			meAnim.SetInteger ("Dir", 1);
			break;
			
		case (int) Dir.right:
			rhead.AddForce (hinc*transform.right); 
			modelAnim.SetInteger("Direction", 3);
			meAnim.SetInteger ("Dir", 3);
			break;
			
		case (int) Dir.left:
			rhead.AddForce (-hinc*transform.right); 
			modelAnim.SetInteger("Direction", 2);
			meAnim.SetInteger ("Dir", 2);
			break;
			
		case (int) Dir.back:
			rhead.AddForce (-(hinc/2)*transform.forward); 
			modelAnim.SetInteger("Direction", 4);
			meAnim.SetInteger ("Dir", 4);
			break; 
			
		default:
			break; 
		}
	}
	
	/* --------------------------------------------------------------------------------------------------------------------------
	 * GET NEW FEET POS: after a certain delay, set the new target position to lerp the feet (under the head)
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void getNewFeetPos (){

		newFeetPos = new Vector3 (rhead.position.x, pfeet.transform.position.y, rhead.position.z);
		moveFeet = false;

	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * WHAT HAPPENS WHEN YOU FALL - PART 1 
	 * stopRead(): no arg, no return
	 * 		(1) saves the position and rotation of the player before falls, 
	 * 		(2) freezes player movement and rotation
	 * 		(3) increase the number of times the player has fallen
	 * 		(4) if the player has fallen the max number of times allowed, lose the game 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void stopRead(){
		// (2) freeze player rigidbody movement and rotation
		rhead.constraints = RigidbodyConstraints.FreezeAll;

		// (3)-(4), increment number of times player fallen and check if the player has lost
		fallCt++; 
		if (fallCt >= maxFallCt) {
			//GameManager.ins.playerStatus = GameState.PlayerStatus.Lost;
		}

		// (1) save the position/rotation of player before fallen
		fallenPos = transform.position; 
		fallenRot = transform.rotation; 

		//Set Global Player State
		//if (GameManager.ins.playerStatus != GameState.PlayerStatus.Fallen){
		//	GameManager.ins.playerStatus = GameState.PlayerStatus.Fallen;
		//}

		currentFrame = 0; 
		frozen = true;
		switchViews = true;

		//(5) Stop model animator
		//modelAnim.enabled = false;
	}

	private void switchToFallCam(){
		// (2) disable maincam, enable fallcam
		cam.enabled = false;
		fallCam.enabled = true;
		
		// (3) play fallover Anim
		meAnim.SetBool("fallOver", true);
		checkTaps = true;
		switchViews = false;

		// (4) make model fallover
		modelAnim.SetBool ("Falling", true);
		modelAnim.SetBool ("GetUp", false);
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * WHAT HAPPENS WHEN YOU FALL - PART 2: ELECTRIC BOOGALOO
	 * tapsToGetUp(): no arg, no return
	 * 		(1) change the player status in the game manager to fallen
	 * 		(2) switch to the fall camera
	 * 		(3) play the fall over animation
	 * 		(4) stopRead() if hasn't already been stopped (i.e. if frozen is false)
	 * 		(5) check button taps to get up
	 * 		(6) get up if tapped enough buttons or lose if run out of time to tap buttons
	 * -------------------------------------------------------------------------------------------------------------------------- */

	public void tapsToGetUp(){

		// (5) check if tapping trigger
		if (!buttonTapped) {
			if (UniMove.Trigger == 1.0f){
				tapCurrent++;
				lidUp = true;
				buttonTapped = true; 
			}
		}
		else {
			lidUp = false;
			if (UniMove.Trigger == 0.0f){
				buttonTapped = false;
			}
		}

		// (6) get up if tapped enough or lose if not enough and waited too long
		if (tapCurrent >= tapsGetUp) {
			checkTaps = false; 
			gettingUp = true;
		} // LOST
		else if (currentFrame >= frameFall){
			//GameManager.ins.playerStatus = GameState.PlayerStatus.Lost;
		}
	}



	/* --------------------------------------------------------------------------------------------------------------------------
	 * WHAT HAPPENS WHEN YOU FALL - PART 3: REDEMPTION
	 * GetUp(): no arg, no return
	 * 		(1) switch back to main camera
	 * 		(2) reset the head (lerp) and feet (snap) positions 
	 * 		(3) start the backToOrigin coroutine
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void GetUp(){
		
		meAnim.SetBool("fallOver", false);
		meAnim.SetBool("getUp", true);

		modelAnim.SetBool ("Falling", false);
		modelAnim.SetBool ("GetUp", true);
		//if (GameManager.ins.playerStatus == GameState.PlayerStatus.Fallen){
		//	GameManager.ins.playerStatus = GameState.PlayerStatus.Fine;
		//}
		
		if (meAnim.GetCurrentAnimatorStateInfo(0).IsName("Fine")){

			// (1) switch from fall cam to main cam
			cam.enabled = true;
			fallCam.enabled = false;

			// (2) reset the head/feet positions
			newPos = fallenPos;
			pfeet.transform.position = new Vector3 (fallenPos.x, pfeet.transform.position.y, fallenPos.z);
			transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);

			newRot = new Quaternion(0.0f, fallenRot.y, 0.0f, fallenRot.w); 

			StartCoroutine (ResetVariables());
			//Set Global Player State
		}
	}

	IEnumerator ResetVariables(){
		camLerp = true;
		rhead.angularVelocity = new Vector3 (0, 0, 0);
		yield return new WaitForSeconds (1.0f);
		camLerp = false;
		rhead.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
		tapCurrent = 0; 
		fallen = false;
		gettingUp = false;
		frozen = false;
		
		newRot = transform.rotation;

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
			UniMove.SetLED (Color.magenta);
			break;
		case 3:
			UniMove.SetLED (Color.yellow);
			break;
		case 4:
			UniMove.SetLED (Color.green);
			break;
		default:
			break;
		}
	}
}
