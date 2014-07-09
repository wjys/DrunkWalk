using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReadMovement : InGame {

	public bool useMouse; 

	// FORCE INCREMENTS FOR HEAD AND FEET
	public float hinc = 0.5f;	
	public float finc = 0.5f; 	

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
	public Camera cam; 			// to force the camera to just fall over if leaning too much
	public Camera fallCam; 		// cam to switch to if lose the game
	public DepthOfFieldScatter dof; // depth of field component on cam


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
	public float soundDelay; 
	private bool soundPlayed; 
	
	// GET RBs' Y COORDS SO THAT THE PLAYER DOESN'T FLOAT OVER BED
	private float headY;



	// Use this for initialization
	void Start () {
		rfeet.MovePosition(new Vector3(transform.position.x, rfeet.position.y, transform.position.z));
		
		fallen = false;
		
		soundPlayed = false; 
		
		headY = transform.position.y; 

		//Mouse Start
			halfWidth = Screen.width / 2; 
			halfHeight = Screen.height / 2; 

		//PSMove Start
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
	
	// Update is called once per frame
	void Update () {
	
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
