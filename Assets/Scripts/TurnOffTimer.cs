using UnityEngine;
using System.Collections;

public class TurnOffTimer : MonoBehaviour {

	public float TimeUntilTurnOff;

	void Start ()
	{
		StartCoroutine (StartTime());
	}

	IEnumerator StartTime ()
	{
		yield return new WaitForSeconds (TimeUntilTurnOff);

		Destroy (this.gameObject);
	}
}
