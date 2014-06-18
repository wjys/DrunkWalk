using UnityEngine;
using System.Collections;

public class Collision : MonoBehaviour {

	//Overall Score
	public /*static*/ int score;

	//Hurt Sound
	public GameObject ouch;
	public Animator ouchAnim;

	private static bool yelling = false;

	// Use this for initialization
	void Start () {
		score = 10000;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//When colliding with something:
	void OnTriggerEnter(Collider col) {
		Debug.Log("Collision");
		ouchAnim.SetTrigger("Ouch");
		
		//If collision is against a wall:
		if (col.tag == "Wall") {
			score -= 100;
			Debug.Log("Wall Collision - " + score);
		}
		else if (col.tag == "Box"){
			score -= 200;
			Debug.Log("Box Collision - " + score);
		}
		else if (col.tag == "Cabinet"){
			score -= 200;
			Debug.Log("Cabinet Collision - " + score);
		}
		else if (col.tag == "Cat"){
			score -= 250;
			Debug.Log("Cat Collision - " + score);
		}
		else if (col.tag == "Table"){
			score -= 300;
			Debug.Log("Table Collision - " + score);
		}
		else if (col.tag == "Bed"){
			Application.LoadLevel (Application.loadedLevel); 
		}
/*		else if (col.tag == "Floor"){
			score -= 500;
			Debug.Log("Floor Collision - " + score);
		}*/

		//If not currently yelling:
		if (yelling == false){
			Yell();
		} else if (yelling == true){
			StopAllCoroutines();
		}

	}
	//When not:
	void OnTriggerExit(Collider col) {
		Debug.Log("No Longer Colliding");
	}

	//Instantiate a hurt sound
	public void Yell () {
		GameObject newOuch = Instantiate(ouch, transform.position + (0.8f * Vector3.up) + (2.5f * Vector3.forward) + (0.5f * Vector3.right), Quaternion.identity) as GameObject;
		this.StartCoroutine(this.DestroyYell(newOuch, 1.0f));
	}

	//Destroy the hurt sound
	IEnumerator DestroyYell(GameObject _yell, float delay) {
		yield return new WaitForSeconds(delay);
		Destroy(_yell);
		yelling = true;
	}
}
