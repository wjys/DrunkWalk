using UnityEngine;
using System.Collections.Generic;

public class InGame : MonoBehaviour {
	public bool paused {
		set {
			// pause animator component if possible
			Animator animator = GetComponent<Animator>();
			if (animator != null) {
				if (value) {
					animatorEnabledStatus = animator.enabled;
					animator.enabled = false;
				} else {
					animator.enabled = animatorEnabledStatus;
				}
			}
			// pause animation component if possible
			Animation animation = GetComponentInChildren<Animation>();
			if (animation != null) {
				if (value) {
					foreach (AnimationState st in animation) {
						animationSpeedStatus[st] = st.speed;
						st.speed = 0.0f;
					}
				} else {
					foreach (AnimationState st in animation) {
						st.speed = animationSpeedStatus[st];
					}
				}
			}

			_paused = value;
		}
		get { return _paused; }
	}
	private bool _paused = false;
	private bool animatorEnabledStatus;
	private Dictionary<AnimationState, float> animationSpeedStatus = new Dictionary<AnimationState, float>();
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