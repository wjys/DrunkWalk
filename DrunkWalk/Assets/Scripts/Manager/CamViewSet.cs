using UnityEngine;
using System.Collections;

public class CamViewSet : MonoBehaviour {

	public Camera[] main;
	public Camera[] fall;
	public Camera[] ui;
	public UniMoveManager manager; 

	private Rect rect1a;
	private Rect rect2a;
	private Rect rect1b;
	private Rect rect2b;
	private Rect rect3;
	private Rect rect4;

	void Start () {
		rect1a.Set (0, 0, 0.5f, 1f);
		rect2a.Set (0.5f, 0, 0.5f, 1f);
		rect1b.Set (0, 0, 0.5f, 0.5f);
		rect2b.Set (0.5f, 0, 0.5f, 0.5f);
		rect3.Set (0, 0.5f, 0.5f, 0.5f);
		rect4.Set (0.5f, 0.5f, 0.5f, 0.5f);

		manager = gameObject.GetComponent<UniMoveManager> ();
		initCamArrays (manager.moveCount);
		setView (manager.moveCount);

	}

	private void initCamArrays(int num){
		main 	= new Camera[num];
		fall 	= new Camera[num];
		ui 		= new Camera[num];

		for (int i = 0; i < num; i++){
			main[i] = GameObject.Find ("Main Camera").camera; 
		}
		for (int i = 0; i < num; i++){
			fall[i] = GameObject.Find ("Fall Camera").camera; 
		}
		for (int i = 0; i < num; i++){
			ui[i] = GameObject.Find ("UICam").camera; 
		}

	}


	private void setView(int num){
		switch (num) {
		case 1:
			print ("ERROR, NEEDS 2 PLAYERS");
			break;

		// 2 PLAYERS
		case 2:
			foreach (Camera cam in main){
				UniMoveDisplay display = cam.GetComponentInParent<UniMoveDisplay>();
				if (display.id == 1){
					cam.rect = rect1a;
				}
				else {
					cam.rect = rect2a;
				}
			}
			foreach (Camera cam in fall){
				UniMoveDisplay display = cam.GetComponentInParent<UniMoveDisplay>();
				if (display.id == 1){
					cam.rect = rect1a;
				}
				else {
					cam.rect = rect2a;
				}
			}
			foreach (Camera cam in ui){
				UniMoveDisplay display = cam.GetComponentInParent<UniMoveDisplay>();
				if (display.id == 1){
					cam.rect = rect1a;
				}
				else {
					cam.rect = rect2a;
				}
			}
			break;

		// 3 PLAYERS
		case 3:
			foreach (Camera cam in main){
				UniMoveDisplay display = cam.GetComponentInParent<UniMoveDisplay>();
				if (display.id == 1){
					cam.rect = rect1b;
				}
				else if (display.id == 2){
					cam.rect = rect2b;
				}
				else {
					cam.rect = rect3;
				}
			}
			foreach (Camera cam in fall){
				UniMoveDisplay display = cam.GetComponentInParent<UniMoveDisplay>();
				if (display.id == 1){
					cam.rect = rect1b;
				}
				else if (display.id == 2){
					cam.rect = rect2b;
				}
				else {
					cam.rect = rect3;
				}
			}
			foreach (Camera cam in ui){
				UniMoveDisplay display = cam.GetComponentInParent<UniMoveDisplay>();
				if (display.id == 1){
					cam.rect = rect1b;
				}
				else if (display.id == 2){
					cam.rect = rect2b;
				}
				else {
					cam.rect = rect3;
				}
			}
			break;

		// 4 PLAYERS
		case 4:
			foreach (Camera cam in main){
				UniMoveDisplay display = cam.GetComponentInParent<UniMoveDisplay>();
				if (display.id == 1){
					cam.rect = rect1b;
				}
				else if (display.id == 2){
					cam.rect = rect2b;
				}
				else if (display.id == 3){
					cam.rect = rect3;
				}
				else {
					cam.rect = rect4;
				}
			}
			foreach (Camera cam in fall){
				UniMoveDisplay display = cam.GetComponentInParent<UniMoveDisplay>();
				if (display.id == 1){
					cam.rect = rect1b;
				}
				else if (display.id == 2){
					cam.rect = rect2b;
				}
				else if (display.id == 3){
					cam.rect = rect3;
				}
				else {
					cam.rect = rect4;
				}
			}
			foreach (Camera cam in ui){
				UniMoveDisplay display = cam.GetComponentInParent<UniMoveDisplay>();
				if (display.id == 1){
					cam.rect = rect1b;
				}
				else if (display.id == 2){
					cam.rect = rect2b;
				}
				else if (display.id == 3){
					cam.rect = rect3;
				}
				else {
					cam.rect = rect4;
				}
			}
			break;
		default:
			break;
		}
	}
}
