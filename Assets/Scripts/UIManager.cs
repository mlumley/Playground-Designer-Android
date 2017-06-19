using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Handeles displaying the content of the model panels
/// as well as fullscreen mode
/// </summary>
public class UIManager : MonoBehaviour {

    protected static UIManager _instance;

    public static UIManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType(typeof(UIManager)) as UIManager;

                if (_instance == null) {
                    Debug.LogError("Warning: there should always be an instance of UIManager in the scene.");
                }
            }
            return _instance;
        }
    }

    public Transform ObjectScrollViewContent;
    public Transform LandscapeScrollViewContent;
    public GameObject FullscreenButton;
    public Font openSans;


    /// <summary>
    /// Populates the elements panel with models corresponding to the
    /// selected category
    /// </summary>
    /// <param name="objects">Complete list of all models</param>
    /// <param name="mainCategory">Elements or Landscape models</param>
    /// <param name="catagory">Category to view</param>
    public void SetObjectData(List<DesignInfo> objects, string mainCategory, string catagory) {

        foreach(Transform child in ObjectScrollViewContent) {
            GameObject.Destroy(child.gameObject);
        }

        List<DesignInfo>PlaygroundObjects = new List<DesignInfo>(objects);

        foreach (DesignInfo info in PlaygroundObjects) {
            if ((info.ContainsCategory(catagory) || catagory == "All") && info.MainCategory == mainCategory) {
                GameObject buttonPrefab = Instantiate(Resources.Load("UIPrefabs/Preview Model")) as GameObject;
                buttonPrefab.transform.SetParent(ObjectScrollViewContent, false);
                buttonPrefab.GetComponent<ObjectSelectionButton>().PopulateButtonData(info);
                buttonPrefab.GetComponentInChildren<Text>().font = openSans;
                buttonPrefab.GetComponentInChildren<Text>().text = info.Name;
            }
        }

    }

    /// <summary>
    /// Populates the landscape panel with models corresponding to the
    /// selected category
    /// </summary>
    /// <param name="objects">Complete list of all models</param>
    /// <param name="mainCategory">Elements or Landscape models</param>
    /// <param name="catagory">Category to view</param>
    public void SetLandscapeData(List<DesignInfo> objects, string mainCategory, string catagory) {

        foreach (Transform child in LandscapeScrollViewContent) {
            GameObject.Destroy(child.gameObject);
        }

        List<DesignInfo> PlaygroundObjects = new List<DesignInfo>(objects);

        foreach (DesignInfo info in PlaygroundObjects) {
            if ((info.ContainsCategory(catagory) || catagory == "All") && info.MainCategory == mainCategory) {
                GameObject buttonPrefab = Instantiate(Resources.Load("UIPrefabs/Preview Model")) as GameObject;
                buttonPrefab.transform.SetParent(LandscapeScrollViewContent, false);
                buttonPrefab.GetComponent<ObjectSelectionButton>().PopulateButtonData(info);
                buttonPrefab.GetComponentInChildren<Text>().font = openSans;
                buttonPrefab.GetComponentInChildren<Text>().text = info.Name;
            }
        }

    }

    /// <summary>
    /// Toggles fullscreen mode and called a JS method when running
    /// in webgl to resize the canvas to fit the screen
    /// </summary>
    public void ToggleFullScreen() {
        Screen.fullScreen = !Screen.fullScreen;
        Application.ExternalCall("resize_canvas");
    }

    void Update() {
        // Swap the fullscreen icon depending on what mode we're in
        if(Screen.fullScreen)
            FullscreenButton.GetComponent<SpriteSwapManager>().ChangeSprite(2);
        else if(!Screen.fullScreen)
            FullscreenButton.GetComponent<SpriteSwapManager>().ChangeSprite(1);
    }
}
