using UnityEngine;
using System.Collections;

public class Eyelids : InGame {

	//World Objects
	public GameObject topLids;
	public GameObject bottomLids;
	public GameObject BR, BL, TR, TL;
	public DrunkMovement me;

	//Blink Counter
	public int blinkCnt;

	//GuiText
	public GUIText Tap;
	
	//Starting Position
	private Vector3 startPosUp;
	private Vector3 startPosDown;

	//Starting Rotation
	private Quaternion startRotBR;
	private Quaternion startRotBL;
	private Quaternion startRotTR;
	private Quaternion startRotTL;

	//Variables of closing and opening speed
	public float smooth;
	public float accel;
	public float wakeUp;
	public float speed;

	//Keep the original variables
	private static float sSpeed;
	private static float sAccel;
	private static float sWakeUp;
	
	private static bool gettingUp;
	private static bool blinked;

	public bool pausing;

	// Use this for initialization
	void Start () {
		startPosUp = topLids.transform.position;
		startPosDown = bottomLids.transform.position;

		startRotBR = BR.transform.rotation;
		startRotBL = BL.transform.rotation;
		startRotTR = TR.transform.rotation;
		startRotTL = TL.transform.rotation;

		sSpeed = speed;
		sAccel = accel;
		sWakeUp = wakeUp;

		pausing = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (pausing == false){
		if (gettingUp == true){
			speed = sSpeed;
			accel = sAccel;
			wakeUp = sWakeUp;
		
		} else {
			//Player Fell
			if (GameManager.ins.playerStatus == GameState.PlayerStatus.Fallen){
			
			//Enable gui
				Tap.enabled = true;

			//Drooping gets faster
			speed += accel;

			//Eyelids falling
			topLids.transform.position = new Vector3 (topLids.transform.position.x, topLids.transform.position.y - speed, topLids.transform.position.z);
			bottomLids.transform.position = new Vector3 (bottomLids.transform.position.x, bottomLids.transform.position.y + speed, bottomLids.transform.position.z);
			
			//Curlout
			if (BR.transform.rotation.z > 0){
				BR.transform.rotation = new Quaternion (BR.transform.rotation.x, BR.transform.rotation.y, BR.transform.rotation.z - (speed * 0.01f), BR.transform.rotation.w);
				BL.transform.rotation = new Quaternion (BL.transform.rotation.x, BL.transform.rotation.y, BL.transform.rotation.z + (speed * 0.01f), BR.transform.rotation.w);
			}
			
			if (TR.transform.rotation.z < 0){
				TR.transform.rotation = new Quaternion (TR.transform.rotation.x, TR.transform.rotation.y, TR.transform.rotation.z + (speed * 0.01f), TR.transform.rotation.w);
				TL.transform.rotation = new Quaternion (TL.transform.rotation.x, TL.transform.rotation.y, TL.transform.rotation.z - (speed * 0.01f), TR.transform.rotation.w);
				}
			

			//Everytime you tap, eyelids flicker
			if (me.buttonTapped){
				topLids.transform.position = new Vector3 (topLids.transform.position.x, topLids.transform.position.y + wakeUp, topLids.transform.position.z);
				bottomLids.transform.position = new Vector3 (bottomLids.transform.position.x, bottomLids.transform.position.y - wakeUp, bottomLids.transform.position.z);

				BR.transform.rotation = new Quaternion (BR.transform.rotation.x, BR.transform.rotation.y, BR.transform.rotation.z + (wakeUp * 0.01f), BR.transform.rotation.w);
				BL.transform.rotation = new Quaternion (BL.transform.rotation.x, BL.transform.rotation.y, BL.transform.rotation.z - (wakeUp * 0.01f), BL.transform.rotation.w);
				TR.transform.rotation = new Quaternion (TR.transform.rotation.x, TR.transform.rotation.y, TR.transform.rotation.z - (wakeUp * 0.01f), TR.transform.rotation.w);
				TL.transform.rotation = new Quaternion (TL.transform.rotation.x, TL.transform.rotation.y, TL.transform.rotation.z + (wakeUp * 0.01f), TL.transform.rotation.w);
				accel += 0.0001f;
			}
			
			//If blinked 3 times, pass out.
			if (topLids.transform.position.y <= (startPosUp.y-(7+blinkCnt)) && blinkCnt < 3){
				blinkCnt += 1;
				blinked = true;
			}
			
			if (blinkCnt >=3){
				GameManager.ins.playerStatus = GameState.PlayerStatus.Lost;
			}

			if (topLids.transform.position.y >= (startPosUp.y-(3*blinkCnt))){
					blinked = false;
			}

			//newPosUp = new Vector3 (startPosUp.x, -100, startPosUp.z);
		} else if (GameManager.ins.playerStatus == GameState.PlayerStatus.Fine){
			gettingUp = true;
				Tap.enabled = false;
		}
		}
		}
	}

	void FixedUpdate() {
		if (gettingUp){
			speed = sSpeed;
			accel = sAccel;
			wakeUp = sWakeUp;

			//blinkCnt = 0;

			lidCurl();

			gettingUp = false;
		}

		if (blinked){

			speed = sSpeed*(me.fallCt) + (0.0005f * blinkCnt);
			accel = sAccel*(me.fallCt) + (0.0005f * blinkCnt);
			wakeUp = sWakeUp*(me.fallCt) - (0.2f * blinkCnt);

			lidCurl ();
		}

	}

	public void lidCurl(){

		topLids.transform.position = Vector3.Lerp(topLids.transform.position, startPosUp, smooth * Time.deltaTime);
		bottomLids.transform.position = Vector3.Lerp(bottomLids.transform.position, startPosDown, smooth * Time.deltaTime);

		BR.transform.rotation = Quaternion.Lerp (BR.transform.rotation, startRotBR, smooth * Time.deltaTime);
		BL.transform.rotation = Quaternion.Lerp (BL.transform.rotation, startRotBL, smooth * Time.deltaTime);
		TR.transform.rotation = Quaternion.Lerp (TR.transform.rotation, startRotTR, smooth * Time.deltaTime);
		TL.transform.rotation = Quaternion.Lerp (TL.transform.rotation, startRotTL, smooth * Time.deltaTime);
	}
}
