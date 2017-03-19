using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts;

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
        if (infoScreenOff || !infoScreen.activeSelf) {
            infoScreenOff = true;
            currentTime -= Time.deltaTime;

            if (currentTime < 0) {
                Debug.Log("AutoSaving");
                saveManager.GetComponent<SaveManager>().SavePlayground();
                currentTime = interval;
            }
        }
	}
}
