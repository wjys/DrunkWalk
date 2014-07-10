using UnityEngine;
using System.Collections.Generic;

public class InGame : MonoBehaviour {

	public bool paused {
		set {
			DrunkForce df = GetComponent<DrunkForce>();
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
		DrunkForce df = GetComponent<DrunkForce>();
		df.enabled = true;
		rigidbody.isKinematic = false;
	}
	
	public virtual void GameLateUpdate () {
		DrunkForce df = GetComponent<DrunkForce>();
		df.enabled = true;
		rigidbody.isKinematic = false;
	}

	private void UpdateComponentStatus<T> (bool paused, ref bool status) {

	}
}