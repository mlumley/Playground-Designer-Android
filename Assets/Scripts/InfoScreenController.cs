using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoScreenController : MonoBehaviour {

    public Button okButton;
    private string buttonText;

    void Start() {
        buttonText = okButton.GetComponentInChildren<Text>().text;
        Debug.Log(buttonText);
    }

    public void ResetText() {
        okButton.GetComponentInChildren<Text>().text = buttonText;
    }
}
