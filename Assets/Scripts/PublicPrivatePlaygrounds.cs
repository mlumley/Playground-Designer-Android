using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Toggles between public and private playgrounds
/// </summary>
public class PublicPrivatePlaygrounds : MonoBehaviour {

    private void Start() {
        gameObject.GetComponent<Toggle>().isOn = DataManager.Instance.PublicPlayground;
    }

    /// <summary>
    /// Set toggle active or inactive
    /// </summary>
    /// <param name="value">Active</param>
    public void SetToggle(bool value) {
        gameObject.GetComponent<Toggle>().isOn = value;
    } 

    /// <summary>
    /// Toggles on and off the public or private playgrounds
    /// </summary>
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
