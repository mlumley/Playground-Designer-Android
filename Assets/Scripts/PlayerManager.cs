using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Handles loading models, rotating the camera and moving models
/// </summary>
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
    bool hitSlider = false;

    Vector3 lastMousePos = new Vector3();
    Vector3 currentMousePos = new Vector3();
    float deltaX = 0;
    float deltaY = 0;

    ClickManager clickManager = new ClickManager();

    GameObject rotationSphere = null;

    public GameObject ToolTip;

    void Update() {
        // Selected an object and deselect when not clicking on an object
        if (Input.GetMouseButtonDown(0)) {
            // Select the smallest object
            RaycastHit[] hits;
            hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);

            bool hit = false;
            RaycastHit hitInfo = new RaycastHit();
            if (hits.Length > 1) {
                foreach (RaycastHit info in hits) {
                    Debug.Log(info.transform.name);
                    if (hitInfo.collider == null || info.collider.bounds.size.x < hitInfo.collider.bounds.size.x || info.collider.bounds.size.z < hitInfo.collider.bounds.size.z) {
                        hitInfo = info;
                        hit = true;
                    }
                }
            }
            else if(hits.Length == 1) {
                hit = true;
                hitInfo = hits[0];
            }


            // Check if we hit the UI, delete button or scale slider
            PointerEventData cursor = new PointerEventData(EventSystem.current);
            cursor.position = Input.mousePosition;
            List<RaycastResult> objectsHit = new List<RaycastResult>();
            EventSystem.current.RaycastAll(cursor, objectsHit);
            hitUI = false;
            hitDelete = false;
            hitSlider = false;

            foreach (RaycastResult result in objectsHit) {
                //Debug.Log("UI raycaster hit " + result.gameObject.name);
                if (result.gameObject.layer == LayerMask.NameToLayer("UI")) {
                    hitUI = true;
                }
                if (result.gameObject.tag == "DeleteButton") {
                    hitDelete = true;
                }
                if(result.gameObject.tag == "ScaleSlider") {
                    hitSlider = true;
                    moveMode = false;
                }
            }

            // We only hit a model
            if (hit && !hitUI && !hitDelete && !hitSlider) {
                // Hit rotation object
                if(hitInfo.transform.gameObject.tag == "RotateSphere") {
                    Debug.Log("Hit rotation");
                    rotateObject = true;
                    rotationSphere = hitInfo.transform.gameObject;
                }
                // Hit an object
                if ((hitInfo.transform.gameObject.tag == "Models" || hitInfo.transform.gameObject.tag == "PhotoObject") && !rotateObject) {
                    Debug.Log(currentObject);
                    // Select the object we hit
                    if ((currentObject == null || !currentObject.Equals(hitInfo.transform)) && clickManager.DoubleClick()) {
                        Debug.Log("Selected " + hitInfo.transform.name);
                        currentObject = hitInfo.transform;
                        isObjSelected = true;
                        SelectObject(currentObject.gameObject);
                        ObjectWorldPanel.Instance.SetTarget(currentObject);
                    }
                    // Needed so double clicking doesn't rotate the camera?
                    else if (!currentObject && !clickManager.DoubleClick()) {
                        Debug.Log("Object deselected");
                        SetSelectableToNull();
                    }
                    // Move selected object
                    else if(isObjSelected && currentObject.Equals(hitInfo.transform)) {
                        Debug.Log("Move Mode");
                        isObjSelected = true;
                        moveMode = true;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit groundHit = new RaycastHit();
                        if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                            Debug.Log("Hit " + groundHit.transform.name);
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
                else {
                    Debug.Log("Hit unknown object" + hitInfo.transform.gameObject.name);
                }
            }
            else if (!hitDelete && !hitSlider) {
                Debug.Log("Hit nothing");
                SetSelectableToNull();
            }
        }

        // With object selected
        if (currentObject) {
            if (hitDelete) {
                DeleteCurrentObject();
                SetSelectableToNull();
            }
            // Rotate Object
            else if (Input.GetMouseButton(0) && rotateObject) {
                //Debug.Log("In rotate");
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                    SelectorIndicator.Instance.RotateSelectedObject(hit.point, rotationSphere);
                }
            }
            // Move Object
            else if (Input.GetMouseButton(0) && moveMode) {
                //Debug.Log("Move Mode");
                isObjSelected = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit groundHit = new RaycastHit();
                if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                    currentObject.position = groundHit.point;
                }
            }
        }
        // Rotate Camera
        else if (Input.GetMouseButton(0) && !hitUI && !CameraPositions.Instance.isFP) {
            Rotating();
        }

        // Stop rotating object on mouse up
        if (Input.GetMouseButtonUp(0)) {
            rotateObject = false;
        }
    }

    /// <summary>
    /// Spawn the specified model and select it
    /// </summary>
    /// <param name="objectInfo">The info of the model to be spawned</param>
    public void SpawnModel(DesignInfo objectInfo) {
        SetSelectableToNull();
        isObjSelected = true;
        StartCoroutine(LoadObject(objectInfo));
    }

    /// <summary>
    /// Looks up a model and loads it from the assetbundle and spawns it
    /// </summary>
    /// <param name="objectInfo">The info of the model to be spawned</param>
    /// <returns></returns>
    IEnumerator LoadObject(DesignInfo objectInfo) {
        // Load and retrieve the AssetBundle
        AssetBundle[] bundles = DataManager.modelBundles.ToArray();
        AssetBundle bundle = new AssetBundle();

        for (int i = 0; i < bundles.Length; i++) {
            if (bundles[i].Contains(objectInfo.ModelName)) {
                bundle = bundles[i];
                break;
            }
        }

        // Load the object asynchronously
        AssetBundleRequest request = bundle.LoadAssetAsync(objectInfo.ModelName, typeof(GameObject));

        // Wait for completion
        yield return request;

        // Get the reference to the loaded object
        GameObject newObject = Instantiate(request.asset) as GameObject;
        newObject.transform.position = new Vector3(0, -100, 0);
        newObject.AddComponent<BoxCollider>();
        newObject.GetComponent<BoxCollider>().isTrigger = true;
        CalculateLocalBounds(newObject);
        newObject.AddComponent<ObjectInfo>();
        newObject.GetComponent<ObjectInfo>().info = objectInfo;
        newObject.layer = LayerMask.NameToLayer("DesignObject");
        newObject.tag = "Models";
        currentObject = newObject.transform;
        moveMode = true;
        SelectObject(currentObject.gameObject);
        ObjectWorldPanel.Instance.SetTarget(currentObject);
    }

    /// <summary>
    /// Adjust the size of the box collider so that it fits the model
    /// </summary>
    /// <param name="newObject">The model</param>
    private static void CalculateLocalBounds(GameObject newObject) {
        Quaternion currentRotation = newObject.transform.rotation;
        newObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Bounds bounds = new Bounds(newObject.transform.position, Vector3.zero);

        foreach (Renderer renderer in newObject.GetComponentsInChildren<Renderer>()) {
            //Debug.Log("Found Renderer");
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 localCenter = bounds.center - newObject.transform.position;
        bounds.center = localCenter;
        //Debug.Log("The local bounds of this model is " + bounds);
        newObject.GetComponent<BoxCollider>().size = bounds.size;
        newObject.GetComponent<BoxCollider>().center = new Vector3(0, localCenter.y, 0);

        newObject.transform.rotation = currentRotation;
    }

    /// <summary>
    /// Looks up a model and loads it from the assetbundle and spawns it at a position with a rotation and a scale
    /// </summary>
    /// <param name="infoList">List of all model infos</param>
    /// <param name="objectName">Name  of model to be spawned</param>
    /// <param name="position">Position of model to be spawned</param>
    /// <param name="rotation">Rotation of model to be spawned</param>
    /// <param name="scale">Scale of model to be spawned</param>
    public static IEnumerator LoadObjectAtPositionAndRotation(List<DesignInfo> infoList, string objectName, Vector3 position, Quaternion rotation, Vector3 scale) {
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

        DesignInfo objectInfo = null;

        foreach(DesignInfo info in infoList) {
            if(info.Name.ToLower() == objectName.ToLower()) {
                objectInfo = info;
            }
        }

        // Create the model in the world
        newObject.AddComponent<ObjectInfo>();
        newObject.GetComponent<ObjectInfo>().info = objectInfo;
        newObject.AddComponent<BoxCollider>();
        CalculateLocalBounds(newObject);
        newObject.transform.localScale = scale;
        newObject.GetComponent<BoxCollider>().isTrigger = true;
        newObject.layer = LayerMask.NameToLayer("DesignObject");
        newObject.tag = "Models";
    }

    // CHANGE ME
    // Get one image: /images/get.php?userId={x}&designId={y}&name={n}
    public static void LoadPhoto(Vector3 position, Quaternion rotation, Vector3 scale, Texture2D image) {
        GameObject photoObject = Instantiate(Resources.Load("UIPrefabs/PhotoWorldObject")) as GameObject;
        photoObject.name = "PhotoObject" + Guid.NewGuid().ToString();
        photoObject.transform.position = position;
        photoObject.transform.rotation = rotation;
        photoObject.transform.localScale = scale;
        Texture2D texture = new Texture2D(1,1);
        texture = image;
        photoObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0f));
        photoObject.GetComponent<BoxCollider>().center = photoObject.GetComponent<SpriteRenderer>().sprite.bounds.center;
        photoObject.GetComponent<BoxCollider>().size = photoObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        PlayerManager.Instance.SelectObject(photoObject);
    }

    /// <summary>
    /// Deletes the currently selected object
    /// </summary>
    public void DeleteCurrentObject() {
        Destroy(currentObject.gameObject);
        ObjectWorldPanel.Instance.SetTarget(null);
    }

    /// <summary>
    /// Deselects the current object
    /// </summary>
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
        SelectObject(null);
    }

    /// <summary>
    /// Handles camera rotation
    /// </summary>
    void Rotating() {
        if (Input.GetMouseButtonDown(0)) {
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0)) {
            currentMousePos = Input.mousePosition;
            // Get the difference between the position the mouse was in when it was click and where it is now
            deltaX = lastMousePos.x - currentMousePos.x;
            deltaY = lastMousePos.y - currentMousePos.y;
            // Left to Right
            if (lastMousePos.x != currentMousePos.x) {
                cameraAnchor.transform.eulerAngles = new Vector3(cameraAnchor.transform.eulerAngles.x, cameraAnchor.transform.eulerAngles.y - deltaX * 0.5f, 0);
            }
            // Top to Bottom
            if (lastMousePos.y != currentMousePos.y && cameraAnchor.transform.eulerAngles.x + deltaY * 0.5f < 90 && cameraAnchor.transform.eulerAngles.x + deltaY * 0.5f > 5) {
                cameraAnchor.transform.eulerAngles = new Vector3(cameraAnchor.transform.eulerAngles.x + deltaY * 0.5f, cameraAnchor.transform.eulerAngles.y, 0);
            }
            lastMousePos = currentMousePos;
        }
    }

    /// <summary>
    /// Change the scale of the current object
    /// </summary>
    /// <param name="percent">Value between 0 and 2 where 1 is the default scale</param>
    public void ScaleCurrentObject(float percent) {
        currentObject.transform.localScale = new Vector3(percent, percent, percent);
        Vector3 scale = currentObject.GetComponent<BoxCollider>().size;
        SelectorIndicator.Instance.selector.transform.localScale = new Vector3(scale.x * percent, 0.1f, scale.x * percent);
    }

    /// <summary>
    /// Select a specific model by name
    /// </summary>
    /// <param name="obj">Name of the model</param>
    public void SelectObject(GameObject obj) {
        isObjSelected = true;
        if (obj) {
            SelectorIndicator.Instance.selector.SetActive(true);
            currentObject = obj.transform;
            SelectorIndicator.Instance.SetSelectedObject(currentObject.gameObject);
            ObjectWorldPanel.Instance.SetTarget(currentObject);
        }
        // No model so select nothing
        else {
            currentObject = null;
            SelectorIndicator.Instance.SetSelectedObject(null);
            ObjectWorldPanel.Instance.SetTarget(null);
        }
    }
}
