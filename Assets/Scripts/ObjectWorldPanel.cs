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

    public GameObject DeleteButton;
	private Transform target = null;
	public Vector3 PanelOffset;

	public void SetTarget (Transform targetTrans)
	{
		target = targetTrans;

		if (target)
		{
            DeleteButton.SetActive (true);
		}
		else
		{
            DeleteButton.SetActive (false);
		}
	}		
	void Update ()
	{
		if (target)
		{
            DeleteButton.transform.position = Camera.main.WorldToScreenPoint(target.position + PanelOffset);
		}
	}
}
