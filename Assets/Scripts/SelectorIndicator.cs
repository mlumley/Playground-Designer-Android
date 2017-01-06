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
            selector.transform.localScale = new Vector3(diameter, 0.1f, diameter);
            selector.transform.position = selectedObject.transform.position;
            selector.transform.rotation = selectedObject.transform.rotation;
        }
        else {
            selector.SetActive(false);
        }
    }

    public void RotateSelectedObject(float degrees) {
        selectedObject.transform.RotateAround(selectedObject.transform.position, Vector3.up, -degrees);
        selector.transform.RotateAround(selector.transform.position, Vector3.up, -degrees);

        //selectedObject.transform.LookAt(mousePos);
        //selector.transform.LookAt(mousePos);
    }
}
