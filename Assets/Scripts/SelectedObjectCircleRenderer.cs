using UnityEngine;
using System.Collections;

public class SelectedObjectCircleRenderer : MonoBehaviour {

	protected static SelectedObjectCircleRenderer _instance;

	public static SelectedObjectCircleRenderer Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(SelectedObjectCircleRenderer)) as SelectedObjectCircleRenderer;

				if(_instance == null)
				{
					Debug.LogError("Warning: there should always be an instance of SelectedObjectCircleRenderer in the scene.");
				}
			}

			return _instance;
		}
	}

	private Transform selectedObjTrans;

	public Material GoodMaterial;
	public Material BadMaterial;

	private Material lineMaterial;

	float radius = 0f;

	public void SetSelectedObject (Transform trans)
	{
		selectedObjTrans = trans;

		if (!trans)
		{
			ObjectWorldPanel.Instance.SetTarget (null);
			RotateCirclesRenderer.Instance.SetSelectedObject (trans, radius);
			return;
		}

		radius = trans.GetComponent<BoxCollider> ().size.x > trans.GetComponent<BoxCollider> ().size.z ? trans.GetComponent<BoxCollider> ().size.x : trans.GetComponent<BoxCollider> ().size.z;
		radius /= 2f;

		lineMaterial = GoodMaterial;

		RotateCirclesRenderer.Instance.SetSelectedObject (trans, radius);
		ObjectWorldPanel.Instance.SetTarget (trans);
	}

	public void ValidatePositionColor (bool isValid)
	{
		lineMaterial = isValid ? GoodMaterial : BadMaterial;
	}

	void OnPostRender()
	{
		if (selectedObjTrans)
		{			
			lineMaterial.SetPass (0);

			GL.Begin (GL.LINES);

			Vector2 Circle = new Vector2 ();

			for (int i = 0; i < 360; i++)
			{
				Circle.x = radius * Mathf.Cos (i * Mathf.Deg2Rad) + selectedObjTrans.position.x;
				Circle.y = radius * Mathf.Sin (i * Mathf.Deg2Rad) + selectedObjTrans.position.z;
				GL.Vertex3 (Circle.x, 0, Circle.y);
			}

			GL.End ();
		}
	}
}
