using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class UploadImage : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void ImageUploaderCaptureClick();

    [DllImport("__Internal")]
    private static extern void ImageDownloaderCaptureClick(string imageName, string bytes);

    byte[] bytes;

    IEnumerator LoadTexture(string url) {
        WWW image = new WWW(url);
        yield return image;
        Texture2D texture = new Texture2D(1, 1);
        image.LoadImageIntoTexture(texture);
        Debug.Log("Loaded image size: " + texture.width + "x" + texture.height);
        CreatePhoto(texture);
    }

    void FileSelected(string url) {
        StartCoroutine(LoadTexture(url));
    }

    public void OnButtonPointerDown() {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel("Open image", "", "jpg,png,bmp");
        if (!System.String.IsNullOrEmpty(path))
            FileSelected("file:///" + path);
#elif UNITY_WEBGL

        ImageUploaderCaptureClick();
#endif
    }

    void CreatePhoto(Texture2D texture) {
        //GameObject PhotoWorldPanelPrefab = Instantiate(Resources.Load("UIPrefabs/WorldPhotoPanel")) as GameObject;
        //PhotoWorldPanelPrefab.transform.SetParent(MainCanvas.transform);
        //PhotoWorldPanelPrefab.transform.SetAsFirstSibling();
        //PhotoWorldPanelPrefab.transform.localPosition = Vector3.zero;
        //PhotoWorldPanelPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(200,100);
        //WorldPhotoPanel worldPhotoPanel = PhotoWorldPanelPrefab.GetComponent<WorldPhotoPanel>();
        //worldPhotoPanel.SetImage(texture);
        //worldPhotoPanel.Select(true);
        GameObject photoObject = Instantiate(Resources.Load("UIPrefabs/PhotoWorldObject")) as GameObject;
        photoObject.name = "PhotoObject" + Guid.NewGuid().ToString();
        photoObject.transform.position = Vector3.zero;
        photoObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0f));
        //photoObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
        photoObject.GetComponent<BoxCollider>().center = photoObject.GetComponent<SpriteRenderer>().sprite.bounds.center;
        photoObject.GetComponent<BoxCollider>().size = photoObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        PlayerManager.Instance.SelectObject(photoObject);
    }


    public void DownloadImage() {
        StartCoroutine(TakeImage());


    }

    IEnumerator TakeImage() {
        yield return null;
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

        bytes = tex.EncodeToPNG();
        Destroy(tex);
        string bytestring = Convert.ToBase64String(bytes);


        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
#if UNITY_EDITOR
        File.WriteAllBytes(Application.dataPath + "/SavedScreen.png", bytes);
#elif UNITY_WEBGL

        ImageDownloaderCaptureClick("SavedScreen", bytestring);
#endif
    }
}