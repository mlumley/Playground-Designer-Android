using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class PlayerManager : MonoBehaviour {

    protected static PlayerManager _instance;

    public static PlayerManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;

                if (_instance == null) {
                    Debug.LogError("Warning: there should always be an instance of PlayerManager in the scene.");
                }
            }
            return _instance;
        }
    }

    [HideInInspector]
    public Transform currentObject = null;

    public Camera PlayerCamera;

    public bool isObjSelected = false;

    private float doubleClickTime = .4f;
    private float lastClickTime = -10f;

    private bool HitRotateCirlce = false;
    private Vector3 RotateCircleHitPreviousPoint;

    [HideInInspector]
    public bool MoveableUISelected = false;

    bool hasHitUI = false;

    // New
    public GameObject cameraAnchor;
    bool moveMode = false;
    bool rotateObject = false;
    bool hitUI = false;
    bool hitDelete = false;

    Vector3 lastMousePos = new Vector3();
    Vector3 currentMousePos = new Vector3();
    float deltaX = 0;
    float deltaY = 0;

    ClickManager clickManager = new ClickManager();

    void Update() {



        // Selected an object and deselect when not clicking on an object
        if (Input.GetMouseButtonDown(0)) {
            //.Debug.Log("Mouse Down");
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

            // Check if we hit the UI
            PointerEventData cursor = new PointerEventData(EventSystem.current);
            cursor.position = Input.mousePosition;
            List<RaycastResult> objectsHit = new List<RaycastResult>();
            EventSystem.current.RaycastAll(cursor, objectsHit);
            hitUI = false;
            hitDelete = false;

            foreach (RaycastResult result in objectsHit) {
                Debug.Log(result.gameObject.name);
                if (result.gameObject.layer == LayerMask.NameToLayer("UI")) {
                    hitUI = true;
                }
                if (result.gameObject.tag == "DeleteButton") {
                    hitDelete = true;
                }
            }

            if (hit && !hitUI && !hitDelete) {
                if (currentObject) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit groundHit = new RaycastHit();
                    if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                        if (RotateCirclesRenderer.Instance.HitRotateCircleCheck(groundHit.point)) {
                            Debug.Log("Hit rotation");
                            rotateObject = true;
                        }
                    }
                }
                if ((hitInfo.transform.gameObject.tag == "Models" || hitInfo.transform.gameObject.tag == "PhotoObject") && !rotateObject) {
                    Debug.Log(currentObject);
                    // Select new object
                    if ((currentObject == null || !currentObject.Equals(hitInfo.transform)) && clickManager.DoubleClick()) {
                        Debug.Log("Selected " + hitInfo.transform.name);
                        currentObject = hitInfo.transform;
                        isObjSelected = true;
                        //moveMode = true;
                        SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
                        ObjectWorldPanel.Instance.SetTarget(currentObject);
                    }

                    else if (!currentObject && !clickManager.DoubleClick()) {
                        Debug.Log("Object deselected");
                        SetSelectableToNull();
                    }
                    // Move selected object
                    else if(isObjSelected && currentObject.Equals(hitInfo.transform)) {
                        Debug.Log("Move Mode");
                        //SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
                        isObjSelected = true;
                        moveMode = true;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit groundHit = new RaycastHit();
                        if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                            Debug.Log("Hit " + groundHit.transform.name);
                            //Debug.Log("Moving " + hitInfo.transform.name + " to " + groundHit.point);
                            //currentObject.position = groundHit.point;
                            //SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
                        }
                    }
                    else {
                        Debug.Log("Object deselected");
                        SetSelectableToNull();
                    }

                }
                else if (hitInfo.transform.gameObject.tag == "Ground" && !rotateObject) {
                    Debug.Log("Hit ground");
                    SetSelectableToNull();
                }
                /*else if (hitInfo.transform.gameObject.tag == "PhotoObject") {
                    if ((currentObject == null || !currentObject.Equals(hitInfo.transform)) && clickManager.DoubleClick()) {
                        Debug.Log("Selected " + hitInfo.transform.name);
                        currentObject = hitInfo.transform;
                        isObjSelected = true;
                        //moveMode = true;
                        SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
                        ObjectWorldPanel.Instance.SetTarget(currentObject);
                    }
                    else if (isObjSelected) {
                        Debug.Log("Move Mode");
                        //SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
                        isObjSelected = true;
                        moveMode = true;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit groundHit = new RaycastHit();
                        if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                            Debug.Log("Hit " + groundHit.transform.name);
                            //Debug.Log("Moving " + hitInfo.transform.name + " to " + groundHit.point);
                            //currentObject.position = groundHit.point;
                            //SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
                        }
                    }
                }*/
                else {
                    Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                }
            }
            else if (!hitDelete) {
                Debug.Log("No hit");
                SetSelectableToNull();
            }
        }

        if (currentObject) {
            if (hitDelete) {
                DeleteCurrentObject();
            }
            else if (Input.GetMouseButton(0) && rotateObject) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit groundHit = new RaycastHit();
                if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                    RaycastHit hitInfo5;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo5, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                        float angle = SignedAngleBetween(RotateCircleHitPreviousPoint - currentObject.position, hitInfo5.point - currentObject.position, Vector3.up);
                        RotateCirclesRenderer.Instance.SetDegrees(-angle);
                        RotateCircleHitPreviousPoint = hitInfo5.point;
                    }
                }
            }
            else if (Input.GetMouseButton(0) && moveMode) {
                //Debug.Log("Move Mode");
                //SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
                isObjSelected = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit groundHit = new RaycastHit();
                if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                    //Debug.Log("Hit " + groundHit.transform.name);
                    //Debug.Log("Moving " + hitInfo.transform.name + " to " + groundHit.point);
                    //Debug.Log("Point " + groundHit.point);
                    currentObject.position = groundHit.point;
                    //SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
                }
            }
        }
        else if (Input.GetMouseButton(0) && !hitUI) {
            Rotating();
        }


        if (Input.GetMouseButtonUp(0)) {
            //moveMode = false;
            rotateObject = false;
        }



        /*if (Input.GetMouseButtonDown(0)) {
            PointerEventData pointer1 = new PointerEventData(EventSystem.current);
            pointer1.position = Input.mousePosition;
            List<RaycastResult> raycastResults1 = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer1, raycastResults1);

            Debug.Log("Mouse Down");
            if (raycastResults1.Count > 0) {
                foreach (RaycastResult result in raycastResults1) {
                    Debug.Log("Hit " + result.gameObject.name);
                    if (result.gameObject == null) {
                        break;
                    }

                    if (result.gameObject.layer == LayerMask.NameToLayer("UI")) {
                        hasHitUI = true;
                        break;
                    }
                    else if (result.gameObject.layer == LayerMask.NameToLayer("MoveableUI")) {
                        if (currentObject == null)
                            continue;

                        if (result.gameObject.transform.position != currentObject.position) {
                            SetSelectableToNull();
                            hasHitUI = false;
                            break;
                        }

                        hasHitUI = false;
                        break;
                    }
                }
            }
            else {
                Debug.Log("Hit nothing");
                hasHitUI = false;

                if (currentObject) {
                    if (MoveableUISelected) {
                        SetSelectableToNull();

                        return;
                    }

                    RaycastHit hitInfo4;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo4, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                        if (!MoveableUISelected) {
                            RaycastHit hitInfo6;
                            if (RotateCirclesRenderer.Instance.HitRotateCircleCheck(hitInfo4.point)) {
                                HitRotateCirlce = true;
                                RotateCircleHitPreviousPoint = hitInfo4.point;
                            }
                            else if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo6, Mathf.Infinity, 1 << LayerMask.NameToLayer("DesignObject"))) {
                                SetSelectableToNull();
                            }
                        }
                        else {
                            HitRotateCirlce = false;
                        }
                    }
                }
            }

            float timeDelta = Time.time - lastClickTime;

            //if(timeDelta < doubleClickTime)
            //{
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            //Debug.Log("Double click");
            //if (raycastResults.Count > 0)
            //{
            //	if (raycastResults [0].gameObject.layer == LayerMask.NameToLayer ("MoveableUI"))
            //	{
            //		IUISelectable selectable = raycastResults [0].gameObject.GetComponent<IUISelectable> ();

            //		if (selectable == null)
            //		{
            //			selectable = raycastResults [0].gameObject.transform.parent.GetComponent<IUISelectable> ();
            //			currentObject = raycastResults [0].gameObject.transform.parent;
            //		}
            //		else
            //		{
            //			currentObject = raycastResults [0].gameObject.transform;
            //		}

            //		selectable.Select(true);
            //		MoveableUISelected = true;
            //	}
            //}
            //else
            {
                Debug.Log("Single click");
                RaycastHit hitInfo;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("DesignObject"))) {
                    currentObject = hitInfo.transform;
                    SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
                }
            }

            lastClickTime = 0f;

        }
        else {
            lastClickTime = Time.time;
        }
        //}

        if (lastClickTime > 0f && Time.time - lastClickTime > doubleClickTime) {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            lastClickTime = 0f;
        }

        if (Input.GetMouseButton(0) && !hasHitUI) {
            //Debug.Log("Hit object");
            if (MoveableUISelected && currentObject) {
                Debug.Log("move");
                //currentObject.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //RaycastHit hit;
                //if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                //    currentObject.position = hit.point;
                //}

                isObjSelected = true;

                RaycastHit hitInfo3;
                Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitInfo3, Mathf.Infinity, 1 << LayerMask.NameToLayer("DesignObject"))) {
                    currentObject.position = new Vector3(Mathf.Round(Mathf.Clamp(hitInfo3.point.x, -(GridManager.Instance.gridSizeX / 2f), (GridManager.Instance.gridSizeX / 2f) - 1f)) + .5f, 0f, Mathf.Round(Mathf.Clamp(hitInfo3.point.z, -(GridManager.Instance.gridSizeZ / 2f), (GridManager.Instance.gridSizeZ / 2f) - 1f)) + .5f);
                }
            }
            else if (!EventSystem.current.IsPointerOverGameObject()) {
                if (HitRotateCirlce) {
                    RaycastHit hitInfo5;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo5, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                        float angle = SignedAngleBetween(RotateCircleHitPreviousPoint - currentObject.position, hitInfo5.point - currentObject.position, Vector3.up);
                        RotateCirclesRenderer.Instance.SetDegrees(-angle);
                        RotateCircleHitPreviousPoint = hitInfo5.point;
                    }
                }
                // need to select the object that we are hovering over first
                else if (currentObject) {
                    Debug.Log("Current");
                    RaycastHit hitInfo3;
                    Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hitInfo3, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                        currentObject.position = new Vector3(Mathf.Round(Mathf.Clamp(hitInfo3.point.x, -(GridManager.Instance.gridSizeX / 2f), (GridManager.Instance.gridSizeX / 2f) - 1f)) + .5f, 0f, Mathf.Round(Mathf.Clamp(hitInfo3.point.z, -(GridManager.Instance.gridSizeZ / 2f), (GridManager.Instance.gridSizeZ / 2f) - 1f)) + .5f);
                    }
                }
            }

        }

        if (Input.GetMouseButtonUp(0)) {
            if (HitRotateCirlce)
                HitRotateCirlce = false;
        }*/
    }


    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n) {
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        float signed_angle = angle * sign;

        return signed_angle;
    }

    public void SetObject(string objectName) {
        /*Debug.Log("Made Object");
		GameObject newObject = Instantiate(Resources.Load ("ModelPrefabs/" + objectName)) as GameObject;
		newObject.transform.position = Vector3.zero;
		currentObject = newObject.transform;

		SelectedObjectCircleRenderer.Instance.SetSelectedObject (currentObject);*/

        /*AssetBundleRequest request = DataManager.modelBundle.LoadAssetAsync(objectName, typeof(GameObject));
        yield return request;
        GameObject newObject = (GameObject)request.asset;
        newObject.transform.position = Vector3.zero;
        currentObject = newObject.transform;

        SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);*/

        isObjSelected = true;
        //moveMode = true;

        StartCoroutine(LoadObject(objectName));
    }

    IEnumerator LoadObject(string objectName) {
        yield return new WaitUntil(() => DataManager.modelBundles.Count == DataManager.names.Length);
        // Load and retrieve the AssetBundle
        AssetBundle[] bundles = DataManager.modelBundles.ToArray();
        AssetBundle bundle = new AssetBundle();

        for (int i = 0; i < bundles.Length; i++) {
            if (bundles[i].Contains(objectName)) {
                bundle = bundles[i];
                break;
            }
        }

        // Load the object asynchronously
        AssetBundleRequest request = bundle.LoadAssetAsync(objectName, typeof(GameObject));

        // Wait for completion
        yield return request;

        // Get the reference to the loaded object
        GameObject newObject = Instantiate(request.asset) as GameObject;
        newObject.transform.position = Vector3.zero;
        newObject.AddComponent<BoxCollider>();
        //newObject.GetComponent<BoxCollider>().center = Vector3.zero;
        //newObject.GetComponent<BoxCollider>().size = new Vector3(10, 10, 10);
        CalculateLocalBounds(newObject);
        newObject.GetComponent<BoxCollider>().isTrigger = true;
        newObject.AddComponent<SelectedObjectCollision>();
        newObject.layer = LayerMask.NameToLayer("DesignObject");
        newObject.tag = "Models";
        currentObject = newObject.transform;
        SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
    }

    private void CalculateLocalBounds(GameObject newObject) {
        Quaternion currentRotation = newObject.transform.rotation;
        newObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Bounds bounds = new Bounds(newObject.transform.position, Vector3.zero);

        foreach (Renderer renderer in newObject.GetComponentsInChildren<Renderer>()) {
            Debug.Log("Found Renderer");
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 localCenter = bounds.center - newObject.transform.position;
        bounds.center = localCenter;
        Debug.Log("The local bounds of this model is " + bounds);
        newObject.GetComponent<BoxCollider>().size = bounds.size;
        newObject.GetComponent<BoxCollider>().center = new Vector3(0, localCenter.y, 0);

        newObject.transform.rotation = currentRotation;
    }

    public static IEnumerator LoadObjectAtPositionAndRotation(string objectName, Vector3 position, Quaternion rotation) {
        // Load and retrieve the AssetBundle
        AssetBundle[] bundles = DataManager.modelBundles.ToArray();
        AssetBundle bundle = new AssetBundle();
        for (int i = 0; i < bundles.Length; i++) {
            if (bundles[i].Contains(objectName)) {
                bundle = bundles[i];
                break;
            }
        }

        // Load the object asynchronously
        AssetBundleRequest request = bundle.LoadAssetAsync(objectName, typeof(GameObject));

        // Wait for completion
        yield return request;

        // Get the reference to the loaded object
        GameObject newObject = Instantiate(request.asset) as GameObject;
        newObject.transform.position = position;
        newObject.transform.rotation = rotation;
        newObject.AddComponent<BoxCollider>();
        newObject.GetComponent<BoxCollider>().center = Vector3.zero;
        newObject.GetComponent<BoxCollider>().size = new Vector3(10, 10, 10);
        newObject.GetComponent<BoxCollider>().isTrigger = true;
        newObject.AddComponent<SelectedObjectCollision>();
        newObject.layer = LayerMask.NameToLayer("DesignObject");
        newObject.tag = "Models";

    }

    public void DeleteCurrentObject() {
        Destroy(currentObject.gameObject);
        ObjectWorldPanel.Instance.SetTarget(null);
    }

    public void SetSelectableToNull() {
        isObjSelected = false;
        moveMode = false;
        if (currentObject) {
            IUISelectable selectable = currentObject.GetComponent<IUISelectable>();

            if (selectable != null)
                selectable.Select(false);

            currentObject = null;
        }

        MoveableUISelected = !hasHitUI;
        ObjectWorldPanel.Instance.SetTarget(null);
        SelectedObjectCircleRenderer.Instance.SetSelectedObject(null);
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
                cameraAnchor.transform.eulerAngles = new Vector3(cameraAnchor.transform.eulerAngles.x, cameraAnchor.transform.eulerAngles.y - deltaX * 0.5f, 0);
            }

            // Top to Bottom
            if (lastMousePos.y != currentMousePos.y && cameraAnchor.transform.eulerAngles.x + deltaY * 0.5f < 90 && cameraAnchor.transform.eulerAngles.x + deltaY * 0.5f > 5) {
                //cameraAnchor.Rotate(Vector3.right, deltaY * 0.5f);
                cameraAnchor.transform.eulerAngles = new Vector3(cameraAnchor.transform.eulerAngles.x + deltaY * 0.5f, cameraAnchor.transform.eulerAngles.y, 0);
            }

            //cameraAnchor.eulerAngles = new Vector3(cameraAnchor.eulerAngles.x, cameraAnchor.eulerAngles.y, 0);
            lastMousePos = currentMousePos;
        }
    }

    public void SelectObject(GameObject obj) {
        isObjSelected = true;
        currentObject = obj.transform;
        SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
    }
}
