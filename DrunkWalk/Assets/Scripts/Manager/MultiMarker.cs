using UnityEngine;
using System.Collections;

public class MultiMarker : MonoBehaviour {
	public int id;
	
	public UniMoveController UniMove;
	public GameObject marker;
	
	public int controller;
	private enum controlInput {mouse, move, xbox };
	
	private enum Dir {forward, right, left, back};
	
	void Start () {
	}
	
	void Update () {
		
	}
}
