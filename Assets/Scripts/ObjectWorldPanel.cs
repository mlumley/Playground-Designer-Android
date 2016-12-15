using UnityEngine;
using System.Collections;

public class ObjectWorldPanel : MonoBehaviour {

	protected static ObjectWorldPanel _instance;

	public static ObjectWorldPanel Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(ObjectWorldPanel)) as ObjectWorldPanel;

				if(_instance == null)
				{
					Debug.LogError("Warning: there should always be an instance of ObjectWorldPanel in the scene.");
				}
			}

			return _instance;
		}
	}

	private Transform target;

	public GameObject PanelObject;
	public Vector3 PanelOffset;

	public void SetTarget (Transform targetTrans)
	{
		target = targetTrans;

		if (target)
		{
			PanelObject.SetActive (true);
		}
		else
		{
			PanelObject.SetActive (false);
		}
	}		
	void Update ()
	{
		if (target)
		{
			PanelObject.transform.position = Camera.main.WorldToScreenPoint (target.position + PanelOffset);
		}
	}
}
