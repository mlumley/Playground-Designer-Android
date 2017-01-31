using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using Assets.Scripts.Classes;
using System;
using System.IO;
using System.Runtime.InteropServices;

public class DataManager : MonoBehaviour {

    protected static DataManager _instance;

    public static DataManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType(typeof(DataManager)) as DataManager;

                if (_instance == null) {
                    Debug.LogError("Warning: there should always be an instance of DataManager in the scene.");
                }
            }
            return _instance;
        }
    }

    public string NameOfJsonFile;
    public string BaseUrlOfApi;
    public string UserId;
    public string DesignId;

    public static List<DesignInfo> objectInfoList;

    public Dropdown objectDropdown;

    public Dropdown landscapeDropdown;

    public static List<AssetBundle> modelBundles = new List<AssetBundle>();
    public static string[] names;

    private bool preexistingPlayground = false;

    public GameObject infoScreen;

    private string screenShotURL;
    public bool isSaving = false;

    [DllImport("__Internal")]
    private static extern void Resize();


    public InputField playgroundName;
    public InputField width;
    public InputField height;


    public string ScreenShotURL {
        get {
            return screenShotURL;
        }
    }


    void Start() {
        StartCoroutine(LoadData());

        //todo: get ID in web player (eg https://docs.unity3d.com/ScriptReference/Application-absoluteURL.html)
        string userId = "1"; //test user
        int savedPlaygroundId = 0; //not saved

        //string url = "http://playgroundideas.endzone.io/app-api/wp-simulate/app.php?userId=1&designId=2";

        Debug.Log("Initial userId: " + userId + ", savedPlaygroundId: " + savedPlaygroundId);

        string[] names1 = { "balance", "bridges", "buildings", "climbing", "cubbies", "furniture", "ground_cover", "hills", "imaginative", "ladders", "monkey_bars", "musical", "natural", "other", "poles", "rocks", "sandpits", "seesaws", "slides", "sports", "swings", "trees", "tunnels" };
        names = names1;


        foreach (string name in names) {
            //StartCoroutine(Download(name));
            StartCoroutine(DownloadAndCache(name));
        }


#if UNITY_WEBGL
        string url = Application.absoluteURL;
        Debug.Log("Application.absoluteURL: " + url);

        var nc = UriHelper.GetQueryString(url);
        if (!String.IsNullOrEmpty(nc["userId"]))
            userId = nc["userId"];

        Debug.Log("WebGL userId from URL: " + userId);

        if (!String.IsNullOrEmpty(nc["designId"]))
            savedPlaygroundId = Convert.ToInt16(nc["designId"]);

        Debug.Log("WebGL savedPlaygroundId from URL: " + savedPlaygroundId);
#endif

        this.UserId = userId;
        this.DesignId = savedPlaygroundId.ToString();

        //StartCoroutine(LoadUser(userId));
        if (savedPlaygroundId != 0) {
            Debug.Log("Loading saved playground");
            preexistingPlayground = true;
            StartCoroutine(WaitTillDownloadedAssets(preexistingPlayground));
            StartCoroutine(LoadSavedPlayground(savedPlaygroundId));
        }
        else {
            StartCoroutine(WaitTillDownloadedAssets(preexistingPlayground));
        }
    }

    void Update() {
        if (playgroundName.isFocused || width.isFocused || height.isFocused)
            CameraPositions.Instance.isTyping = true;
        else
            CameraPositions.Instance.isTyping = false;
    }

    IEnumerator WaitTillDownloadedAssets(bool isExsitingPlayground) {
        Button okButton = infoScreen.GetComponentInChildren<Button>();

        if (isExsitingPlayground) {
            okButton.GetComponentInChildren<Text>().text = "Loading your Playground";
        }
        else {
            okButton.GetComponentInChildren<Text>().text = "Loading Models";
        }

        yield return new WaitUntil(() => modelBundles.Count == names.Length);
        infoScreen.SetActive(true);
        okButton.interactable = true;
        okButton.GetComponentInChildren<Text>().text = "Ready!";
    }

    public IEnumerator GetScreenShotURL() {
        yield return new WaitUntil(() => isSaving == false);
        string jsonUrl = BaseUrlOfApi + "/playgrounds/get.php?userId=" + UserId;
        string json = "";

        WWW www = new WWW(jsonUrl);
        yield return www;

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.LogError("LoadUser API call error (" + jsonUrl + "): " + www.error);
            yield break;
        }

        json = www.text;

        JSONNode N = JSON.Parse(json);
        Debug.Log("Full Json returned from API: " + json);

        

        yield return "http://playgroundideas.endzone.io/app-api/uploads/" + N["screenShot"];
        Debug.Log("ScreenshotURL is " + DataManager.Instance.ScreenShotURL);
    }


    IEnumerator LoadSavedPlayground(int savedPlaygroundId) {
        string jsonUrl = BaseUrlOfApi + "/playgrounds/get.php?id=" + savedPlaygroundId;
        string json = "";

        WWW www = new WWW(jsonUrl);
        yield return www;

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.LogError("LoadUser API call error (" + jsonUrl + "): " + www.error);
            yield break;
        }

        json = www.text;

        JSONNode N = JSON.Parse(json);

        if (N["status"].Value != "true") {
            Debug.LogError("Playground not found.");
            yield break;
        }
        else {
            Debug.Log("Full Json returned from API: " + json);
            Debug.Log("Model Json returned from API: " + N["playground"]["model"] + " Name: " + N["name"]);
            //todo: parse the model
            /*using (WWW assetBundleLink = new WWW(BaseUrlOfApi + "wp-simulate/models")) {
                yield return assetBundleLink;
                if (assetBundleLink.error != null)
                    throw new Exception("WWW download had an error:" + assetBundleLink.error);
                modelBundle = assetBundleLink.assetBundle;
            }*/
            yield return new WaitUntil (() => modelBundles.Count == names.Length);
            LoadSaveFile(N["playground"]["name"], N["playground"]["model"]);

        }
    }

    IEnumerator LoadUser(string userId) {
        string jsonUrl = BaseUrlOfApi + "/users/get.php?id=" + userId;
        string json = "";

        WWW www = new WWW(jsonUrl);
        yield return www;

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.Log("LoadUser API call error (" + jsonUrl + "): " + www.error);
            yield break;
        }

        json = www.text;

        JSONNode N = JSON.Parse(json);

        if (N["status"].Value != "true") {
            //todo: This is required in the Web version, how do we handle the error when user ID is not passed?
            Debug.LogError("User not found.");
            yield break;
        }

        User user = User.Parse(N);
        Debug.Log("User found: " + user.Name);


    }


    public IEnumerator SavePlayground(string name, string saveFile) {
        isSaving = true;
#if UNITY_EDITOR
        Debug.Log("WaitForEndOfFrame doesn't work in editor");
#elif UNITY_WEBGL
        yield return null;
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        yield return new WaitForEndOfFrame();
#endif

        string apiUrl = BaseUrlOfApi + "/playgrounds/save.php";
        string json = "";


        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;

        // Comment out for deploy
        //Debug.Log(Application.dataPath + "/SavedScreen.png");
        //File.WriteAllBytes(Application.dataPath + "/SavedScreen.png", bytes);

        // Create a Web Form
        WWWForm form = new WWWForm();
        form.AddField("userId", UserId);
        form.AddField("name", name);
        if (preexistingPlayground) {
            Debug.Log("DesignID " + DesignId);
            form.AddField("designId", DesignId);
        }
        else {
            preexistingPlayground = true;
        }
        //form.AddBinaryData("fileUpload", bytes, "screenShot.png", "image/png");
        //Debug.Log(saveFile);
        form.AddField("model", saveFile);
        form.AddBinaryData("screenshot", bytes, "screenShot_" + name + ".png", "image/png");

        //Debug.Log("Saving: Name: " + name + " User: " + UserId + " Model: " + saveFile);

        WWW www = new WWW(apiUrl, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.Log("SavePlayground API call error: " + www.error);
            yield break;
        }

        json = www.text;
        Debug.Log(json);

        JSONNode N = JSON.Parse(json);

        DesignId = N["playground"]["id"];
        screenShotURL = N["playground"]["screenshot_Url"];
        //Debug.Log("ScreenshotURL is " + DataManager.Instance.ScreenShotURL);


        if (N["status"].Value != "true") {
            Debug.LogError("Error with saving.");
            yield break;
        }
        isSaving = false;
        //Debug.Log(isSaving);
    }



    IEnumerator LoadData() {
        if (string.IsNullOrEmpty(NameOfJsonFile)) {
            Debug.LogError("You havent specified a json file to load.");
            yield break;
        }

        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, NameOfJsonFile);
        string result = "";

        if (filePath.Contains("://")) {
            WWW www = new WWW(filePath);
            yield return www;
            result = www.text;
        }
        else {
            result = System.IO.File.ReadAllText(filePath);
        }

        JSONNode N = JSON.Parse(result);

        objectInfoList = new List<DesignInfo>();

        List<string> objectCatagories = new List<string>();
        List<string> landscapeCatagories = new List<string>();

        for (int i = 0; i < N["Objects"].Count; i++) {
            objectInfoList.Add(new DesignInfo(N["Objects"][i]["Name"].Value, N["Objects"][i]["ImageName"].Value, N["Objects"][i]["ModelName"].Value, N["Objects"][i]["MainCategory"].Value, N["Objects"][i]["Category"].Value));
            //Debug.Log((N["Objects"][i]["MainCategory"].Value));
            if (N["Objects"][i]["MainCategory"].Value == "Elements" && !objectCatagories.Contains(N["Objects"][i]["Category"].Value))
                objectCatagories.Add(N["Objects"][i]["Category"].Value);
            else if (N["Objects"][i]["MainCategory"].Value == "Landscape" && !landscapeCatagories.Contains(N["Objects"][i]["Category"].Value))
                landscapeCatagories.Add(N["Objects"][i]["Category"].Value);
        }

        objectInfoList.Sort();

        //Debug.Log(objectCatagories.Count);

        UIManager.Instance.SetObjectData(objectInfoList, "Elements", "None");
        UIManager.Instance.SetLandscapeData(objectInfoList, "Landscape", "None");
        objectDropdown.GetComponent<DropdownSorter>().setCatagories(objectCatagories);
        landscapeDropdown.GetComponent<DropdownSorter>().setCatagories(landscapeCatagories);
    }


    public void LoadSaveFile(string name, string saveJSON) {
        SaveFile saveFile = JsonUtility.FromJson<SaveFile>(saveJSON);
        //Debug.Log(name);
        playgroundName.text = name;
        width.text = saveFile.width.ToString();
        height.text = saveFile.length.ToString();
        ModelData[] models = saveFile.models;
        for (int i = 0; i < models.Length; i++) {
            StartCoroutine(PlayerManager.LoadObjectAtPositionAndRotation(objectInfoList, models[i].name, models[i].position, models[i].rotation, models[i].scale));
        }

        PhotoData[] photos = saveFile.photos;
        foreach(PhotoData photo in photos) {
            PlayerManager.LoadPhoto(photo.position, photo.rotation, photo.scale, photo.image);
        }
    }

    // Not in use
    IEnumerator Download(string bundleName) {
        // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        //using (WWW www = WWW.LoadFromCacheOrDownload(BaseUrlOfApi + "wp-simulate/models", 1)) {
        Debug.Log("Started downloading " + bundleName);
        using (WWW www = new WWW(BaseUrlOfApi + "wp-simulate/AssetBundles/" + bundleName)) {
            yield return www;
            Debug.Log("Finished downloading " + bundleName);
            if (www.error != null)
                throw new Exception("WWW download had an error:" + www.error);
            modelBundles.Add(www.assetBundle);
        }
    }

    IEnumerator DownloadAndCache(string bundleName) {
        while (!Caching.ready)
            yield return null;
        //Debug.Log("Started downloading " + bundleName);
        // Increment version number with new assest bundles
        var www = WWW.LoadFromCacheOrDownload(BaseUrlOfApi + "wp-simulate/AssetBundles/" + bundleName, 4);
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) {
            Debug.Log(www.error);
            yield return null;
        }
        //Debug.Log("Finished downloading " + bundleName);
        modelBundles.Add(www.assetBundle);
    }
}
