using UnityEngine;
using System.Collections;

public class ExitTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter(Collider other){
		if (other.tag == "Me"){
			Debug.Log ("Went through door");
		}
	}
}
