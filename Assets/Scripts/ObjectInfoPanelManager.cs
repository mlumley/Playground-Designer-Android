using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectInfoPanelManager : MonoBehaviour {

	protected static ObjectInfoPanelManager _instance;

	public static ObjectInfoPanelManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(ObjectInfoPanelManager)) as ObjectInfoPanelManager;

				if(_instance == null)
				{
					Debug.LogError("Warning: there should always be an instance of ObjectInfoPanelManager in the scene.");
				}
			}
			return _instance;
		}
	}

	public GameObject thisPanelObject;

	public Text InfoPanelHeader;
	public Text InfoPanelBody;

	public Transform PreviewTrans;

	private GameObject objectPreview;

	void BringIn ()
	{
		thisPanelObject.SetActive (true);
	}

	public void Dismiss ()
	{
		thisPanelObject.SetActive (false);
		Destroy (objectPreview);
	}

	public void Populate (DesignInfo info)
	{
		InfoPanelHeader.text = info.Name;

		objectPreview = Instantiate(Resources.Load ("ModelPrefabs/" + info.ModelName)) as GameObject;
		objectPreview.transform.position = PreviewTrans.position;
		objectPreview.transform.rotation = PreviewTrans.rotation;
		BringIn ();
	}

}
