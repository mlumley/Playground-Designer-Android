using UnityEngine;

/// <summary>
/// Tells the browser to go back to the dashboard of the website
/// </summary>

public class BackToDashboard : MonoBehaviour {

    // Call the JS function on the web page
    public void ToDashboard() {
        Application.ExternalCall("goto_dashboard");
    }
}
