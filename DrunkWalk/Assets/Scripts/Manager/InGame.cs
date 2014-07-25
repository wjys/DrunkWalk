using UnityEngine;
using System.Collections.Generic;

public class InGame : MonoBehaviour {

	public bool paused {
		set {
			Eyelids eye = GetComponent<Eyelids>();
			DrunkForce df = GetComponent<DrunkForce>();
			eye.pausing = true;
			df.enabled = false;
			rigidbody.isKinematic = true;

			_paused = value;
		}
		get { return _paused; }
	}
	private bool _paused = false;

	void Update () {
		if (!paused) {
			GameUpdate();
		}
	}
	void LateUpdate () {
		if (!paused) {
			GameLateUpdate();
		}
	}
	public virtual void GameUpdate () {
		Eyelids eye = GetComponent<Eyelids>();
		DrunkForce df = GetComponent<DrunkForce>();
		df.enabled = true;
		eye.pausing = false;
		rigidbody.isKinematic = false;
	}
	
	public virtual void GameLateUpdate () {
		Eyelids eye = GetComponent<Eyelids>();
		DrunkForce df = GetComponent<DrunkForce>();
		df.enabled = true;
		eye.pausing = false;
		rigidbody.isKinematic = false;
	}

	private void UpdateComponentStatus<T> (bool paused, ref bool status) {

	}
}