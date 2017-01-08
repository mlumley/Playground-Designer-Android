using UnityEngine;
using System.Collections;

public class SelectorIndicator : MonoBehaviour {

    protected static SelectorIndicator _instance;

    public static SelectorIndicator Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType(typeof(SelectorIndicator)) as SelectorIndicator;

                if (_instance == null) {
                    Debug.LogError("Warning: there should always be an instance of SelectorIndicator in the scene.");
                }
            }

            return _instance;
        }
    }

    public GameObject selector;
    private GameObject selectedObject;

    void Start() {
        selector = gameObject;
    }

    void Update() {
        if (selectedObject) {
            selector.transform.position = selectedObject.transform.position;
        }
    }

    public void SetSelectedObject(GameObject selectedObject) {
        if (selectedObject) {
            this.selectedObject = selectedObject;
            float diameter = selectedObject.GetComponent<BoxCollider>().size.x > selectedObject.GetComponent<BoxCollider>().size.z ? selectedObject.GetComponent<BoxCollider>().size.x : selectedObject.GetComponent<BoxCollider>().size.z;
            diameter += 1;
            selector.transform.localScale = new Vector3(diameter * selectedObject.transform.localScale.x, 1, diameter * selectedObject.transform.localScale.z);
            selector.transform.position = selectedObject.transform.position;
            selector.transform.rotation = selectedObject.transform.rotation;

            foreach(Transform sphere in selector.transform.GetComponentsInChildren<Transform>()) {
                if (sphere.GetComponent<SphereCollider>()) {
                    sphere.localScale = new Vector3(0.3f, diameter * 0.3f, 0.3f); ;
                }
            }
        }
        else {
            selector.SetActive(false);
        }
    }

    public void RotateSelectedObject(Vector3 mousePos, GameObject rotationSphere) {
        //selectedObject.transform.RotateAround(selectedObject.transform.position, Vector3.up, -degrees);
        //selector.transform.RotateAround(selector.transform.position, Vector3.up, -degrees);

        selectedObject.transform.LookAt(mousePos);
        selector.transform.LookAt(mousePos);

        //Debug.Log("Look at " + selector.transform.eulerAngles);

        float offset;
        if (rotationSphere.name == "Forward") {
            offset = 0;
        }
        else if (rotationSphere.name == "Right") {
            offset = 90;
        }
        else if (rotationSphere.name == "Backward") {
            offset = 180;
        }
        else {
            offset = 270;
        }

        //Debug.Log("Offset is " + offset);

        selectedObject.transform.eulerAngles = new Vector3(0, selectedObject.transform.eulerAngles.y - offset, 0);
        selector.transform.eulerAngles = new Vector3(0, selector.transform.eulerAngles.y - offset, 0);

        //Debug.Log("Offset " + selector.transform.eulerAngles);
    }
}
