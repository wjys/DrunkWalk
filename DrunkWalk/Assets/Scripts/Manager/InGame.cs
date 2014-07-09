﻿using UnityEngine;
using System.Collections.Generic;

public class InGame : MonoBehaviour {

	public bool paused {
		set {
			// pause animator component if possible
			Transform trans = GetComponent<Transform>();
			TopplingForce tf = GetComponent<TopplingForce>();
			trans = gameObject.transform;
			tf.enabled = false; 
			
			// pause animation component if possible
			Transform transChild = GetComponentInChildren<Transform>();
			transChild = transform;

			tf.enabled = false; 

			_paused = value;
		}
		get { return _paused; }
	}
	private bool _paused = false;
	void Update () {
		if (!paused) GameUpdate();
	}
	void LateUpdate () {
		if (!paused) GameLateUpdate();
	}
	public virtual void GameUpdate () {}
	public virtual void GameLateUpdate () {}

	private void UpdateComponentStatus<T> (bool paused, ref bool status) {

	}
}