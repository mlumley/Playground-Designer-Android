using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the contents of the thumbnail image in the model selection panels
/// </summary>
public class ObjectSelectionButton : MonoBehaviour {

    public Image ColourBar;
    public Image ThisImage;

    public DesignInfo ObjectInfo;

    // Use this for initialization
    void Start() {
        if (ObjectInfo.MainCategory == "Elements") {
            ColourBar.color = new Color32(223, 40, 125, 255);
        }
        else {
            ColourBar.color = new Color32(238, 163, 75, 255);
        }
    }

    /// <summary>
    /// Fills out the info of the button using a DesignInfo object
    /// </summary>
    /// <param name="info">Info for the button</param>
    public void PopulateButtonData(DesignInfo info) {
        ObjectInfo = info;
        ThisImage.sprite = Resources.Load<Sprite>("ObjectImages/" + info.ImageName);
    }

    /// <summary>
    /// Select the object
    /// </summary>
    public void SelectThisObject() {
        Debug.Log(ObjectInfo.ModelName);
        PlayerManager.Instance.SpawnModel(ObjectInfo);
    }
}
