using UnityEngine;
using System.Collections;

public class moveFeet : MonoBehaviour {

	private Rigidbody rb; 

	void Start () {
		rb = GetComponent <Rigidbody> (); 
	}

	void Update () {
		transform.position = rb.position; 
	}
}
