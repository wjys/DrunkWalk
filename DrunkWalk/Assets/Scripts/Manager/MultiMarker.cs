using UnityEngine;
using System.Collections;

// MARKER FOR CHARACTER SELECTION IN MULTIPLAYER MENU

public class MultiMarker : MonoBehaviour {
	public int id;

	// -------- OBJECTS AND COMPONENTS TO GET/RECEIVE
	public UniMoveController UniMove;
	public MainMenu main;

	// -------- PRIVATE VARIABLES

	private enum Dir { right, left, idle };
	private int direction;

	// -------- UNIMOVE BOUNDS (set in inspector)

	public float MoveBoundRight;
	public float MoveBoundLeft;

	// -------- CHARACTER SELECTION STUFF
	private int currentChar;
	private int previousChar;
	private enum characters { multi, zach, ana, anhchi, winnie };

	// -------- MARKER POSITION STUFF
	public float smooth;
	public Transform[] charPositions;
	private float markerY;
	public float[] markerYs;
	
	public Vector3 newPos;		// position to lerp to

	// -------- BOOLS
	private bool switchingCharacters;
	public bool charSelected;

	// --------- SPRITES
	public int spriteID;
	public Sprite[] markerSprites;

	/* --------------------------------------------------------------------------------------------------------------------------
	 * START: 
	 * (1) get the main menu object = need to know which menu we are currently at
	 * (2) GET MARKER HEIGHT: get the height the marker should stay at
	 * (3) set the default character to zach
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void Start () {
		main = GameObject.Find ("MainMenu").GetComponent<MainMenu>();
		getMarkerHeight ();
		currentChar = (int) characters.zach;
		previousChar = (int) characters.zach;
		charSelected = false;

		charPositions = GameObject.Find ("MultiCharacters").GetComponentsInChildren<Transform>();
		transform.position = new Vector3 (charPositions[currentChar].position.x, markerY, charPositions[currentChar].position.z);
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * UPDATE:
	 * (A) if at character selection menu
	 * 		(1) get the tilt of the controller
	 * 		(2) get the character resulting from how the controller is tilted
	 * 		(3) set the position the marker should lerp to
	 * 		(4) delay if we switched character
	 * (B) if at the next menu (level/mode select) => deactivate marker object
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	void Update () {

		// AT MULTI CHARACTER SELECT
		if (main.menuNumPublic == 5) {
			gameObject.GetComponent<SpriteRenderer>().sprite = markerSprites[spriteID];
			if (!charSelected){
				if (!switchingCharacters){
					selectCharacter();
					direction = getMoveTilt ();
					getCharacter (currentChar);
					moveMarker (currentChar);
				}
				else {
					StartCoroutine(snapToPos());
				}
			}
			else {
				unselectCharacter ();
			}
		}

		// AT MULTI MODE/LEVEL SELECT
		else {
			this.gameObject.SetActive (false);
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * FIXED UPDATE: for lerping
	 * -------------------------------------------------------------------------------------------------------------------------- */

	void FixedUpdate(){
		transform.position = Vector3.Lerp (transform.position, newPos, smooth*Time.deltaTime);
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * GET MOVE TILT: depending on how the player is tilting the controller, return left, right, or idle (not tilting)
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private int getMoveTilt(){

		if (UniMove.ax < MoveBoundRight) {
			print ("tilting right");
			return (int) Dir.right; 
		}
		if (UniMove.ax > MoveBoundLeft) {
			print ("tilting left");
			return (int) Dir.left; 
		}
		return (int) Dir.idle;
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * UNSELECT CHARACTER: if player hits x, square, triangle, circle buttons => cancel their current char selection
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void unselectCharacter(){
		bool cross 		= UniMove.GetButtonUp (PSMoveButton.Cross);
		bool square 	= UniMove.GetButtonUp (PSMoveButton.Square);
		bool triangle 	= UniMove.GetButtonUp (PSMoveButton.Triangle);
		bool circle 	= UniMove.GetButtonUp (PSMoveButton.Circle);

		if (cross || square || triangle || circle){
			charSelected = false;
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * SELECT CHARACTER: if player hits move button => confirm their char selection
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void selectCharacter(){
		if (UniMove.GetButtonUp(PSMoveButton.Move)){
			charSelected = true;
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * GET CHARACTER: depending on tilt of the controller and which character the marker is above, switch current character
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void getCharacter(int current){

		if (direction == (int) Dir.right){
			switch (current){
			case (int) characters.zach:
				currentChar = (int) characters.ana;
				break;
			case (int) characters.ana:
				currentChar = (int) characters.anhchi;
				break;
			case (int) characters.anhchi:
				currentChar = (int) characters.winnie;
				break;
			case (int) characters.winnie:
				currentChar = (int) characters.zach;
				break;
			default:
				currentChar = current;
				break;
			}
		}
		if (direction == (int) Dir.left){
			switch (current){
			case (int) characters.zach: 
				currentChar = (int) characters.winnie;
				break;
			case (int) characters.ana:
				currentChar = (int) characters.zach;
				break;
			case (int) characters.anhchi:
				currentChar = (int) characters.ana;
				break;
			case (int) characters.winnie:
				currentChar = (int) characters.anhchi;
				break;
			default:
				currentChar = current;
				break;
			}
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * MOVE MARKER: depending on the current character, set the new position the marker should lerp to
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void moveMarker(int current){
		if (previousChar != currentChar){
			newPos = new Vector3 (charPositions[current].position.x, markerY, charPositions[current].position.z);
			previousChar = currentChar;
			switchingCharacters = true;
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * SNAP TO POS: delay to allow marker to lerp and snap to next position
	 * -------------------------------------------------------------------------------------------------------------------------- */

	IEnumerator snapToPos(){
		yield return new WaitForSeconds(0.5f);
		switchingCharacters = false;
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * GET MARKER HEIGHT: depending on the player id, set the height at which the marker should stay
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private void getMarkerHeight(){
		switch (id){
		case 1:
			markerY = markerYs[0];
			return;
		case 2:
			markerY = markerYs[1];
			return;
		case 3:
			markerY = markerYs[2];
			return;
		case 4:
			markerY = markerYs[3];
			return;
		default:
			return;
		}
	}
}
