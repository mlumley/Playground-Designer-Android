using UnityEngine;
using Assets.Scripts;

/// <summary>
/// Controls auto saving
/// </summary>
public class AutoSave : MonoBehaviour {

    public float interval;
    public SaveManager saveManager;
    public GameObject infoScreen;

    private float currentTime;
    private bool infoScreenOff = false;

	// Use this for initialization
	void Start () {
        currentTime = interval;
	}
	
	// Update is called once per frame
	void Update () {
        // Make sure to only start auto saving when the info screen has be dismissed
        if (infoScreenOff || !infoScreen.activeSelf) {
            infoScreenOff = true;
            currentTime -= Time.deltaTime;

            // When the timer reaches zero save and reset
            if (currentTime < 0) {
                Debug.Log("AutoSaving");
                saveManager.GetComponent<SaveManager>().SavePlayground();
                currentTime = interval;
            }
        }
	}
}
