using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Handles tooltip conext and placement
/// </summary>
public class TooltipController : MonoBehaviour {

    int width;
    int height;

    bool modelPanels = true;

    GameObject tooltip;

    Vector3 mouseOffset;
    Vector3 screenOffset;

    public GameObject Canvas;
    public GameObject panels;


    public bool ModelPanels {
        set {
            modelPanels = value;
        }
    }

    // Use this for initialization
    void Start () {
        tooltip = transform.GetChild(0).gameObject;
        // mouseOffset sets the tooltip to appear 30px below the mouse pointer
        mouseOffset = new Vector3(0, -30, 0);
        // screenOffset is used to store how much of the tool tip appears off screen
        screenOffset = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        width = (int) gameObject.GetComponent<RectTransform>().rect.width;
        height = -Screen.height;
        // if the mouse is over the model panels set the tooltip to
        // be above the panels
        if (modelPanels) {
            gameObject.transform.SetParent(panels.transform);
            tooltip.transform.localPosition = new Vector3(0, -490, 0);
        }
        // otherwise if the tooltip is visible check that it's on screen
        // if it isn't move the tooltiip so that it is
        else if (tooltip.activeSelf) {
            gameObject.transform.SetParent(Canvas.transform);
            StartCoroutine(CheckBounds());
            tooltip.transform.localPosition = screenOffset + mouseOffset;
        }
	}

    /// <summary>
    /// Set the text of the tooltip
    /// </summary>
    /// <param name="text">Tooltip text</param>
    public void SetTooltipText (string text) {
        tooltip.GetComponentInChildren<Text>().text = text;
        if (!modelPanels) {
            StartCoroutine(CheckBounds());
        }
    }

    /// <summary>
    /// Calculate the screenOffset needed so that the tooltip remains completely on screen
    /// </summary>
    IEnumerator CheckBounds() {
        yield return new WaitForEndOfFrame();
        float offsetX = 0;
        float offsetY = 0;
        Vector2 point;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.GetComponent<RectTransform>(), Input.mousePosition, null, out point);
        if (point.x + tooltip.GetComponent<RectTransform>().rect.width > width) {
            offsetX = point.x + tooltip.GetComponent<RectTransform>().rect.width - width;
        }
        if (point.y + tooltip.GetComponent<RectTransform>().rect.height < height) {
            offsetY = point.y + tooltip.GetComponent<RectTransform>().rect.height - height;
        }
        screenOffset = new Vector3(point.x - offsetX, point.y - offsetY, 0);
    }
}
