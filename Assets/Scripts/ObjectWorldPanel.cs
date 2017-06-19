using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls UI objects linked to models
/// </summary>
public class ObjectWorldPanel : MonoBehaviour {

    protected static ObjectWorldPanel _instance;

    public static ObjectWorldPanel Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType(typeof(ObjectWorldPanel)) as ObjectWorldPanel;

                if (_instance == null) {
                    Debug.LogError("Warning: there should always be an instance of ObjectWorldPanel in the scene.");
                }
            }

            return _instance;
        }
    }

    public GameObject DeleteButton;
    public GameObject Slider;
    private Transform target = null;
    public Vector3 DeleteButtonOffset;
    public Vector3 SliderOffset;
    public GameObject Selector;

    /// <summary>
    /// Select the model and display the delete button and the scale slider
    /// if it is photo or landscape model
    /// </summary>
    /// <param name="targetTrans">Model to place the UI elements around</param>
    public void SetTarget(Transform targetTrans) {
        target = targetTrans;

        if (target) {
            DeleteButton.SetActive(true);
            if(target.gameObject.tag == "PhotoObject" || target.gameObject.GetComponent<ObjectInfo>().info.MainCategory == "Landscape") {
                Slider.SetActive(true);
                Slider.GetComponent<Slider>().value = target.localScale.x;
            }
        }
        else {
            DeleteButton.SetActive(false);
            Slider.SetActive(false);
        }
    }

    void LateUpdate() {
        // Move the scale slider and delete button with the model seleted
        if (target) {
            // Delete Button
            Vector3 point = Selector.transform.localScale.x * new Vector3(-1, 0, 1).normalized + new Vector3(-1.25f, 0, 1.25f) + Selector.transform.position;
            point = rotateAround(point, Selector.transform.position, new Vector3 (0, Camera.main.transform.parent.transform.eulerAngles.y, 0));
            DeleteButton.transform.position = Camera.main.WorldToScreenPoint(point);

            // Scale Slider
            point = Selector.transform.localScale.x * new Vector3(1, 0, 0).normalized + new Vector3(1.5f, 0, 0) + Selector.transform.position;
            point = rotateAround(point, Selector.transform.position, new Vector3(0, Camera.main.transform.parent.transform.eulerAngles.y, 0));
            Slider.transform.position = Camera.main.WorldToScreenPoint(point);
        }
    }

    /// <summary>
    /// Rotates a point around a pivot by an angle clockwise
    /// </summary>
    /// <param name="point">Point to rotate</param>
    /// <param name="pivot">Point to rotate around</param>
    /// <param name="angle">Angle to rotate by clockwise</param>
    /// <returns>The rotated point</returns>
    Vector3 rotateAround(Vector3 point, Vector3 pivot, Vector3 angle) {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angle) * dir;
        point = dir + pivot;
        return point;
    }
}
