using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

/// <summary>
/// Handles uploading and downloading of images to and from the server
/// </summary>
public class ImageManager : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void ImageUploaderCaptureClick();

    [DllImport("__Internal")]
    private static extern void ImageDownloaderCaptureClick(string imageName, string bytes);

    byte[] bytes;

    /// <summary>
    /// Download a image from the URL and create a texture object
    /// </summary>
    /// <param name="url">URL to download the image from</param>
    IEnumerator LoadTexture(string url) {
        WWW image = new WWW(url);
        yield return image;
        Texture2D texture = new Texture2D(1, 1);
        image.LoadImageIntoTexture(texture);
        Debug.Log("Loaded image size: " + texture.width + "x" + texture.height);
        CreatePhoto(texture);
    }
    
    /// <summary>
    /// Open the file select dialogue to up load a photo
    /// </summary>
    public void OnButtonPointerDown() {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel("Open image", "", "jpg,png,bmp");
        if (!System.String.IsNullOrEmpty(path))
            StartCoroutine(LoadTexture("file:///" + path));
#elif UNITY_WEBGL
        ImageUploaderCaptureClick();
#endif
    }

    /// <summary>
    /// Converts the texture object to a photoObject which can be place in the world and then selects the object
    /// </summary>
    /// <param name="texture">The texture with the image applied</param>
    void CreatePhoto(Texture2D texture) {
        GameObject photoObject = Instantiate(Resources.Load("UIPrefabs/PhotoWorldObject")) as GameObject;
        photoObject.name = "PhotoObject" + Guid.NewGuid().ToString();
        photoObject.transform.position = Vector3.zero;
        photoObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0f));
        photoObject.GetComponent<BoxCollider>().center = photoObject.GetComponent<SpriteRenderer>().sprite.bounds.center;
        photoObject.GetComponent<BoxCollider>().size = photoObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        PlayerManager.Instance.SelectObject(photoObject);
    }

    /// <summary>
    /// Downloads a screenshot and saves it in the downloads folder if done through
    /// the browser
    /// </summary>
    public void DownloadImage() {
        StartCoroutine(TakeImage());
    }

    /// <summary>
    /// Takes the screenshot
    /// </summary>
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