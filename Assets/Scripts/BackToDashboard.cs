using UnityEngine;
using System.Collections;

public class BackToDashboard : MonoBehaviour {

    public void ToDashboard() {
        Application.ExternalCall("goto_dashboard");
    }
}
