using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	static public GameManager ins;

	public GUISkin skin;
	public GameObject pauseMenu;
	public GameObject audioManager;
	private GameObject pauseMenuIns;
	private float lastRealtimeSinceStartup;

	private bool paused = false;

	void Awake () {
		ins = this;

		Screen.showCursor = false;

		if (AudioManager.ins == null){
			(Instantiate(audioManager) as GameObject).SendMessage("Initialize");
		}

		pauseMenuIns = Instantiate(pauseMenu) as GameObject;
		pauseMenuIns.SetActive(false);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		 if (Input.GetKeyDown("p")){
            if (!paused) {
                Pause();
            } else {
                UnPause();
            }
        }
	}

	//Pause Menu Initialize
	public void Pause () {
		InGame[] objs = FindObjectsOfType(typeof(InGame)) as InGame[];
		foreach (var obj in objs) {
			obj.paused = true;
		}

		paused = true;
		pauseMenuIns.SetActive(paused);

	}

	public void UnPause () {
		InGame[] objs = FindObjectsOfType(typeof(InGame)) as InGame[];
		foreach (var obj in objs) {
			obj.paused = false;
		}

		paused = false;
		pauseMenuIns.SetActive(paused);

	}
}