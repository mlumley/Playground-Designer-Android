using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AnnotatePanelManager : MonoBehaviour {

	protected static AnnotatePanelManager _instance;

	public static AnnotatePanelManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(AnnotatePanelManager)) as AnnotatePanelManager;

				if(_instance == null)
				{
					Debug.LogError("Warning: there should always be an instance of AnnotatePanelManager in the scene.");
				}
			}
			return _instance;
		}
	}

	public Canvas MainCanvas;
	public GameObject AnnotatePanel;
	public InputField AnnotateInputField;

	WorldAnnotatePanel currenPanel;
	
	public void SetPanel (WorldAnnotatePanel panel)
	{
		currenPanel = panel;
		AnnotateInputField.text = currenPanel.thisText.text;
		AnnotatePanel.SetActive(true);
	}

	public void CreatePanel ()
	{
		if (string.IsNullOrEmpty (AnnotateInputField.text))
		{
			Close();
			return;
		}

		if(!currenPanel)
		{
			GameObject annotatePanelPrefab = Instantiate(Resources.Load ("UIPrefabs/WorldAnnotatePanel")) as GameObject;
			annotatePanelPrefab.transform.SetParent (MainCanvas.transform);
			annotatePanelPrefab.transform.SetAsFirstSibling();
			annotatePanelPrefab.transform.localPosition = Vector3.zero;
			currenPanel = annotatePanelPrefab.GetComponent<WorldAnnotatePanel>();
			currenPanel.Select(true);
		}

		currenPanel.SetText(AnnotateInputField.text);
		Close();
		AnnotateInputField.text = "";
	}

	public void Open ()
	{
		if(currenPanel)
		{
			currenPanel.Select(false);
			currenPanel = null;
		}

		AnnotatePanel.SetActive(true);
	}

	void Close ()
	{
		AnnotatePanel.SetActive(false);
	}
}
