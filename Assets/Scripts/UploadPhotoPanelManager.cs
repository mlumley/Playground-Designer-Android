using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UploadPhotoPanelManager : MonoBehaviour {

	protected static UploadPhotoPanelManager _instance;

	public static UploadPhotoPanelManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UploadPhotoPanelManager)) as UploadPhotoPanelManager;

				if(_instance == null)
				{
					Debug.LogError("Warning: there should always be an instance of UploadPhotoPanelManager in the scene.");
				}
			}
			return _instance;
		}
	}

	public GameObject UploadPhotoPanelObj;
	public Canvas MainCanvas;
	public RawImage ThisRawImage;
	public RectTransform ThisRawImageRect;
	public RectTransform InsetPanel;

	Vector2 worldPanelSize = new Vector2();

	public void ResizePanel (Vector2 texSize)
	{
		Debug.Log("Texture size: " + texSize);

		float ratio = 0f;
		Vector2 insetPanelSize = InsetPanel.rect.size;

		if(texSize.x > texSize.y)
		{
			ratio = texSize.y / texSize.x;

			ThisRawImageRect.sizeDelta = new Vector2(insetPanelSize.x, insetPanelSize.x * ratio);

		}
		else if(texSize.x < texSize.y)
		{
			ratio = texSize.x / texSize.y;

			ThisRawImageRect.sizeDelta = new Vector2(insetPanelSize.y * ratio, insetPanelSize.y);
		}
		else
		{
			ThisRawImageRect.sizeDelta = new Vector2(insetPanelSize.x, insetPanelSize.y);
		}

		worldPanelSize = new Vector2(ThisRawImageRect.sizeDelta.x / 8f, ThisRawImageRect.sizeDelta.y / 8f);

	}

	public void CreatePhoto ()
	{
		UploadPhotoPanelObj.SetActive (false);

		GameObject PhotoWorldPanelPrefab = Instantiate(Resources.Load ("UIPrefabs/WorldPhotoPanel")) as GameObject;
		PhotoWorldPanelPrefab.transform.SetParent (MainCanvas.transform);
		PhotoWorldPanelPrefab.transform.SetAsFirstSibling();
		PhotoWorldPanelPrefab.transform.localPosition = Vector3.zero;
		PhotoWorldPanelPrefab.GetComponent<RectTransform>().sizeDelta = worldPanelSize;
		WorldPhotoPanel worldPhotoPanel = PhotoWorldPanelPrefab.GetComponent<WorldPhotoPanel>();
		worldPhotoPanel.SetImage(ThisRawImage.texture);
		worldPhotoPanel.Select(true);
	}
}
