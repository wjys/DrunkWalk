using UnityEngine;
using System.Collections;

// ADDITIONAL DRUNK FORCE (NOT PLAYER MOVEMENT) 

public class TopplingForce : MonoBehaviour {

	public Vector3 toppleDir;	// currently toppling in this direction
	public Vector3 detectDir;	// constantly update to find current direction of lean
	public bool isToppling;		// true if currently toppling in direction

	public float initInc; 		// base amount to increment 
	public float inc;			// amount by which we increment/multiple the toppling force

	public Rigidbody rbody; 	// object's rigidbody
	public ConstantForce constantForce;	// force on object's rigidbody
	
	void Start () {
		rbody = GetComponent <Rigidbody> ();
		constantForce = GetComponent <ConstantForce> (); 
		isToppling = false; 
	}

	void Update () {

		/* ---------------------------------------------------------------------------------------
		 * PSEUDOCODE
		 * 
		 * if leaning in different direction 
		 * 		not toppling anymore: stop constant force in that direction
		 * 	
		 * else (leaning in same direction) 
		 * 		increment constant force in current direction
		 * 
		 * --------------------------------------------------------------------------------------- */

		// !!!! *** **** read the direction currently leaning towards 

		// if leaning in a different direction = reset the direction and the increment amount 
		if (!compareVectors (toppleDir, detectDir, 0.05)){
			inc = initInc; 
			toppleDir = detectDir; 
		}
		else {
			constantForce.force.Set(toppleDir.x * inc, toppleDir.y, toppleDir.z * inc);
			inc++; 
		}
	}

	private bool compareVectors (Vector3 vecA, Vector3 vecB, float err){

			// if same length then false
			if (!Mathf.Approximately (vecA.magnitude, vecB.magnitude))
						return false; 

			// value in [-1, 1] which is the angle
			float cosError = Mathf.Cos (err * Mathf.Deg2Rad); 

			// dot product of normalized vectors
			float cosAngle = Vector3.Dot (vecA.normalized, vecB.normalized); 

			if (cosAngle >= cosError) {
						return true; 
				} else
						return false; 
	}
}
