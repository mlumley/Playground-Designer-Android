using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class UploadImage : MonoBehaviour {

    public GameObject MainCanvas;

    [DllImport("__Internal")]
    private static extern void ImageUploaderCaptureClick();

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
        photoObject.transform.position = Vector3.zero;
        photoObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0f));
        photoObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
        photoObject.GetComponent<BoxCollider>().center = photoObject.GetComponent<SpriteRenderer>().sprite.bounds.center;
        photoObject.GetComponent<BoxCollider>().size = photoObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        PlayerManager.Instance.SelectObject(photoObject);
    }
}