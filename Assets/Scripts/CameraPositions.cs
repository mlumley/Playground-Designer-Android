using UnityEngine;
using System;
//using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class CameraPositions : MonoBehaviour {

    protected static CameraPositions _instance;

    public static CameraPositions Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType(typeof(CameraPositions)) as CameraPositions;

                if (_instance == null) {
                    Debug.LogError("Warning: there should always be an instance of CameraPositions in the scene.");
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
    public bool isFP = false;

    // For first person view
    Vector3 temp_2 = new Vector3(0, 0, 0);
    float viewingAngle = 0.0f;
    float lookingDownAngle = 10;

    // Speeds;

    public float panSpeed = 4.0f;       // Speed of the camera when being panned
    public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth
    public Transform cameraAnchor;


    // Rotation
    Vector3 lastMousePos = new Vector3();
    Vector3 currentMousePos = new Vector3();
    float deltaX = 0;
    float deltaY = 0;

    // Ground
    public GameObject ground;

    public bool IsZooming {
        get {
            return isZooming;
        }
        set {
            isZooming = value;
        }
    }


    void Start() {
        //transform.position = new Vector3(0f,14f,-16f);
        transform.LookAt(cameraAnchor);
        transform.localPosition = new Vector3(0, 0, -ground.transform.localScale.z * 10 / 2 - 40);
    }

    void Rotating() {


        if (Input.GetMouseButtonDown(0)) {

            lastMousePos = Input.mousePosition;
            //Debug.Log("Pressed " + lastMousePos.x);
        }

        if (Input.GetMouseButton(0)) {
            currentMousePos = Input.mousePosition;
            //Debug.Log("Current " + currentMousePos.x);
            deltaX = lastMousePos.x - currentMousePos.x;
            deltaY = lastMousePos.y - currentMousePos.y;

            // Left to Right
            if (lastMousePos.x != currentMousePos.x) {
                //Debug.Log("DeltaX: " + deltaX);
                cameraAnchor.eulerAngles = new Vector3(cameraAnchor.eulerAngles.x, cameraAnchor.eulerAngles.y + deltaX * 0.5f, 0);
            }

            // Top to Bottom
            if (lastMousePos.y != currentMousePos.y && cameraAnchor.eulerAngles.x + deltaY * 0.5f < 90 && cameraAnchor.eulerAngles.x + deltaY * 0.5f > 0) {
                //cameraAnchor.Rotate(Vector3.right, deltaY * 0.5f);
                cameraAnchor.eulerAngles = new Vector3(cameraAnchor.eulerAngles.x + deltaY * 0.5f, cameraAnchor.eulerAngles.y, 0);
            }

            //cameraAnchor.eulerAngles = new Vector3(cameraAnchor.eulerAngles.x, cameraAnchor.eulerAngles.y, 0);
            lastMousePos = currentMousePos;
        }
    }


    void Update() {
        // Lines 56 - 124 = First Person View code

        if (!isFP) {
            //Rotating();
        }

        /*if (Input.GetKeyDown("space")) {

            if (isFP == false) {
                transform.position = new Vector3(0, 2, 0);
                transform.rotation = Quaternion.Euler(0.0f, 0, 0);
                isFP = true;
                viewingAngle = 0.0f;
            }

            else if (isFP == true) {
                isFP = false;
                transform.position = new Vector3(0, 14, -16);
                transform.rotation = Quaternion.Euler(41.182f, 0, 0);

            }

        }*/

        if (Input.GetKey(KeyCode.LeftArrow) && isFP == true) {
            viewingAngle -= 1f;
            transform.rotation = Quaternion.Euler(lookingDownAngle, viewingAngle, 0);

        }

        if (Input.GetKey(KeyCode.RightArrow) && isFP == true) {
            viewingAngle += 1f;
            transform.rotation = Quaternion.Euler(lookingDownAngle, viewingAngle, 0);

        }

        if (Input.GetKey(KeyCode.UpArrow) && isFP == true) {


            temp_2.x = transform.position.x;
            temp_2.y = transform.position.y;
            temp_2.z = transform.position.z;

            temp_2.z += (float)(Math.Cos(viewingAngle * Math.PI / 180.0) / 10);

            temp_2.x += (float)(Math.Sin(viewingAngle * Math.PI / 180.0) / 10);

            transform.position = new Vector3(temp_2.x, temp_2.y, temp_2.z);

            //transform.position += transform.forward * 0.5f;

        }

        if (Input.GetKey(KeyCode.DownArrow) && isFP == true) {

            temp_2.x = transform.position.x;
            temp_2.y = transform.position.y;
            temp_2.z = transform.position.z;

            temp_2.z -= (float)(Math.Cos(viewingAngle * Math.PI / 180.0) / 10);

            temp_2.x -= (float)(Math.Sin(viewingAngle * Math.PI / 180.0) / 10);

            transform.position = new Vector3(temp_2.x, temp_2.y, temp_2.z);

            //transform.position -= transform.forward * 0.5f;
        }

        // 3D Rotation code. The AND condition forbids first person view unless
        // the first person view is enabled (in this case, using a
        // button to toggle it).



        float speed = 0.5f;

        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && isFP == false) {
            cameraAnchor.position += speed * new Vector3(cameraAnchor.forward.x, 0, cameraAnchor.forward.z);
        }

        if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && isFP == false) {
            cameraAnchor.position -= speed * new Vector3(cameraAnchor.forward.x, 0, cameraAnchor.forward.z);
        }

        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))&& isFP == false) {
            cameraAnchor.position += speed * new Vector3(cameraAnchor.right.x, 0, cameraAnchor.right.z);
        }

        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && isFP == false) {
            cameraAnchor.position -= speed * new Vector3(cameraAnchor.right.x, 0, cameraAnchor.right.z);
        }


        if (Input.GetAxis("Mouse ScrollWheel") != 0 && isFP == false) {
            // Check if we hit the UI
            PointerEventData cursor = new PointerEventData(EventSystem.current);
            cursor.position = Input.mousePosition;
            System.Collections.Generic.List<RaycastResult> objectsHit = new List<RaycastResult>();
            EventSystem.current.RaycastAll(cursor, objectsHit);
            bool hitUI = false;

            foreach (RaycastResult result in objectsHit) {
                //Debug.Log(result.gameObject.name);
                if (result.gameObject.layer == LayerMask.NameToLayer("UI")) {
                    hitUI = true;
                }
            }

            //Debug.Log(PlayerManager.Instance.hitUI);
            if (!hitUI) {
                if(Input.GetAxis("Mouse ScrollWheel") > 0) {
                    isZooming = true;
                    ZoomIn(5);
                    isZooming = false;
                }
                else {
                    isZooming = true;
                    ZoomOut(5);
                    isZooming = false;
                }
            }

            //}

        }


        if (!Input.GetMouseButton(0))
            isRotating = false;

        if (isRotating) {
            //Rotating();
        }



        /*if (isRotating) {

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
			}*/



        /*if (isZooming) {

			zoomY = Input.GetAxis ("Mouse Y");

			cameraMove = zoomY * zoomSpeed * transform.forward;

			transform.Translate (cameraMove, Space.World);
			


		}*/

    }

    Vector3 rotateAround(Vector3 point, Vector3 pivot, Vector3 angle) {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angle) * dir;
        point = dir + pivot;
        return point;
    }

    /*public void UpdatePosition() {
        if (isFP == false) {
            transform.position = new Vector3(0, 2, 0);
            transform.rotation = Quaternion.Euler(lookingDownAngle, 0, 0);
            isFP = true;
            //walkAround.isOn = true;
            //birdsEye.isOn = false;
            viewingAngle = 0.0f;
        }

        else if (isFP == true) {
            isFP = false;
            transform.localPosition = new Vector3(0, 0, -20);
            transform.localRotation = Quaternion.identity;
            //walkAround.isOn = false;
            //birdsEye.isOn = true;
        }

        //StringToEdit = GUI.TextField (new Rect (10, 10, 200, 20), StringToEdit, 25);
    }*/

    public void TurnOnWalkAround() {
        transform.parent.position = new Vector3(0, 2, -ground.transform.localScale.z*10/2);
        transform.parent.rotation = Quaternion.Euler(lookingDownAngle, 0, 0);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        isFP = true;
        viewingAngle = 0.0f;
    }

    public void TurnOffWalkAround() {
        isFP = false;
        transform.parent.position = new Vector3(0, 0, 0);
        transform.parent.rotation = Quaternion.Euler(45, 0, 0);
        transform.localPosition = new Vector3(0, 0, -20);
        transform.localRotation = Quaternion.identity;
    }

    public void ZoomIn(int zoomSpeed = 2) {
        if (isZooming) {
            //zoomSpeed = 2;
            cameraMove = zoomSpeed * 0.25f * transform.forward;
            if (transform.localPosition.z + cameraMove.z < -5)
                transform.Translate(cameraMove, Space.World);
        }
    }

    public void ZoomOut(int zoomSpeed = 2) {
        if (isZooming) {
            //zoomSpeed = -2;
            cameraMove = -zoomSpeed * 0.25f * transform.forward;
            transform.Translate(cameraMove, Space.World);
        }
    }

}




