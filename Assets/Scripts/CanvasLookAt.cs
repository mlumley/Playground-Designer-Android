using UnityEngine;
using System.Collections;

public class CanvasLookAt : MonoBehaviour {


	void Update ()
	{
		transform.LookAt(Camera.main.transform.transform, Vector3.up);
	}
}
