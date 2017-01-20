using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour {

    int width;
    int height;
    GameObject tooltip;
    bool modelPanels = true;
    Vector3 mouseOffset;
    Vector3 screenOffset;
    bool fullScreen = false;


    public bool ModelPanels {
        set {
            modelPanels = value;
        }
    }

    // Use this for initialization
    void Start () {
        tooltip = transform.GetChild(0).gameObject;
        mouseOffset = new Vector3(0, -30, 0);
        screenOffset = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        //width = Screen.width;
        width = (int) gameObject.GetComponent<RectTransform>().rect.width;
        height = -Screen.height;
        if (modelPanels) {
            //Debug.Log("Models");
            tooltip.transform.localPosition = new Vector3(0, -490, 0);
        }
        else if (tooltip.activeSelf) {
            StartCoroutine(CheckBounds());
            tooltip.transform.localPosition = screenOffset + mouseOffset;
        }
	}

    public void SetTooltipText (string text) {
        tooltip.GetComponentInChildren<Text>().text = text;
        if (!modelPanels) {
            StartCoroutine(CheckBounds());
        }
    }

    IEnumerator CheckBounds() {
        yield return new WaitForEndOfFrame();
        float offsetX = 0;
        float offsetY = 0;
        Debug.Log("Width: " + width);
        //Debug.Log("Height: " + height);
        Vector2 point;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.GetComponent<RectTransform>(), Input.mousePosition, null, out point);
        Debug.Log("PosX: " + point);
        Debug.Log("WidthBox: " + tooltip.GetComponent<RectTransform>().rect.width);
        if (point.x + tooltip.GetComponent<RectTransform>().rect.width > width) {
            offsetX = point.x + tooltip.GetComponent<RectTransform>().rect.width - width;
            Debug.Log("OffsetX: " + offsetX);
        }
        if (point.y + tooltip.GetComponent<RectTransform>().rect.height < height) {
            offsetY = point.y + tooltip.GetComponent<RectTransform>().rect.height - height;
            //Debug.Log("OffsetY: " + offsetY);
        }
        screenOffset = new Vector3(point.x - offsetX, point.y - offsetY, 0);
        Debug.Log("screenOffset: " + screenOffset);
    }
}
