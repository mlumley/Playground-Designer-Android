using UnityEngine;
using System.Collections;
// Include Facebook namespace
using Facebook.Unity;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Assets.Scripts;
using System.Runtime.InteropServices;

public class FacebookButton : MonoBehaviour {

    public GameObject postPanel;
    public Image screenshot;

    private InputField caption;
    private Button postButton;
    private List<string> perms = new List<string>() { "public_profile", "publish_actions" };
    private byte[] bytes;

    public SaveManager save;

    [DllImport("__Internal")]
    private static extern void FacebookLogInCaptureClick(string _contentURL, string _contentTitle, string _contentDesc, string _contentPhoto, FacebookDelegate<IShareResult> _callbackId);

    public void UploadImageToFacebook() {
        //FB.LogInWithReadPermissions(perms, AuthCallback);
        //StartCoroutine(TakeImage());
        if(!DataManager.Instance.isSaving)
            save.SavePlayground(false);
        //StartCoroutine(DataManager.Instance.GetScreenShotURL());
        //FB.ShareLink(new Uri("http://playgroundideas.endzone.io/app-api/wp-simulate/Build/app.php"), "Playground Ideas", "Create your own playground", new Uri(DataManager.Instance.ScreenShotURL), ShareCallback);
        StartCoroutine(Post());
    }

    IEnumerator Post() {
        yield return new WaitUntil(() => DataManager.Instance.isSaving == false);
        Debug.Log("FB called");
        //FB.ShareLink(new Uri("http://staging.playgroundideas.org"), "Playground Ideas", "Create your own playground", new Uri(DataManager.Instance.ScreenShotURL), ShareCallback);
        FacebookLogInCaptureClick("http://staging.playgroundideas.org", "Playground Ideas", "Create your own playground", DataManager.Instance.ScreenShotURL, ShareCallback);
    }

    public void PostToFacebook() {

        var wwwForm = new WWWForm();
        wwwForm.AddField("caption", caption.text + "\nMake your own at: http://playgroundideas.endzone.io/app-api/wp-simulate/Build/app.php");
        wwwForm.AddBinaryData("image", bytes, "Screenshot.png");

        FB.API("me/photos", HttpMethod.POST, APICallback, wwwForm);
        caption.text = "";
        postPanel.SetActive(false);
    }

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


    // Awake function from Unity's MonoBehavior
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

    void Start() {
        //caption = postPanel.GetComponentInChildren<InputField>();
        //postButton = postPanel.GetComponentInChildren<Button>();
    }

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


    private void AuthCallback(ILoginResult result) {
        if (FB.IsLoggedIn) {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions) {
                Debug.Log(perm);
            }
        }
        else {
            Debug.Log("User cancelled login");
            postPanel.SetActive(false);
            StopCoroutine(TakeImage());
        }
    }

    IEnumerator TakeImage() {

        yield return new WaitUntil(() => FB.IsLoggedIn == true);
#if UNITY_EDITOR
        Debug.Log("WaitForEndOfFrame doesn't work in editor");
#elif UNITY_WEBGL
        
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        yield return new WaitForEndOfFrame();
#endif
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        screenshot.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        bytes = tex.EncodeToPNG();
        //Destroy(tex);
        string bytestring = Convert.ToBase64String(bytes);


        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
        postPanel.SetActive(true);
    }

    private void APICallback(IGraphResult result) {
        Debug.Log("API result: " + result.RawResult);
    }
}
