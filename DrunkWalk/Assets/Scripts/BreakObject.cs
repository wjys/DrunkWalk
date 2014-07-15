using UnityEngine;
using System.Collections;

// ONLY ATTACHED TO OBJECTS THAT WILL SHATTER (FRAMES, VASES, ETC.) 
// shatter on collision with the floor 

public class BreakObject : MonoBehaviour {

	public Transform brokenVase; 
	public float shatterVelocity; 
	public float shatterRadius;
	public float shatterForce; 

	void OnCollisionEnter (Collision collision) {
		if (collision.relativeVelocity.magnitude > shatterVelocity) { return; } // why is this syntax wrong?
		else {
			Destroy (gameObject); 
			GameObject broken = Instantiate(brokenVase, transform.position, transform.rotation);
			Rigidbody[] pieces = broken.GetComponentsInChildren<Rigidbody>(); 
			Vector3 shatterPos = transform.position; 

			foreach (Rigidbody piece in pieces){
				piece.AddExplosionForce (shatterForce, shatterPos, shatterRadius);
				piece.velocity = rigidbody.velocity;
				piece.angularVelocity = rigidbody.angularVelocity; 
			}
		}
	}

	void OnTriggerEnter (Collider col){
		if (col.tag == "Floor"){
			GameObject broken = Instantiate(brokenVase, transform.position, transform.rotation);
			Rigidbody[] pieces = broken.GetComponentsInChildren<Rigidbody>(); 
			Vector3 shatterPos = transform.position; 
			
			foreach (Rigidbody piece in pieces){
				piece.AddExplosionForce (shatterForce, shatterPos, shatterRadius);
				piece.velocity = rigidbody.velocity;
				piece.angularVelocity = rigidbody.angularVelocity; 
			}
		}
	}
}
