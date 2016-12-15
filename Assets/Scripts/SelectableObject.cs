using UnityEngine;
using System.Collections;

public class SelectableObject : MonoBehaviour {

	public Material mat;



	void Start ()
	{

	}

	void OnPostRender()
	{
		if (!mat)
		{
			Debug.LogError("Please Assign a material on the inspector");
			return;
		}

		GL.PushMatrix();
		mat.SetPass(0);
		GL.LoadOrtho();
		GL.Begin(GL.LINES);
		GL.Vertex3(0, 0, 0);
		GL.Vertex3(1, 1, 0);
		GL.End();
		GL.PopMatrix();
	}
}
