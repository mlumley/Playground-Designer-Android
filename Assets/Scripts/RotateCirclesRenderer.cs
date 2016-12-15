using UnityEngine;
using System.Collections;

public class RotateCirclesRenderer : MonoBehaviour {

	protected static RotateCirclesRenderer _instance;

	public static RotateCirclesRenderer Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(RotateCirclesRenderer)) as RotateCirclesRenderer;

				if(_instance == null)
				{
					Debug.LogError("Warning: there should always be an instance of RotateCirclesRenderer in the scene.");
				}
			}

			return _instance;
		}
	}

	private Transform selectedObjTrans;

	public Material lineMaterial;

	public float Deg = 0f;

	float BigCircleRadius = 0f;
	float SmallCircleRadius = 0f;

	Vector2 centerPointCircle1;
	Vector2 centerPointCircle2;
	Vector2 centerPointCircle3;
	Vector2 centerPointCircle4;

	public void SetSelectedObject (Transform trans, float rad)
	{
		selectedObjTrans = trans;

		if (!trans) return;

		BigCircleRadius = rad;
		SmallCircleRadius = BigCircleRadius / 8f;
	}

	public void SetDegrees (float degreesToAdd)
	{
		Deg += degreesToAdd;

		selectedObjTrans.RotateAround (selectedObjTrans.position, Vector3.up, -degreesToAdd);
	}

	public bool HitRotateCircleCheck (Vector3 hitPoint)
	{
		if (Mathf.Pow(centerPointCircle1.x - hitPoint.x, 2f) + Mathf.Pow(centerPointCircle1.y - hitPoint.z, 2f) < Mathf.Pow(SmallCircleRadius, 2f))
		{
			return true;
		}
		else if (Mathf.Pow(centerPointCircle2.x - hitPoint.x, 2f) + Mathf.Pow(centerPointCircle2.y - hitPoint.z, 2f) < Mathf.Pow(SmallCircleRadius, 2f))
		{
			return true;
		}
		else if (Mathf.Pow(centerPointCircle3.x - hitPoint.x, 2f) + Mathf.Pow(centerPointCircle3.y - hitPoint.z, 2f) < Mathf.Pow(SmallCircleRadius, 2f))
		{
			return true;
		}
		else if (Mathf.Pow(centerPointCircle4.x - hitPoint.x, 2f) + Mathf.Pow(centerPointCircle4.y - hitPoint.z, 2f) < Mathf.Pow(SmallCircleRadius, 2f))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
		
	void OnPostRender()
	{
		if (selectedObjTrans)
		{			
			lineMaterial.SetPass (0);

			GL.Begin (GL.LINES);

			Vector2 miniCircle = new Vector2 ();

			centerPointCircle1 = new Vector2(BigCircleRadius * Mathf.Cos(Deg * Mathf.Deg2Rad) + selectedObjTrans.transform.position.x, BigCircleRadius * Mathf.Sin(Deg * Mathf.Deg2Rad) + selectedObjTrans.transform.position.z);
			for (int i = 0; i < 360; i+=10)
			{
				miniCircle.x = SmallCircleRadius * Mathf.Cos (i * Mathf.Deg2Rad) + centerPointCircle1.x;
				miniCircle.y = SmallCircleRadius * Mathf.Sin (i * Mathf.Deg2Rad) + centerPointCircle1.y;
				GL.Vertex3 (miniCircle.x, 0, miniCircle.y);
			}

			centerPointCircle2 = new Vector2(BigCircleRadius * Mathf.Cos((Deg + 90f) * Mathf.Deg2Rad) + selectedObjTrans.transform.position.x, BigCircleRadius * Mathf.Sin((Deg + 90f) * Mathf.Deg2Rad) + selectedObjTrans.transform.position.z);
			for (int i = 0; i < 360; i+=10)
			{
				miniCircle.x = SmallCircleRadius * Mathf.Cos (i * Mathf.Deg2Rad) + centerPointCircle2.x;
				miniCircle.y = SmallCircleRadius * Mathf.Sin (i * Mathf.Deg2Rad) + centerPointCircle2.y;
				GL.Vertex3 (miniCircle.x, 0, miniCircle.y);
			}

			centerPointCircle3 = new Vector2(BigCircleRadius * Mathf.Cos((Deg + 180f) * Mathf.Deg2Rad) + selectedObjTrans.transform.position.x, BigCircleRadius * Mathf.Sin((Deg + 180f) * Mathf.Deg2Rad) + selectedObjTrans.transform.position.z);
			for (int i = 0; i < 360; i+=10)
			{
				miniCircle.x = SmallCircleRadius * Mathf.Cos (i * Mathf.Deg2Rad) + centerPointCircle3.x;
				miniCircle.y = SmallCircleRadius * Mathf.Sin (i * Mathf.Deg2Rad) + centerPointCircle3.y;
				GL.Vertex3 (miniCircle.x, 0, miniCircle.y);
			}

			centerPointCircle4 = new Vector2(BigCircleRadius * Mathf.Cos((Deg + 270f) * Mathf.Deg2Rad) + selectedObjTrans.transform.position.x, BigCircleRadius * Mathf.Sin((Deg + 270f) * Mathf.Deg2Rad) + selectedObjTrans.transform.position.z);
			for (int i = 0; i < 360; i+=10)
			{
				miniCircle.x = SmallCircleRadius * Mathf.Cos (i * Mathf.Deg2Rad) + centerPointCircle4.x;
				miniCircle.y = SmallCircleRadius * Mathf.Sin (i * Mathf.Deg2Rad) + centerPointCircle4.y;
				GL.Vertex3 (miniCircle.x, 0, miniCircle.y);
			}

			GL.End ();
		}
	}
}
