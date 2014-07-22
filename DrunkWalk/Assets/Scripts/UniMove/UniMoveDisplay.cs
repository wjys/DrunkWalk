using UnityEngine;
using System;
using System.Collections.Generic;

public class UniMoveDisplay : MonoBehaviour 
{
	// We save the Move controller associated to the player
	UniMoveController move = new UniMoveController();
	public int id; 
	private string display = "";
	
	void Start() 
	{
		/* NOTE! We recommend that you limit the maximum frequency between frames.
		 * This is because the controllers use Update() and not FixedUpdate(),
		 * and yet need to update often enough to respond sufficiently fast.
		 * Unity advises to keep this value "between 1/10th and 1/3th of a second."
		 * However, even 100 milliseconds could seem slightly sluggish, so you
		 * might want to experiment w/ reducing this value even more.
		 * Obviously, this should only be relevant in case your framerare is starting
		 * to lag. Most of the time, Update() should be called very regularly.
		 */
		Time.maximumDeltaTime = 0.1f;
		
		int count = UniMoveController.GetNumConnected();

		move = gameObject.AddComponent<UniMoveController> (); 

		if (!move.Init(id)) 
		{	
			display = "No Bluetooth-connected controllers found. Make sure one or more are both paired and connected to this computer.";
			//Destroy(move);	// If it failed to initialize, destroy and continue on
		}

		PSMoveConnectionType conn = move.ConnectionType;
		if (conn == PSMoveConnectionType.Unknown || conn == PSMoveConnectionType.USB) 
		{
			display = "No Bluetooth-connected controllers found. Make sure one or more are both paired and connected to this computer.";
			//Destroy(move);
		}
		else 
		{
			move.OnControllerDisconnected += HandleControllerDisconnected;
			
			// Start all controllers with a white LED
			move.SetLED(Color.white);
		}
	}
	
	
	void Update() 
	{		
		// Button events. Works like Unity's Input.GetButton
		if (move.GetButtonDown(PSMoveButton.Circle)){
			Debug.Log("Circle Down");
		}
		if (move.GetButtonUp(PSMoveButton.Circle)){
			Debug.Log("Circle UP");
		}
		
		// Change the colors of the LEDs based on which button has just been pressed:
		if (move.GetButtonDown(PSMoveButton.Circle)) 		move.SetLED(Color.cyan);
		else if(move.GetButtonDown(PSMoveButton.Cross)) 	move.SetLED(Color.red);
		else if(move.GetButtonDown(PSMoveButton.Square)) 	move.SetLED(Color.yellow);
		else if(move.GetButtonDown(PSMoveButton.Triangle)) 	move.SetLED(Color.magenta);
		else if(move.GetButtonDown(PSMoveButton.Move)) 		move.SetLED(Color.black);
		
		// Set the rumble based on how much the trigger is down
		move.SetRumble(move.Trigger);
	}
	
	void HandleControllerDisconnected (object sender, EventArgs e)
	{
		// TODO: Remove this disconnected controller from the list and maybe give an update to the player
	}
	
	void OnGUI() 
	{
		if (!move.Init (id) || move.Disconnected) {
			display = "No Bluetooth-connected controllers found. Make sure one or more are both paired and connected to this computer.";
		}
		else {
			display += string.Format(	"Player {0}: \n" +
										"ax:{1:0.000}, " + "ay:{2:0.000}, " + "az:{3:0.000}, \n" +
										"gx:{4:0.000}, " + "gy:{5:0.000}, " + "gz:{6:0.000} \n", 
				                         id, move.Acceleration.x, move.Acceleration.y, move.Acceleration.z,
				                         move.Gyro.x, move.Gyro.y, move.Gyro.z);
		}
		GUI.Label(new Rect(10, Screen.height-100, 500, 100), display);
	}
}
