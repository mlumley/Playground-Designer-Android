using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ClickAndGetImage : MonoBehaviour {

	Button thisButton;

	public RawImage ImageTest;

	public Texture TextureTest;

	void Start ()
	{
		thisButton = GetComponent<Button> ();
		thisButton.onClick.AddListener (GetImageOnClick);
	}

	public void GetImageOnClick()
    {
#if UNITY_EDITOR
		GetImageInEditor();
#else
        // NOTE: gameObject.name MUST BE UNIQUE!!!!
        GetImage.GetImageFromUserAsync(gameObject.name, "ReceiveImage");
#endif
    }

    static string s_dataUrlPrefix = "data:image/png;base64,";
    public void ReceiveImage(string dataUrl)
    {
        if (dataUrl.StartsWith(s_dataUrlPrefix))
        {
            byte[] pngData = System.Convert.FromBase64String(dataUrl.Substring(s_dataUrlPrefix.Length));

            // Create a new Texture (or use some old one?)
            Texture2D tex = new Texture2D(1, 1); // does the size matter?
			
            if (tex.LoadImage(pngData))
            {
				ImageTest.texture = tex;
				UploadPhotoPanelManager.Instance.ResizePanel(new Vector2(tex.width, tex.height));
            }
            else
            {
                Debug.LogError("could not decode image");
            }
        }
        else
        {
            Debug.LogError("Error getting image:" + dataUrl);
        }


    }

	void GetImageInEditor ()
	{
		if(TextureTest)
		{
			ImageTest.texture = TextureTest;
			UploadPhotoPanelManager.Instance.ResizePanel(new Vector2(TextureTest.width, TextureTest.height));
		}
		else
		{
			Debug.LogError("TextureTest IS NULL");
		}
	}
}


