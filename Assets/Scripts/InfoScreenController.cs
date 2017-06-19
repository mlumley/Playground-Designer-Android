using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls what the info screen displays
/// </summary>
public class InfoScreenController : MonoBehaviour {

    public Button okButton;
    private string buttonText;

    // Get the text on the button
    void Start() {
        buttonText = okButton.GetComponentInChildren<Text>().text;
    }

    /// <summary>
    /// Reset the ok button text to the original text
    /// </summary>
    public void ResetText() {
        okButton.GetComponentInChildren<Text>().text = buttonText;
    }
}
