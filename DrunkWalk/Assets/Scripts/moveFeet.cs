using UnityEngine;
using System.Collections;

public class moveFeet : MonoBehaviour {

	public Rigidbody rb; 

	public void Start () {
		//rb = gameObject.GetComponent<Rigidbody>(); 
	}

	public void FixedUpdate () {
		//Debug.Log("rb.x " + rb.transform.position.x);
		//Debug.Log("feet.x " + transform.position.x);
		gameObject.transform.position = rb.transform.position; 
	}
}
