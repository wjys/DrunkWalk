using UnityEngine;
using System.Collections.Generic;

public class InGame : MonoBehaviour {

	public bool paused {
		set {
			// pause transform
			Transform trans = GetComponent<Transform>();
			TopplingForce tf = GetComponent<TopplingForce>();
			trans = gameObject.transform;
			tf.enabled = false; 
			
			// pause child transform if relevant
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
	public virtual void GameUpdate () {
		TopplingForce tf = GetComponent<TopplingForce>();
		tf.enabled = true;
	}
	public virtual void GameLateUpdate () {
		TopplingForce tf = GetComponent<TopplingForce>();
		tf.enabled = true;
	}

	private void UpdateComponentStatus<T> (bool paused, ref bool status) {

	}
}