using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Controls the cameras movement in the app
/// </summary>

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

    public bool isZooming = false;
    public bool isFP = false;
    public bool isTyping = false;

    // For first person view
    float viewingAngle = 0.0f;
    float lookingDownAngle = 10;

    // Speeds;
    public float panSpeed = 0.5f;       // Speed of the camera when being panned
    public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth

    public Transform cameraAnchor;

    // Ground
    public GameObject ground;

    // Used in the zoom in and out buttons
    // so we can do click and hold
    public bool IsZooming {
        get{
            return isZooming;
        }
        set {
            isZooming = value;
        }
    }


    void Start() {
        // Set the camera to point towards the centre of the ground plane
        transform.LookAt(cameraAnchor);
        // Set the camera to be 40 units back from the edge of the ground
        transform.localPosition = new Vector3(0, 0, -ground.transform.localScale.z * 10 / 2 - 40);
    }

    void Update() {
        // Control movement in first person mode
        if (Input.GetKey(KeyCode.LeftArrow) && isFP == true) {
            viewingAngle -= 1f;
            transform.rotation = Quaternion.Euler(lookingDownAngle, viewingAngle, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow) && isFP == true) {
            viewingAngle += 1f;
            transform.rotation = Quaternion.Euler(lookingDownAngle, viewingAngle, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow) && isFP == true) {
            Vector3 temp_2 = transform.position;

            temp_2.z += (float)(Math.Cos(viewingAngle * Math.PI / 180.0) / 10);
            temp_2.x += (float)(Math.Sin(viewingAngle * Math.PI / 180.0) / 10);

            transform.position = new Vector3(temp_2.x, temp_2.y, temp_2.z);
        }
        if (Input.GetKey(KeyCode.DownArrow) && isFP == true) {
            Vector3 temp_2 = transform.position;

            temp_2.z -= (float)(Math.Cos(viewingAngle * Math.PI / 180.0) / 10);
            temp_2.x -= (float)(Math.Sin(viewingAngle * Math.PI / 180.0) / 10);

            transform.position = new Vector3(temp_2.x, temp_2.y, temp_2.z);
        }


        // Control movement of the camera anchor
        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && isFP == false && !isTyping) {
            cameraAnchor.position += panSpeed * new Vector3(cameraAnchor.forward.x, 0, cameraAnchor.forward.z);
        }
        if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && isFP == false && !isTyping) {
            cameraAnchor.position -= panSpeed * new Vector3(cameraAnchor.forward.x, 0, cameraAnchor.forward.z);
        }
        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && isFP == false && !isTyping) {
            cameraAnchor.position += panSpeed * new Vector3(cameraAnchor.right.x, 0, cameraAnchor.right.z);
        }
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && isFP == false && !isTyping) {
            cameraAnchor.position -= panSpeed * new Vector3(cameraAnchor.right.x, 0, cameraAnchor.right.z);
        }

        // Control zooming with mouse wheel
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && isFP == false) {
            // Check if we hit the UI
            PointerEventData cursor = new PointerEventData(EventSystem.current);
            cursor.position = Input.mousePosition;
            System.Collections.Generic.List<RaycastResult> objectsHit = new List<RaycastResult>();
            EventSystem.current.RaycastAll(cursor, objectsHit);
            bool hitUI = false;

            foreach (RaycastResult result in objectsHit) {
                if (result.gameObject.layer == LayerMask.NameToLayer("UI")) {
                    hitUI = true;
                }
            }

            // If the mouse is not hovered over the UI zoom in and out
            if (!hitUI) {
                if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                    isZooming = true;
                    ZoomIn(zoomSpeed);
                    isZooming = false;
                }
                else {
                    isZooming = true;
                    ZoomOut(zoomSpeed);
                    isZooming = false;
                }
            }
        }
    }

    /// <summary>
    /// Enable the First Person walk around mode
    /// </summary>
    public void TurnOnWalkAround() {
        transform.parent.position = new Vector3(0, 2, -ground.transform.localScale.z * 10 / 2);
        transform.parent.rotation = Quaternion.Euler(lookingDownAngle, 0, 0);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        isFP = true;
        viewingAngle = 0.0f;
    }

    /// <summary>
    /// Disable the First Person walk around mode
    /// </summary>
    public void TurnOffWalkAround() {
        isFP = false;
        transform.parent.position = new Vector3(0, 0, 0);
        transform.parent.rotation = Quaternion.Euler(45, 0, 0);
        transform.localPosition = new Vector3(0, 0, -20);
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Move the camera closer to the anchor point to zoom in
    /// </summary>
    /// <param name="zoomSpeed">Amount to zoom in by</param>
    public void ZoomIn(float zoomSpeed = 2) {
        if (isZooming) {
            Vector3 cameraMove = zoomSpeed * 0.25f * transform.forward;
            if (transform.localPosition.z + cameraMove.z < -5)
                transform.Translate(cameraMove, Space.World);
        }
    }

    /// <summary>
    /// Move the camera further from the anchor point to zoom out
    /// </summary>
    /// <param name="zoomSpeed">Amount to zoom out by</param>
    public void ZoomOut(float zoomSpeed = 2) {
        if (isZooming) {
            Vector3 cameraMove = -zoomSpeed * 0.25f * transform.forward;
            transform.Translate(cameraMove, Space.World);
        }
    }

}