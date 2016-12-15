using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldAnnotatePanel : MonoBehaviour, IUISelectable {

	public Text thisText;
	public Image BackgroundImage;
	public Button DeleteButton;
	public Button EditButton;

	float elapsedTime = 0f;
	float timeThreshold = 0.2f;

	bool selected = false;

	public Color startColor;

	Vector3 ThreeDReference = Vector3.zero;

	void Start ()
	{
		DeleteButton.onClick.AddListener (Delete);
		EditButton.onClick.AddListener (Edit);
	}

	public void SetText (string textToSet)
	{
		thisText.text = textToSet;

		if (textToSet.Length > 20)
			thisText.resizeTextForBestFit = true;
		else
			thisText.resizeTextForBestFit = false;
	}

	void Update ()
	{
		if (!selected)
		{
			elapsedTime += Time.deltaTime;

			if (elapsedTime > timeThreshold)
			{
				transform.position = Camera.main.WorldToScreenPoint (ThreeDReference);

				elapsedTime -= timeThreshold;
			}
		}
	}

	public void Select (bool isSelected)
	{
//		Debug.Log("ISSLEVCTED: " + isSelected);

		selected = isSelected;
		BackgroundImage.color = new Color (startColor.r, startColor.g, startColor.b, isSelected ? 1f : 0f);

		DeleteButton.gameObject.SetActive (isSelected);
		EditButton.gameObject.SetActive (isSelected);

		RaycastHit hitInfo;
		if (Physics.Raycast (Camera.main.ScreenPointToRay (transform.position), out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer ("GridCollider")))
		{
			ThreeDReference = hitInfo.point;
		}
		else
			ThreeDReference = Vector3.zero;

		if(isSelected)
			PlayerManager.Instance.currentObject = transform;
		
		PlayerManager.Instance.MoveableUISelected = isSelected ? true : false;
	}

	public void Delete ()
	{
		//PlayerManager.Instance.MoveableUISelected = false;
		Destroy (this.gameObject);
	}

	public void Edit ()
	{
		//Debug.Log ("PRESSING EDIT!!!!!");

		AnnotatePanelManager.Instance.SetPanel(this);
	}
}
