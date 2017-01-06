using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectSelectionButton : MonoBehaviour {

	Button thisButton;

	public Button InfoButton;
	public Image ThisImage;

	public DesignInfo ObjectInfo;

	// Use this for initialization
	void Start ()
	{
		thisButton = GetComponent<Button> ();
        //thisButton.onClick.AddListener (SelectThisObject);
        //InfoButton.onClick.AddListener (PressedInfoButton);
    }

	public void PopulateButtonData (DesignInfo info)
	{
		ObjectInfo = info;

		ThisImage.sprite = Resources.Load<Sprite>("ObjectImages/" + info.ImageName);
	}

	
	public void SelectThisObject ()
	{
        Debug.Log(ObjectInfo.ModelName);
		PlayerManager.Instance.SetObject (ObjectInfo.ModelName);
		UIManager.Instance.DismissDesignPanel ();
	}

	public void PressedInfoButton ()
	{
		//ObjectInfoPanelManager.Instance.Populate (ObjectInfo);
	}
}
