using UnityEngine;

public class AudioManager : MonoBehaviour {
	static public AudioManager ins;

	public void Initialize () {
		if (ins != null) Debug.LogError("Multiple Audio Manager!!");
		ins = this;
		DontDestroyOnLoad(this);
	}
}