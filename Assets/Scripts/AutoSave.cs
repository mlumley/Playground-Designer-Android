using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts;

public class AutoSave : MonoBehaviour {

    public float interval;
    public SaveManager saveManager;

    private float currentTime;

	// Use this for initialization
	void Start () {
        currentTime = interval;
	}
	
	// Update is called once per frame
	void Update () {
        currentTime -= Time.deltaTime;

        if(currentTime < 0) {
            Debug.Log("AutoSaving");
            saveManager.GetComponent<SaveManager>().SavePlayground();
            currentTime = interval;
        }
	}
}
