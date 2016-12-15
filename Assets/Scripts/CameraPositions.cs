using UnityEngine;
using System;
//using System.Collections.Generic;
using System.Collections;

public class CameraPositions : MonoBehaviour
{

	protected static CameraPositions _instance;

	public static CameraPositions Instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType (typeof (CameraPositions)) as CameraPositions;

				if (_instance == null) {
					Debug.LogError ("Warning: there should always be an instance of CameraPositions in the scene.");
				}
			}

			return _instance;
		}
	}

	Vector3 mousePosition;
	Vector3 mouseGamePosition;
	float zoomY;
	Vector3 undergroundChecker;
	Vector3 previousPosition;
	Vector3 cameraMove;
	double xzHypotenuse;

	public string StringToEdit = "Hello World";



	// All the perspective modes (isRotating, isZooming and isFirstPerson)
	public bool isRotating = false;
	bool isZooming = false;
	bool isFP = false;

	// For first person view
	Vector3 temp_2 = new Vector3(0,0,0);
	float viewingAngle = 0.0f;

	// Speeds;
	   
	public float panSpeed = 4.0f;       // Speed of the camera when being panned
	public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth
	public Transform cameraAnchor;


	void Start ()
	{
		transform.position = new Vector3(0f,14f,-16f);
		transform.LookAt (cameraAnchor);
	}



	void Update ()
	{
		// Lines 56 - 124 = First Person View code


		if (Input.GetKeyDown ("space")) {
			
			if (isFP == false) {
				transform.position = new Vector3 (0, 2, 0);
				transform.rotation = Quaternion.Euler (0.0f, 0, 0);
				isFP = true;
				viewingAngle = 0.0f;
			}

			else if (isFP == true) {
				isFP = false;
				transform.position = new Vector3(0, 14, -16);
				transform.rotation = Quaternion.Euler(41.182f,0,0);

			}

		}
			
		if (Input.GetKey (KeyCode.LeftArrow) && isFP == true) {
			viewingAngle -= 0.8f;
			transform.rotation = Quaternion.Euler (0, viewingAngle, 0);

		}

		if (Input.GetKey (KeyCode.RightArrow) && isFP == true) {
			viewingAngle += 0.8f;
			transform.rotation = Quaternion.Euler (0, viewingAngle, 0);

		}

		if (Input.GetKey (KeyCode.UpArrow) && isFP == true) {


			temp_2.x = transform.position.x;
			temp_2.y = transform.position.y;
			temp_2.z = transform.position.z;

			temp_2.z += (float)(Math.Cos (viewingAngle * Math.PI/180.0 ) / 10);

			temp_2.x += (float)(Math.Sin (viewingAngle * Math.PI/180.0) / 10);

			transform.position = new Vector3 (temp_2.x, temp_2.y, temp_2.z);

		}

		if (Input.GetKey (KeyCode.DownArrow) && isFP == true) {

			temp_2.x = transform.position.x;
			temp_2.y = transform.position.y;
			temp_2.z = transform.position.z;

			temp_2.z -= (float)(Math.Cos (viewingAngle * Math.PI / 180.0) / 10);

			temp_2.x -= (float)(Math.Sin (viewingAngle * Math.PI / 180.0) / 10);

			transform.position = new Vector3 (temp_2.x, temp_2.y, temp_2.z);
		}

		// 3D Rotation code. The AND condition forbids first person view unless
		// the first person view is enabled (in this case, using a
		// button to toggle it).

		if (Input.GetKey(KeyCode.UpArrow) && isFP == false)
		{
			ZoomIn();
		}

		if (Input.GetKey(KeyCode.DownArrow) && isFP == false)
		{
			ZoomOut();
		}


		if (Input.GetMouseButtonDown (0) && isFP == false) {
			mousePosition = Input.mousePosition;
			if ((Input.mousePosition.y >= (Screen.height/285 * 100)) && (Input.mousePosition.y <= (Screen.height/285 * 266)))
			{
				//if (!(Input.mousePosition.x / Screen.width >= 48/35 ))

				if (PlayerManager.Instance.isObjSelected == false) {
					
					isRotating = true;
				}

			}
					
		}


		if (!Input.GetMouseButton(0)) isRotating = false;



		if (isRotating) {

			undergroundChecker.x = this.transform.position.x;
			undergroundChecker.y = this.transform.position.y;
			undergroundChecker.z = this.transform.position.z;


			cameraMove = new Vector3 (Input.GetAxis ("Mouse X") * panSpeed, Input.GetAxis ("Mouse Y") * panSpeed, 0);



			if (this.transform.rotation.x != 90f) {
				transform.Translate (cameraMove, Space.Self);
				transform.LookAt (cameraAnchor);
			} else if (this.transform.rotation.x == 90f) {
				transform.position = new Vector3 (0, 0, 0);
				panSpeed = 0.01f;
				transform.Translate (new Vector3 (Input.GetAxis ("Mouse X") * panSpeed, 0, 0), Space.Self);
				transform.LookAt (cameraAnchor);
			}
			if (undergroundChecker.y <= 5) {
				transform.position = new Vector3 (undergroundChecker.x, 5, undergroundChecker.z);
				transform.Translate (cameraMove, Space.Self);
				transform.LookAt (cameraAnchor);
			}

	

		/*if (isZooming) {

			zoomY = Input.GetAxis ("Mouse Y");

			cameraMove = zoomY * zoomSpeed * transform.forward;

			transform.Translate (cameraMove, Space.World);
			*/


		}

	}

	public void UpdatePosition ()

	{
		if (isFP == false) {
			transform.position = new Vector3 (0, 2, 0);
			transform.rotation = Quaternion.Euler (0.0f, 0, 0);
			isFP = true;
			viewingAngle = 0.0f;
		}

		else if (isFP == true) {
			isFP = false;
			transform.position = new Vector3(0, 14, -16);
			transform.rotation = Quaternion.Euler(41.182f,0,0);

		}

		//StringToEdit = GUI.TextField (new Rect (10, 10, 200, 20), StringToEdit, 25);
	}		

	public void ZoomIn()
	{
		zoomSpeed = 4;
		cameraMove = zoomSpeed * 0.25f * transform.forward;
		transform.Translate(cameraMove, Space.World);
	}

	public void ZoomOut()
	{
		zoomSpeed = -4;
		cameraMove = zoomSpeed * 0.25f * transform.forward;
		transform.Translate(cameraMove, Space.World);
	}

}




