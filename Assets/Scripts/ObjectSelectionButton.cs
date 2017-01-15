using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectSelectionButton : MonoBehaviour {

	public Image ColourBar;
	public Image ThisImage;

	public DesignInfo ObjectInfo;

	// Use this for initialization
	void Start ()
	{
        if(ObjectInfo.MainCategory == "Elements") {
            ColourBar.color = new Color32(223, 40, 125,255);
        }
        else {
            ColourBar.color = new Color32(238, 163, 75,255);
        }
    }

	public void PopulateButtonData (DesignInfo info)
	{
		ObjectInfo = info;

		ThisImage.sprite = Resources.Load<Sprite>("ObjectImages/" + info.ImageName);
	}

	
	public void SelectThisObject ()
	{
        Debug.Log(ObjectInfo.ModelName);
		PlayerManager.Instance.SetObject (ObjectInfo);
		//UIManager.Instance.DismissDesignPanel ();

	}

	public void PressedInfoButton ()
	{
		//ObjectInfoPanelManager.Instance.Populate (ObjectInfo);
	}
}
