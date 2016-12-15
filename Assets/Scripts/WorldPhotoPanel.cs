using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldPhotoPanel : MonoBehaviour, IUISelectable {

	public GameObject DeleteButtonObj;
	public RawImage ThisRawImage;
	public Image BorderImage;


	public void Select (bool isSelected)
	{
		DeleteButtonObj.SetActive(isSelected);
		BorderImage.enabled = isSelected;

		if(isSelected)
			PlayerManager.Instance.currentObject = transform;
		
		PlayerManager.Instance.MoveableUISelected = isSelected ? true : false;
	}

	public void SetImage (Texture tex)
	{
		ThisRawImage.texture = tex;
	}

	public void Delete ()
	{
		Destroy (gameObject);
	}
}
