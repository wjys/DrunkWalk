using UnityEngine;
using System.Collections.Generic;

public class InGame : MonoBehaviour {

	public bool paused {
		set {
			// pause transform
			Transform trans = GetComponent<Transform>();
			DrunkForce df = GetComponent<DrunkForce>();
			trans = gameObject.transform;
			df.enabled = false;

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
		DrunkForce df = GetComponent<DrunkForce>();
		df.enabled = true;
	}
	public virtual void GameLateUpdate () {
		DrunkForce df = GetComponent<DrunkForce>();
		df.enabled = true;
	}

	private void UpdateComponentStatus<T> (bool paused, ref bool status) {

	}
}