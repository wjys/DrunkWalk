using UnityEngine;
using System.Collections;

public class Collision : MonoBehaviour {

	public GameObject ouch;
	public Animator ouchAnim;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col) {
		Debug.Log("Collision");
		ouchAnim.SetTrigger("Ouch");
		Yell();
		if (col.tag == "Wall") {
			Debug.Log("Wall Collision");
		}

	}

	void OnTriggerExit(Collider col) {
		Debug.Log("ExitCollision");
		if (col.tag == "Wall") {
			Debug.Log("Exit Wall Collision");
		}
	}

	public void Yell () {
		Debug.Log("Instantiate Yell");
		GameObject newOuch = Instantiate(ouch, transform.position + (0.8f * Vector3.up) + (2.5f * Vector3.forward) + (0.5f * Vector3.right), Quaternion.identity) as GameObject;
		this.StartCoroutine(this.DestroyYell(newOuch, 1.0f));
	}

	IEnumerator DestroyYell(GameObject _yell, float delay) {
		yield return new WaitForSeconds(delay);
		Destroy(_yell);
	}
}
