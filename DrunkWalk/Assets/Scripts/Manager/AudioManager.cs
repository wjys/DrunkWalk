using UnityEngine;

public class AudioManager : MonoBehaviour {
	static public AudioManager ins;

	public void Initialize () {
		if (ins != null) Debug.LogError("Multiple Audio Managers!!");
		ins = this;
		DontDestroyOnLoad(this);
	}
}