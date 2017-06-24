using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using Assets.Scripts.Classes;
using System;
using System.Runtime.InteropServices;


/// <summary>
/// Handles reading and save data for playgrounds and the model manifest
/// </summary>
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
    public string BaseUrlOfDesigner;
    string UserId;
    string DesignId;
    private static bool publicPlayground = false;
    public PublicPrivatePlaygrounds playgroundSwitch;

    public static List<DesignInfo> objectInfoList;

    public Dropdown objectDropdown;

    public Dropdown landscapeDropdown;

    public static List<AssetBundle> modelBundles = new List<AssetBundle>();
    static string[] names = { "balance", "bridges", "buildings", "climbing", "cubbies", "furniture", "ground_cover", "hills", "imaginative", "ladders", "monkey_bars", "musical", "natural", "other", "people", "poles", "rocks", "sandpits", "seesaws", "slides", "sports", "swings", "trees", "tunnels" };

    private bool preexistingPlayground = false;

    public GameObject infoScreen;

    private string screenShotURL;
    bool isSaving = false;

    public InputField playgroundName;
    public InputField width;
    public InputField height;


    public string ScreenShotURL {
        get {
            return screenShotURL;
        }
    }

    public bool PublicPlayground{
        get {
            return publicPlayground;
        }
        set {
            publicPlayground = value;
        }
    }

    public bool IsSaving {
        get {
            return isSaving;
        }
        set {
            isSaving = value;
        }
    }


    void Start() {
        // Load from model manifest
        StartCoroutine(LoadData());
        string userId = "1"; //test user
        int savedPlaygroundId = 0; //not saved
        string creatorId = "0";
        Debug.Log("Initial userId: " + userId + ", savedPlaygroundId: " + savedPlaygroundId);

#if UNITY_EDITOR

// If deployed on to webgl get the user id and design id from the url
// also get the address to the api and the designer app itself
#elif UNITY_WEBGL
        string url = Application.absoluteURL;
        Debug.Log("Application.absoluteURL: " + url);

        var nc = UriHelper.GetQueryString(url);

        if (!String.IsNullOrEmpty(nc["creatorId"]))
            creatorId = nc["creatorId"];

        Debug.Log("WebGL savedPlaygroundId from URL: " + savedPlaygroundId);

        if (!String.IsNullOrEmpty(nc["userId"]))
            userId = nc["userId"];

        Debug.Log("WebGL userId from URL: " + userId);

        if (!String.IsNullOrEmpty(nc["designId"]))
            savedPlaygroundId = Convert.ToInt16(nc["designId"]);

        Debug.Log("WebGL savedPlaygroundId from URL: " + savedPlaygroundId);

        string[] stringSeparators = new string[] {"designer"};
        BaseUrlOfApi = url.Split(stringSeparators, StringSplitOptions.None)[0];
        BaseUrlOfApi = BaseUrlOfApi + "designer_api/";
        BaseUrlOfDesigner = url.Split(stringSeparators, StringSplitOptions.None)[0];
        BaseUrlOfDesigner = BaseUrlOfDesigner + "designer/";
        Debug.Log("Base URL of API is:" + BaseUrlOfApi);
        Debug.Log("Base URL of designer is:" + BaseUrlOfDesigner);
#endif
        // Download all assetbundles
        foreach (string name in names) {
            StartCoroutine(DownloadAndCache(name));
        }

        this.UserId = userId;
        this.DesignId = savedPlaygroundId.ToString();

        // If a creatorId is provided download that playground and set the designId to 0
        // so that when the users saves the playground is saved to their own account
        if (creatorId != "0" && savedPlaygroundId != 0) {
            Debug.Log("Loading public playground");
            StartCoroutine(WaitTillDownloadedAssets(preexistingPlayground));
            StartCoroutine(LoadSavedPlayground(creatorId,savedPlaygroundId));
            StartCoroutine(AddView(creatorId, DesignId));
            DesignId = "0";
        }
        // If the designId is provided load the saved playground
        else if (savedPlaygroundId != 0) {
            Debug.Log("Loading saved playground");
            preexistingPlayground = true;
            StartCoroutine(WaitTillDownloadedAssets(preexistingPlayground));
            StartCoroutine(LoadSavedPlayground(this.UserId, savedPlaygroundId));
            StartCoroutine(AddView(UserId, DesignId));
        }
        // Otherwise wait until all models are downloaded before leting the user use the app
        else {
            StartCoroutine(WaitTillDownloadedAssets(preexistingPlayground));
        }
    }

    void Update() {

        // If the user has a text box selected disable WASD input for camera movement
        if (playgroundName.isFocused || width.isFocused || height.isFocused)
            CameraPositions.Instance.isTyping = true;
        else
            CameraPositions.Instance.isTyping = false;
    }

    /// <summary>
    /// Increments the view count for a playground so that it shows at the top of the
    /// community designs section of the website
    /// </summary>
    /// <param name="UserId">User ID of the playground owner</param>
    /// <param name="DesignId">Design ID of the playground</param>
    IEnumerator AddView(string UserId, string DesignId) {
        WWW www = new WWW(BaseUrlOfApi + "playgrounds/views.php?UserId=" + UserId + "&DesignId=" + DesignId);

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.LogError("View API call error " + www.error);
            yield break;
        }
    }

    /// <summary>
    /// Disable the Ready button on the info screen when the app is loaded
    /// until all assetbundles have been downloaded
    /// </summary>
    /// <param name="isExsitingPlayground">Is the app opening a saved playground</param>
    /// <returns></returns>
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

    /// <summary>
    /// Load a saved playground into the world
    /// </summary>
    /// <param name="UserId">User ID of the playground owner</param>
    /// <param name="savedPlaygroundId">Design ID of the playground</param>
    IEnumerator LoadSavedPlayground(string UserId, int savedPlaygroundId) {
        string jsonUrl = BaseUrlOfApi + "/playgrounds/get.php?id=" + savedPlaygroundId;
        string json = "";

        WWW www = new WWW(jsonUrl);
        yield return www;

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.LogError("LoadUser API call error (" + jsonUrl + "): " + www.error);
            yield break;
        }

        json = www.text;
        Debug.Log(json);

        JSONNode N = JSON.Parse(json);

        Debug.Log(N["playground"]["public"]);

        if (N["playground"]["public"].AsBool == true) {
            PublicPlayground = true;
        }
        else if (N["playground"]["public"].AsBool == false) {
            PublicPlayground = false;
        }
        else {
            Debug.Log("Error public field in save file is not a boolean");
        }
        playgroundSwitch.SetToggle(PublicPlayground);


        string imageUrl = BaseUrlOfApi + "/images/get.php?userId=" + UserId + "&designId=" + savedPlaygroundId;
        string imageJSON = "";

        WWW imageWWW = new WWW(imageUrl);
        yield return imageWWW;
        if (!string.IsNullOrEmpty(imageWWW.error)) {
            Debug.LogError("LoadUser API call error (" + jsonUrl + "): " + imageWWW.error);
            yield break;
        }

        imageJSON = imageWWW.text;

        JSONNode N2 = JSON.Parse(imageJSON);

        if (N2["status"].Value != "true") {
            Debug.LogError("Playground not found.");
            yield break;
        }

        if (N["status"].Value != "true") {
            Debug.LogError("Playground not found.");
            yield break;
        }

        Debug.Log("Full Json returned from API: " + json);
        Debug.Log("Model Json returned from API: " + N["playground"]["model"] + " Name: " + N["name"]);
        yield return new WaitUntil(() => modelBundles.Count == names.Length);

        List<PhotoData> images = new List<PhotoData>();
        for (int i = 0; i < N2["images"].Count; i++) {
            WWW imageOneWWW = new WWW(N2["images"][i]["full_Url"]);
            yield return imageOneWWW;
            images.Add(new PhotoData(Vector3.zero, Quaternion.identity, Vector3.zero, N2["images"][i]["name"], imageOneWWW.texture));
        }
        //Debug.Log(images.Count);
        LoadSaveFile(N["playground"]["name"], N["playground"]["model"], images.ToArray());
    }

    /// <summary>
    /// Save a playground and upload it to the api to be stored
    /// </summary>
    /// <param name="name">Name of the playground</param>
    /// <param name="saveFile">Savefile contain the data to be saved</param>
    public IEnumerator SavePlayground(string name, SaveFile saveFile) {
        // Make sure all assetbundles have been downloaded
        yield return new WaitUntil(() => modelBundles.Count == names.Length);

        // Convert savefile to JSON
        string saveJSON = JsonUtility.ToJson(saveFile);
        isSaving = true;

        // Take a screenshot
        // In webgl the UI is hidden for the screenshot
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

        // Enable the UI again
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;

        // Create a Web Form to submit saved data
        WWWForm form = new WWWForm();
        form.AddField("userId", UserId);
        form.AddField("name", name);
        if (preexistingPlayground) {
            form.AddField("designId", DesignId);
        }
        else {
            preexistingPlayground = true;
        }
        form.AddField("model", saveJSON);
        form.AddBinaryData("screenshot", bytes, "screenShot_" + name + ".png", "image/png");
        form.AddField("public", PublicPlayground.ToString().ToLower());

        //Send the form to the api
        //Debug.Log("Saving: Name: " + name + " User: " + UserId + " Model: " + saveFile);
        Debug.Log(apiUrl);
        WWW www = new WWW(apiUrl, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.Log("SavePlayground API call error: " + www.error);
            yield break;
        }

        // Response from server
        json = www.text;
        Debug.Log(json);

        JSONNode N = JSON.Parse(json);

        // Check if server had success
        if (N["status"].Value != "true") {
            Debug.LogError("Error with saving.");
            yield break;
        }

        // Get the design id the server has assigned to the playground
        DesignId = N["playground"]["id"];
        // Get the screenshot URL the server has assigned
        screenShotURL = N["playground"]["screenshot_Url"];
        //Debug.Log("ScreenshotURL is " + DataManager.Instance.ScreenShotURL);

        // Move out into own function
        // Upload photos
        Debug.Log(saveFile.ToString());
        foreach (PhotoData photo in saveFile.photos) {
            WWWForm imageForm = new WWWForm();
            imageForm.AddField("userId", UserId);
            imageForm.AddField("designId", DesignId);
            imageForm.AddField("name", photo.name);
            imageForm.AddBinaryData("image", photo.Image.EncodeToPNG());

            WWW imageWWW = new WWW(BaseUrlOfApi + "/images/save.php", imageForm);
            yield return imageWWW;

            yield return new WaitForSecondsRealtime(0.5f);

            if (!string.IsNullOrEmpty(imageWWW.error)) {
                Debug.Log("SavePlayground API call error: " + imageWWW.error);
                yield break;
            }

            json = imageWWW.text;
            Debug.Log(json);

            JSONNode N2 = JSON.Parse(json);
            if (N2["status"].Value != "true") {
                Debug.LogError("Error with saving.");
                yield break;
            }
        }

        isSaving = false;
        //Debug.Log(isSaving);
    }

    /// <summary>
    /// Loads data from the model manifest
    /// </summary>
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

        // Add all objects to either elements and landscape lists
        objectInfoList = new List<DesignInfo>();

        List<string> objectCatagories = new List<string>();
        List<string> landscapeCatagories = new List<string>();

        Debug.Log("Line is " + N);
        for (int i = 0; i < N["Objects"].Count; i++) {
            DesignInfo info = new DesignInfo(N["Objects"][i]["Name"].Value, N["Objects"][i]["ImageName"].Value, N["Objects"][i]["ModelName"].Value, N["Objects"][i]["MainCategory"].Value, N["Objects"][i]["Category"].ToString());
            objectInfoList.Add(info);
            foreach (string category in info.Category) {
                if (info.MainCategory == "Elements" && !objectCatagories.Contains(category)) {
                    objectCatagories.Add(category);
                }
                else if (info.MainCategory == "Landscape" && !landscapeCatagories.Contains(category)) {
                    landscapeCatagories.Add(category);
                }
            }

        }

        objectInfoList.Sort();

        UIManager.Instance.SetObjectData(objectInfoList, "Elements", "None");
        UIManager.Instance.SetLandscapeData(objectInfoList, "Landscape", "None");
        objectDropdown.GetComponent<DropdownSorter>().setCatagories(objectCatagories);
        landscapeDropdown.GetComponent<DropdownSorter>().setCatagories(landscapeCatagories);
    }

    /// <summary>
    /// Load models in the save file into the world at their positions
    /// </summary>
    /// <param name="name">Name is the playground</param>
    /// <param name="saveJSON">Text of the JSON file</param>
    /// <param name="saveImages">Images that were savedin the playground</param>
    public void LoadSaveFile(string name, string saveJSON, PhotoData[] saveImages) {
        SaveFile saveFile = JsonUtility.FromJson<SaveFile>(saveJSON);
        //Debug.Log(name);
        playgroundName.text = name;
        width.text = saveFile.width.ToString();
        height.text = saveFile.length.ToString();
        ModelData[] models = saveFile.models;
        for (int i = 0; i < models.Length; i++) {
            StartCoroutine(PlayerManager.LoadObjectAtPositionAndRotation(objectInfoList, models[i].name, models[i].position, models[i].rotation, models[i].scale));
        }

        // HERE WE GET EACH IMAGE AND LOAD IT
        PhotoData[] photos = saveFile.photos;
        Debug.Log("SaveFile photo length: " + photos.Length + ", SaveImages length: " + saveImages.Length);
        if (photos.Length != saveImages.Length) {
            Debug.Log("Error images missing");
        }
        foreach (PhotoData photo in photos) {
            foreach (PhotoData file in saveImages) {
                if (photo.name == file.name) {
                    Debug.Log(file.Image.width);
                    PlayerManager.LoadPhoto(photo.position, photo.rotation, photo.scale, file.Image);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Download and store the assetbundle so that it is quicker to load to the
    /// next time the app is loaded
    /// </summary>
    /// <param name="bundleName">Assetbundle to be downloaded</param>
    IEnumerator DownloadAndCache(string bundleName) {
        while (!Caching.ready)
            yield return null;
        //Debug.Log("Started downloading " + bundleName);
        // Increment version number with new assest bundles
        var www = WWW.LoadFromCacheOrDownload(BaseUrlOfDesigner + "AssetBundles/Android/" + bundleName, 10);
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) {
            Debug.Log(www.error);
            yield return null;
        }
        //Debug.Log("Finished downloading " + bundleName);
        modelBundles.Add(www.assetBundle);
    }
}
