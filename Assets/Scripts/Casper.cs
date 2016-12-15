using UnityEngine;
using System.Collections;

public class Casper : MonoBehaviour {

	private Vector3 mousePosition;




	void Update () 
	{
		if (!Input.GetMouseButton(0))
			mousePosition = Input.mousePosition;
			mousePosition.z = 10f; // Set this to be the distance you want the object to be placed in front of the camera.
			this.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
			//gameObject.SetActive(false);
	}
}
