using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadPdf : MonoBehaviour {

	public Button ThisButton;

	void Start ()
	{
		ThisButton.onClick.AddListener (ShowPdf);
	}

	public void ShowPdf ()
	{
		Application.OpenURL("file:///c:/filename.PDF");
	}

}
