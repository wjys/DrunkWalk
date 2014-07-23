﻿/**
 * UniMove API - A Unity plugin for the PlayStation Move motion controller
 * Copyright (C) 2012, 2013, Copenhagen Game Collective (http://www.cphgc.org)
 * 					         Patrick Jarnfelt
 * 					         Douglas Wilson (http://www.doougle.net)
 * 
 * 
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *    1. Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *
 *    2. Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 **/

using UnityEngine;
using System;
using System.Collections.Generic;

public class UniMoveManager : MonoBehaviour 
{
	// We save a list of Move controllers.
	public List<UniMoveController> moves = new List<UniMoveController>();
	public GameObject[] players;
	public int moveCount; 
	
	void Start() 
	{
		moveCount = 0; 
		UniMoveInit (); 
	}
	
	
	void Update() 
	{
		UniMoveSetID ();
		UniMoveSetPlayers ();
		UniMoveActivateComponents ();
	}
	
	void HandleControllerDisconnected (object sender, EventArgs e)
	{
		// TODO: Remove this disconnected controller from the list and maybe give an update to the player
	}

	private void UniMoveInit(){
		Time.maximumDeltaTime = 0.1f;
		
		int count = UniMoveController.GetNumConnected();
		
		// Iterate through all connections (USB and Bluetooth)
		for (int i = 0; i < count; i++) 
		{	
			UniMoveController move = gameObject.AddComponent<UniMoveController>();	// It's a MonoBehaviour, so we can't just call a constructor
			/*move.id = i+1; 
			foreach (GameObject player in players){
				DrunkMovement dm = player.GetComponent<DrunkMovement>();
				if (dm.id == move.id){
					move = player.AddComponent<UniMoveController>(); 
				}
			}*/
			// Remember to initialize!
			if (!move.Init(i)) 
			{	
				Destroy(move);	// If it failed to initialize, destroy and continue on
				continue;
			}
			
			// This example program only uses Bluetooth-connected controllers
			PSMoveConnectionType conn = move.ConnectionType;
			if (conn == PSMoveConnectionType.Unknown || conn == PSMoveConnectionType.USB) 
			{
				Destroy(move);
			}
			else 
			{
				moves.Add(move);
				
				move.OnControllerDisconnected += HandleControllerDisconnected;

			}
		}
	}

	private void UniMoveButtons(){
		foreach (UniMoveController move in moves) 
		{
			// Instead of this somewhat kludge-y check, we'd probably want to remove/destroy
			// the now-defunct controller in the disconnected event handler below.
			if (move.Disconnected) continue;
			
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
	}
	private void UniMoveSetID(){
		foreach (UniMoveController move in moves){
			
			if (move.GetButtonDown(PSMoveButton.Move)){
				switch (moveCount){
				case 0:
					move.SetLED (Color.cyan);
					move.id = 1;
					moveCount++;
					break;
				case 1:
					move.SetLED (Color.red);
					move.id = 2;
					moveCount++;
					break;
				case 2:
					move.SetLED (Color.yellow);
					move.id = 3;
					moveCount++;
					break;
				case 3:
					move.SetLED (Color.magenta);
					move.id = 4;
					moveCount++; 
					break;
				default:
					break;
				}
			}
		}
	}

	private void UniMoveSetPlayers(){ 
		for(int i = 0; i < moves.Count; i++){
			UniMoveController mv; 
			if (moves[i].id > 0){
				foreach (GameObject player in players){
					mv = player.GetComponent<UniMoveController>();
					if (mv == null){
						UniMoveDisplay display = player.GetComponent<UniMoveDisplay>();
						if (moves[i].id == display.id){
							moves[i] = player.AddComponent<UniMoveController>() as UniMoveController;
							moves[i].Init (i);
						}
						break;
					}
				}
			}
		}
	}

	private void UniMoveActivateComponents(){
		UniMoveController mv; 
		foreach (GameObject player in players) {
			mv = player.GetComponent<UniMoveController>();
			if (mv != null){
				DrunkMovement dm 	= player.GetComponent<DrunkMovement>(); 
				Rotation rot 		= player.GetComponent<Rotation>();
				DrunkForce df 		= player.GetComponent<DrunkForce>();
				Collision col 		= player.GetComponent<Collision>();

				dm.enabled 	= true;
				rot.enabled = true;
				df.enabled	= true;
				col.enabled = true;
			}
		}
	}
}
