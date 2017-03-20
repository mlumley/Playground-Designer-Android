using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToogleSwitch : MonoBehaviour {

    private void Start() {
        gameObject.GetComponent<Toggle>().isOn = DataManager.Instance.PublicPlayground;
    }

    public void UpdateToggle() {
        gameObject.GetComponent<Toggle>().isOn = DataManager.Instance.PublicPlayground;
    } 

    public void toggle() {
        DataManager.Instance.PublicPlayground = !DataManager.Instance.PublicPlayground;
        gameObject.GetComponent<Toggle>().isOn = DataManager.Instance.PublicPlayground;
    }
}
