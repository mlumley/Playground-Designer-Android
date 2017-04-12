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
    public string BaseUrlOfDesigner;
    public string UserId;
    public string DesignId;
    private static bool publicPlayground = false;
    public ToogleSwitch playgroundSwitch;

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

    public bool PublicPlayground{
        get {
            return publicPlayground;
        }
        set {
            publicPlayground = value;
        }
    }


    void Start() {
        StartCoroutine(LoadData());

        //todo: get ID in web player (eg https://docs.unity3d.com/ScriptReference/Application-absoluteURL.html)
        string userId = "1"; //test user
        int savedPlaygroundId = 0; //not saved
        string creatorId = "0";

        //string url = "http://playgroundideas.endzone.io/app-api/wp-simulate/app.php?userId=1&designId=2";

        Debug.Log("Initial userId: " + userId + ", savedPlaygroundId: " + savedPlaygroundId);

        string[] names1 = { "balance", "bridges", "buildings", "climbing", "cubbies", "furniture", "ground_cover", "hills", "imaginative", "ladders", "monkey_bars", "musical", "natural", "other", "poles", "rocks", "sandpits", "seesaws", "slides", "sports", "swings", "trees", "tunnels" };
        names = names1;


        foreach (string name in names) {
            //StartCoroutine(Download(name));
            StartCoroutine(DownloadAndCache(name));
        }

#if UNITY_EDITOR

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

        this.UserId = userId;
        this.DesignId = savedPlaygroundId.ToString();

        //StartCoroutine(LoadUser(userId));
        if (creatorId != "0" && savedPlaygroundId != 0) {
            Debug.Log("Loading public playground");
            StartCoroutine(WaitTillDownloadedAssets(preexistingPlayground));
            StartCoroutine(LoadSavedPlayground(creatorId,savedPlaygroundId));
            StartCoroutine(AddView(creatorId, DesignId));
            DesignId = "0";
        }
        else if (savedPlaygroundId != 0) {
            Debug.Log("Loading saved playground");
            preexistingPlayground = true;
            StartCoroutine(WaitTillDownloadedAssets(preexistingPlayground));
            StartCoroutine(LoadSavedPlayground(this.UserId, savedPlaygroundId));
            StartCoroutine(AddView(UserId, DesignId));
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

    IEnumerator AddView(string UserId, string DesignId) {
        WWW www = new WWW(BaseUrlOfApi + "playgrounds/views.php?UserId=" + UserId + "&DesignId=" + DesignId);

        if (!string.IsNullOrEmpty(www.error)) {
            Debug.LogError("View API call error " + www.error);
            yield break;
        }
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

    // Not used
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

        json = imageWWW.text;

        JSONNode N2 = JSON.Parse(json);

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


    public IEnumerator SavePlayground(string name, SaveFile saveFile) {
        yield return new WaitUntil(() => modelBundles.Count == names.Length);
        string saveJSON = JsonUtility.ToJson(saveFile);
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
        form.AddField("model", saveJSON);
        form.AddBinaryData("screenshot", bytes, "screenShot_" + name + ".png", "image/png");
        form.AddField("public", PublicPlayground.ToString().ToLower());

        //Debug.Log("Saving: Name: " + name + " User: " + UserId + " Model: " + saveFile);
        Debug.Log(apiUrl);
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

        //HAVE TO POST IMAGES HERE CAUSE WE NEED DESIGNID
        /*  Save Image: /images/save.php
            Post vars: userId, designId (playground id), name (optional), image (image file)
        */

        // Delete exisiting photos
        /*string imageUrl = BaseUrlOfApi + "/images/get.php?userId=" + UserId + "&designId=" + DesignId;
        string imageJSON = "";

        WWW imageOneWWW = new WWW(imageUrl);
        yield return imageOneWWW;
        if (!string.IsNullOrEmpty(imageOneWWW.error)) {
            Debug.Log("SavePlayground API call error: " + imageOneWWW.error);
            yield break;
        }

        imageJSON = imageOneWWW.text;

        JSONNode N3 = JSON.Parse(imageJSON);
        for(int i = 0; i < N3["images"].Count; i++) {
            WWW delete = new WWW(BaseUrlOfApi + "/images/delete.php?userId=" + UserId + "&designId=" + DesignId + "&imageId=" + i);
            yield return delete;
            Debug.Log(delete.text);
        }*/

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
        //JsonData data = JsonUtility.FromJson<JsonData>(result);

        objectInfoList = new List<DesignInfo>();

        List<string> objectCatagories = new List<string>();
        List<string> landscapeCatagories = new List<string>();

        Debug.Log("Line is " + N);
        for (int i = 0; i < N["Objects"].Count; i++) {
            //foreach(DesignInfo info in data.data) {
            DesignInfo info = new DesignInfo(N["Objects"][i]["Name"].Value, N["Objects"][i]["ImageName"].Value, N["Objects"][i]["ModelName"].Value, N["Objects"][i]["MainCategory"].Value, N["Objects"][i]["Category"].ToString());
            objectInfoList.Add(info);
            //Debug.Log(info.Category[0].ToString());
            /*if (N["Objects"][i]["MainCategory"].Value == "Elements" && !objectCatagories.Contains(N["Objects"][i]["Category"].Value))
                objectCatagories.Add(N["Objects"][i]["Category"].Value);
            else if (N["Objects"][i]["MainCategory"].Value == "Landscape" && !landscapeCatagories.Contains(N["Objects"][i]["Category"].Value))
                landscapeCatagories.Add(N["Objects"][i]["Category"].Value);*/
            //Debug.Log(model);
            //DesignInfo info = JsonUtility.FromJson<DesignInfo>(model);
            //Debug.Log(N["Objects"][i]["Category"]);
            //objectInfoList.Add(N["Objects"][i]);
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

        //Debug.Log(objectCatagories.Count);

        UIManager.Instance.SetObjectData(objectInfoList, "Elements", "None");
        UIManager.Instance.SetLandscapeData(objectInfoList, "Landscape", "None");
        objectDropdown.GetComponent<DropdownSorter>().setCatagories(objectCatagories);
        landscapeDropdown.GetComponent<DropdownSorter>().setCatagories(landscapeCatagories);
    }


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

    // Not in use
    IEnumerator Download(string bundleName) {
        // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        //using (WWW www = WWW.LoadFromCacheOrDownload(BaseUrlOfApi + "wp-simulate/models", 1)) {
        Debug.Log("Started downloading " + bundleName);
        using (WWW www = new WWW(BaseUrlOfDesigner + "AssetBundles/" + bundleName)) {
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
        var www = WWW.LoadFromCacheOrDownload(BaseUrlOfDesigner + "AssetBundles/" + bundleName, 9);
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) {
            Debug.Log(www.error);
            yield return null;
        }
        //Debug.Log("Finished downloading " + bundleName);
        modelBundles.Add(www.assetBundle);
    }
}
