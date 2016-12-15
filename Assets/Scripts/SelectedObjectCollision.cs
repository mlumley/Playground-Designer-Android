using UnityEngine;
using System.Collections;

public class SelectedObjectCollision : MonoBehaviour 
{
	void OnTriggerEnter(Collider other)
	{
		Debug.Log("OnColliderEnter");
	}

	void OnTriggerExit(Collider other)
	{
		Debug.Log("OnColliderExit");
	}
}