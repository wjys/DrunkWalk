using UnityEngine;
using System.Collections;

// PUT THIS ON EACH UICAM

public class Sounds : MonoBehaviour {

	public DrunkMovement dm;
	public Collision col;
	public Eyelids eyelids;
	public Ouch ouch;

	public int charIndex;
	public enum character {zach, ana, acb, winnie};

	public int clipType;
	public enum ctype { grunts, objects, furniture, wall, drowsy, fall, struggle, getup, giveup, bed };

	// 1st index: 0-grunts, 1-objects, 2-furniture, 3-drowsy, 4-fall, 5-struggle, 6-getup, 7-giveup, 8-bed
	// 2nd index: the actual clips
	public AudioClip[][] clips;
	public AudioClip steps;

	public AudioClip[] zach_grunts;
	public AudioClip[] zach_objects;
	public AudioClip[] zach_furniture;
	public AudioClip[] zach_wall;
	public AudioClip[] zach_drowsy;
	public AudioClip[] zach_fall;
	public AudioClip[] zach_struggle;
	public AudioClip[] zach_getup;
	public AudioClip[] zach_giveup;
	public AudioClip[] zach_bed;

	public AudioClip[] ana_grunts;
	public AudioClip[] ana_objects;
	public AudioClip[] ana_furniture;
	public AudioClip[] ana_wall;
	public AudioClip[] ana_drowsy;
	public AudioClip[] ana_fall;
	public AudioClip[] ana_struggle;
	public AudioClip[] ana_getup;
	public AudioClip[] ana_giveup;
	public AudioClip[] ana_bed;

	public AudioClip[] acb_grunts;
	public AudioClip[] acb_objects;
	public AudioClip[] acb_furniture;
	public AudioClip[] acb_wall;
	public AudioClip[] acb_drowsy;
	public AudioClip[] acb_fall;
	public AudioClip[] acb_struggle;
	public AudioClip[] acb_getup;
	public AudioClip[] acb_giveup;
	public AudioClip[] acb_bed;

	public AudioClip[] winnie_grunts;
	public AudioClip[] winnie_objects;
	public AudioClip[] winnie_furniture;
	public AudioClip[] winnie_wall;
	public AudioClip[] winnie_drowsy;
	public AudioClip[] winnie_fall;
	public AudioClip[] winnie_struggle;
	public AudioClip[] winnie_getup;
	public AudioClip[] winnie_giveup;
	public AudioClip[] winnie_bed;

	public bool soundPlayed;
	public bool stepped;

	public bool objectCollision;
	public bool furnitureCollision;
	public bool wallCollision;

	public bool falling;
	public bool struggling; 
	public bool gettingUp;

	public float soundDelay;
	public float maxStepDelay;
	
	void Start () {
		soundPlayed = false;
	}

	void Update () {
		// lose
		if (!eyelids.enabled){
			giveupSounds();
			this.enabled = false;
		}
		// win
		if (!ouch.enabled){
			bedSounds ();
			this.enabled = false;
		}
		// in game
		if (!dm.fallen){
			if (gettingUp){
				getupSounds();
				gettingUp = false;
			}
			else if (col.colliding){
				audio.Stop();
				if (objectCollision){
					objectCollSounds();
					objectCollision = false;
				}
				if (furnitureCollision){
					furnitureCollSounds();
					furnitureCollision = false;
				}
				if (wallCollision){
					wallCollSounds();
					wallCollision = false;
				}
			}
			else {
				if (dm.radius/dm.maxRad < 0.5f){
					if (!soundPlayed){
						playSound (clips[(int) ctype.grunts][Random.Range (0, clips[(int) ctype.grunts].Length)]);
						StartCoroutine (resumeSound());
					}
				}
				else {
					if (!soundPlayed){
						drowsySounds();
						StartCoroutine (resumeSound ());
					}
				}
				if (!stepped){
					playSound (steps);
					StartCoroutine (resumeSteps());
				}
			}
		}
		else {
			if (falling){
				audio.Stop();
				fallSounds();
				StartCoroutine (resumeStruggle());
			}
			else if (struggling){
				struggleSounds ();
				struggling = false;
			}

		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * PLAY SELECTED SOUND
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void playSound(AudioClip clip){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clip); 
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * RESUME GRUNT
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	IEnumerator resumeSound() {
		soundPlayed = true;
		yield return new WaitForSeconds (soundDelay);
		soundPlayed = false;
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * RESUME STEPS
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	IEnumerator resumeSteps() {
		stepped = true;
		yield return new WaitForSeconds (maxStepDelay*(dm.radius/dm.maxRad));
		stepped = false;
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * RESUME STRUGGLE
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	IEnumerator resumeStruggle() {
		falling = false;
		yield return new WaitForSeconds (soundDelay);
		struggling = true;
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * COLLISION INTO OBJECT
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void objectCollSounds(){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clips[(int) ctype.objects][Random.Range (0, clips[(int) ctype.objects].Length)]); 
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * COLLISION INTO FURNITURE
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void furnitureCollSounds(){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clips[(int) ctype.furniture][Random.Range (0, clips[(int) ctype.furniture].Length)]); 
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * COLLISION INTO WALL
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void wallCollSounds(){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clips[(int) ctype.wall][Random.Range (0, clips[(int) ctype.wall].Length)]); 
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * DROWSY
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void drowsySounds(){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clips[(int) ctype.drowsy][Random.Range (0, clips[(int) ctype.drowsy].Length)]); 
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * FALL
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void fallSounds(){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clips[(int) ctype.fall][Random.Range (0, clips[(int) ctype.fall].Length)]); 
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * STRUGGLE ON THE FLOOR
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void struggleSounds(){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clips[(int) ctype.struggle][Random.Range (0, clips[(int) ctype.struggle].Length)]); 
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * GET UP FROM FLOOR (SUCCESS IN TAPPING TRIGGER)
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void getupSounds(){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clips[(int) ctype.getup][Random.Range (0, clips[(int) ctype.getup].Length)]); 
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * GIVE UP ON FLOOR (FAILED TO TAP TRIGGER)
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void giveupSounds(){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clips[(int) ctype.giveup][Random.Range (0, clips[(int) ctype.giveup].Length)]); 
		
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * FALLING INTO BED SOUNDS
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void bedSounds(){
		
		audio.pitch = Random.value * 0.1f + 0.95f;
		audio.volume = Random.value * 0.3f + 0.7f;
		audio.PlayOneShot(clips[(int) ctype.bed][Random.Range (0, clips[(int) ctype.bed].Length)]); 
		
	}
}
