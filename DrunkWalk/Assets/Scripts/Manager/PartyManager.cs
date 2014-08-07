using UnityEngine;
using System.Collections;

public class PartyManager : MonoBehaviour {

	public GameObject bed;
	public GameObject couch;
	public GameObject tub;

	public GameManager gm;

	public int previousIndex;

	// Use this for initialization
	void Start () {
		bed = GameObject.Find ("BedObj");
		couch = GameObject.Find ("CouchObj");
		tub = GameObject.Find ("TubObj");

		bed.GetComponent<BoxCollider>().enabled = true;
		couch.GetComponent<BoxCollider>().enabled = false;
		BoxCollider[] tubcols = tub.GetComponentsInChildren<BoxCollider>();
		foreach (BoxCollider col in tubcols){
			if (col.isTrigger){
				col.enabled = false;
			}
		}

		gm = gameObject.GetComponent<GameManager>();
		previousIndex = -1;
	}
	
	// Update is called once per frame
	void Update () {

		if (gm.winnerIndex == 0 && previousIndex == -1){
			bed.GetComponent<BoxCollider>().enabled = true;
			couch.GetComponent<BoxCollider>().enabled = false;
			BoxCollider[] tubcols = tub.GetComponentsInChildren<BoxCollider>();
			foreach (BoxCollider col in tubcols){
				if (col.isTrigger){
					col.enabled = false;
				}
			}
			previousIndex = 0;
		}
		else if (gm.winnerIndex == 1 && previousIndex == 0){
			bed.GetComponent<BoxCollider>().enabled = false;
			couch.GetComponent<BoxCollider>().enabled = true;
			previousIndex = 1;
		}
		else if (gm.winnerIndex == 2 && previousIndex == 1){
			couch.GetComponent<BoxCollider>().enabled = false;
			BoxCollider[] tubcols = tub.GetComponentsInChildren<BoxCollider>();
			foreach (BoxCollider col in tubcols){
				if (col.isTrigger){
					col.enabled = true;
				}
			}
			previousIndex = 2;
		}
	}
}
