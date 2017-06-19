using UnityEngine;
using System.Collections;
// Include Facebook namespace
using Facebook.Unity;
using System;
using Assets.Scripts;

/// <summary>
/// Handle facebook sharing
/// </summary>
public class FacebookButton : MonoBehaviour {

    public SaveManager save;

    /// <summary>
    /// Uploads a screenshot to facebook by using the screenshot taken during
    /// saving, if the app is not saving it will trigger a save before uploading
    /// </summary>
    public void UploadImageToFacebook() {
        if(!DataManager.Instance.IsSaving)
            save.SavePlayground(false);
        StartCoroutine(Post());
    }

    /// <summary>
    /// Sends the image to facebook
    /// </summary>
    IEnumerator Post() {
        // Wait till saving has started and then wait for it to finish
        yield return new WaitUntil(() => DataManager.Instance.IsSaving == true);
        yield return new WaitUntil(() => DataManager.Instance.IsSaving == false);
        Debug.Log("FB called");
        FB.ShareLink(new Uri("http://playgroundideas.org"), "Playground Ideas", "Create your own playground", new Uri(DataManager.Instance.ScreenShotURL), ShareCallback);
    }

    /// <summary>
    /// Defined in facebook api for unity
    /// </summary>
    private void ShareCallback(IShareResult result) {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error)) {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!String.IsNullOrEmpty(result.PostId)) {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");
        }
    }


    /// <summary>
    /// Defined in facebook api for unity
    /// </summary>
    void Awake() {
        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback);
        }
        else {
            // Already initialized, signal an app activation App Event
            //FB.ActivateApp();
        }
    }

    /// <summary>
    /// Defined in facebook api for unity
    /// </summary>
    private void InitCallback() {
        if (FB.IsInitialized) {
            // Signal an app activation App Event
            //FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }
}
