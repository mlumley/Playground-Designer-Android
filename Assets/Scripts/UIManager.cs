using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
    public GameObject ObjectSelectionPanel;

    public Animator DesignButtonAnimator;
    public Animator DesignPanelAnimator;

    public GameObject DesignPanelObj;

    public void SetObjectData(List<DesignInfo> objects, string mainCategory, string catagory) {

        foreach(Transform child in ObjectScrollViewContent) {
            GameObject.Destroy(child.gameObject);
        }

        List<DesignInfo>PlaygroundObjects = new List<DesignInfo>(objects);

        foreach (DesignInfo info in PlaygroundObjects) {
            if ((info.Category == catagory || catagory == "All") && info.MainCategory == mainCategory) {
                GameObject buttonPrefab = Instantiate(Resources.Load("UIPrefabs/Preview Model")) as GameObject;
                buttonPrefab.transform.SetParent(ObjectScrollViewContent, false);
                buttonPrefab.GetComponent<ObjectSelectionButton>().PopulateButtonData(info);
                buttonPrefab.GetComponentInChildren<Text>().text = info.Name;
            }
        }

    }

    public void SetLandscapeData(List<DesignInfo> objects, string mainCategory, string catagory) {

        foreach (Transform child in LandscapeScrollViewContent) {
            GameObject.Destroy(child.gameObject);
        }

        List<DesignInfo> PlaygroundObjects = new List<DesignInfo>(objects);

        foreach (DesignInfo info in PlaygroundObjects) {
            if ((info.Category == catagory || catagory == "All") && info.MainCategory == mainCategory) {
                GameObject buttonPrefab = Instantiate(Resources.Load("UIPrefabs/Preview Model")) as GameObject;
                buttonPrefab.transform.SetParent(LandscapeScrollViewContent, false);
                buttonPrefab.GetComponent<ObjectSelectionButton>().PopulateButtonData(info);
                buttonPrefab.GetComponentInChildren<Text>().text = info.Name;
            }
        }

    }


    #region DESIGN PANEL 

    public void BringOutDesignButton() {
        DesignButtonAnimator.SetBool("BringIn", true);

        Invoke("BringInDesignPanel", 0.1f);
    }

    void BringInDesignPanel() {
        DesignPanelAnimator.SetBool("BringIn", true);
    }

    public void DismissDesignPanel() {
        DesignPanelObj.SetActive(false);

        //		DesignPanelAnimator.SetBool ("BringIn", false);
        //
        //		Invoke ("DismissDesignButton", 0.3f);
    }

    void DismissDesignButton() {
        DesignButtonAnimator.SetBool("BringIn", false);
    }

    #endregion

    public void ToggleFullScreen() {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
