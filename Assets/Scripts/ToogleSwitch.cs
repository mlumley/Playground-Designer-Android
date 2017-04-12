using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToogleSwitch : MonoBehaviour {

    private void Start() {
        gameObject.GetComponent<Toggle>().isOn = DataManager.Instance.PublicPlayground;
    }

    public void SetToggle(bool value) {
        gameObject.GetComponent<Toggle>().isOn = value;
    } 

    public void toggle() {
        Debug.Log(gameObject.GetComponent<Toggle>().isOn);
        if (gameObject.GetComponent<Toggle>().isOn) {
            DataManager.Instance.PublicPlayground = true;
        }
        else {
            DataManager.Instance.PublicPlayground = false;
        }
    }
}
